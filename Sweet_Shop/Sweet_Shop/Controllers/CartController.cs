using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Sweet_Shop.Extensions;
using Sweet_Shop.Models;
using System.Data;
using System.Data.SqlClient;
using System.Net;
using System.Numerics;
using System.Reflection;

public class CartController : Controller
{
    private readonly IConfiguration _configuration;
    public string connectionString { get; set; }

    public CartController(IConfiguration configuration)
    {
        _configuration = configuration;
        connectionString = _configuration.GetConnectionString("database");
    }

    public IActionResult Index()
    {
        var cart = GetCart();
        return View(cart);
    }

    public IActionResult AddToCart(int productId)
    {
        var product = GetProductById(productId);
        if (product == null)
        {
            return NotFound();
        }

        var cart = GetCart();
        cart.AddToCart(product);
        SaveCart(cart);

        // Get the URL of the previous page
        string returnUrl = Request.Headers["Referer"].ToString();

        // If the previous page URL is empty or null, redirect to the default action
        if (string.IsNullOrEmpty(returnUrl))
        {
            return RedirectToAction("Index");
        }

        // Redirect the user back to the previous page
        return Redirect(returnUrl);
    }
    public IActionResult buyNow(int productId)
    {
        var product = GetProductById(productId);
        if (product == null)
        {
            return NotFound();
        }

        var cart = GetCart();
        cart.AddToCart(product);
        SaveCart(cart);

     
        // Redirect the user back to the previous page
        return Redirect("Index");
    }

    public IActionResult RemoveFromCart(int productId)
    {
        var product = GetProductById(productId);
        if (product == null)
        {
            return NotFound();
        }

        var cart = GetCart();
        cart.RemoveFromCart(product);
        SaveCart(cart);

        return RedirectToAction("Index");
    }


    // Sample method to retrieve a product from the database(replace with your actual logic)
    public Product GetProductById(int id)
    {
        // Example logic to retrieve product from database
        using (SqlConnection connection = new SqlConnection(connectionString))
        {
            string query = "SELECT Id, Name, price, stock, salePrice, imageUrl, category, IsOnSale FROM Products WHERE Id = @Id";

            SqlCommand command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@Id", id);
            connection.Open();
            SqlDataReader reader = command.ExecuteReader();

            if (reader.Read())
            {
                Product product = new Product
                {
                    Id = Convert.ToInt32(reader["Id"]),
                    Name = reader["Name"].ToString(),
                    price = Convert.ToSingle(reader["price"]),
                    stock = Convert.ToInt32(reader["stock"]),
                    salePrice = Convert.ToSingle(reader["salePrice"]),
                    imageUrl = reader["imageUrl"].ToString(),
                    category = reader["category"].ToString(),
                    IsOnSale = Convert.ToBoolean(reader["IsOnSale"])
                };
                reader.Close();
                connection.Close();
                return product;
            }

            reader.Close();
            connection.Close();
            return null; // Product not found
        }
    }

    public IActionResult Checkout()
    {
        var cart = GetCart();

        if (cart.CartItems.Any())
        {
            // Check if stock is sufficient for each product in the cart
            bool stockSufficient = true;
            List<string> outOfStockProducts = new List<string>();

            foreach (var cartItem in cart.CartItems)
            {
                Product product = GetProductById(cartItem.Product.Id);
                if (product != null && product.stock < cartItem.Quantity)
                {
                    stockSufficient = false;
                    outOfStockProducts.Add(product.Name);
                }
            }

            if (stockSufficient)
            {
                var payment = new PaymentModel
                {
                    Cart = cart
                };
                return View(payment);
            }
            else
            {
                // Display an error message with the list of out-of-stock products
                ViewBag.ErrorMessage = $"The following products are out of stock or have insufficient quantity: {string.Join(", ", outOfStockProducts)}";
                return View("Index", cart);
            }
        }

        return RedirectToAction("Index");
    }
    public IActionResult ProcessPayment()
    {
        string customerId = HttpContext.Session.GetString("CustomerID");
        string firstName = HttpContext.Session.GetString("FirstName");
        string lastName = HttpContext.Session.GetString("LastName");
        string email = HttpContext.Session.GetString("Email");
        string phone = HttpContext.Session.GetString("Phone");
        string cAddress = HttpContext.Session.GetString("CAddress");

        if (!string.IsNullOrEmpty(customerId) && !string.IsNullOrEmpty(firstName) && !string.IsNullOrEmpty(email))
        {
            // All necessary session values are present

            // Create a CustomerModel object with the retrieved session values
            var loggedInCustomer = new CustomerModel
            {
                CustomerID = customerId,
                FirstName = firstName,
                LastName = lastName,
                Email = email,
                Phone = phone,
                CAddress = cAddress
            };

            // Pass the CustomerModel object to the view
            return View(loggedInCustomer);
        }
        else
        {
            // Handle the case where session values are missing
            // Here you might want to redirect to a login page or take appropriate action
            // In this example, we return a view with a null CustomerModel
            return View((CustomerModel)null);
        }

        //return View("ProcessPayment");
    }

    [HttpPost]
    public IActionResult PlaceOrder(PaymentModel Payment)
    {
        Payment.Cart = GetCart();

        // Get the cart items
        var cart = GetCart();
        // Server-side validation for expiry date
        if (!IsExpiryDateValid(Payment.ExpiryDate))
        {
            ModelState.AddModelError(nameof(Payment.ExpiryDate), "Expiry date must be in the future.");
            return View("Checkout", Payment);
        }

        Cart cartforOre = Payment.Cart;
        Ordering(cartforOre);
        // Check if the cart has any items
        if (cart.CartItems.Any())
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                // Begin a database transaction
                using (SqlTransaction transaction = connection.BeginTransaction())
                {
                    try
                    {
                        // Iterate through each item in the cart
                        foreach (var cartItem in cart.CartItems)
                        {
                            var productId = cartItem.Product.Id;
                            var quantity = cartItem.Quantity;

                            // Update the stock of each product in the cart
                            bool stockUpdated = CartController.SQLUpdateProductStock(productId, -quantity, connection, transaction);

                            // Rollback the transaction if stock update fails
                            if (!stockUpdated)
                            {
                                transaction.Rollback();
                                // Optionally, you can display an error message or handle the failure
                                return RedirectToAction("OrderFailed");
                            }
                        }

                        // If all stock updates were successful, process the order
                        // You can save the order details to a database here

                        // Clear the cart after successful order placement


                        ClearCart(cart);
                        SaveCart(cart);

                        // Commit the transaction
                        transaction.Commit();

                        // Optionally, you can display a success message or redirect to a confirmation page
                        return RedirectToAction("OrderConfirmation");
                    }
                    catch (Exception ex)
                    {
                        // Handle any exceptions here (e.g., log the error)
                        Console.WriteLine(ex.Message);
                        // Rollback the transaction if an exception occurs
                        transaction.Rollback();
                        // Optionally, you can display an error message or redirect to a failure page
                        return RedirectToAction("OrderFailed");
                    }
                }
            }
        }
        else
        {
            // Handle case where cart is empty
            // Optionally, you can display a message or redirect to a page
            return RedirectToAction("EmptyCart");
        }
        //}

        // If the model state is not valid, return the checkout view with validation errors
        return View("Checkout", Payment);
    }

    private bool IsExpiryDateValid(string expiryDate)
    {
        if (DateTime.TryParseExact(expiryDate, "MM/yyyy", null, System.Globalization.DateTimeStyles.None, out DateTime expiryDateTime))
        {
            return expiryDateTime > DateTime.Now;
        }
        return false;
    }
    public static bool SQLUpdateProductStock(int productId, int amountDifference, SqlConnection connection, SqlTransaction transaction)
    {

        // Check if the amountDifference is negative, and if so, check if we have enough stock in the database.
        // We multiply by -1 for our stock check function.
        if (amountDifference < 0 && !SQLCheckProductStock(productId, (-1) * amountDifference, connection, transaction))
        {
            transaction.Rollback(); // Rollback the transaction and return false if the update fails.
            return false;
        }

        string query = @"UPDATE Products SET stock = stock + @stock WHERE Id = @productId;";

        try
        {
            using (SqlCommand command = new SqlCommand(query, connection, transaction))
            {
                command.Parameters.AddWithValue("@stock", amountDifference);
                command.Parameters.AddWithValue("@productId", productId);

                // Execute the command and check if we updated the product stock.
                int rowsAffected = command.ExecuteNonQuery();
                if (rowsAffected > 0)
                {
                    return true; // Stock successfully changed.
                }
                else
                {
                    transaction.Rollback(); // Rollback the transaction and return false if the update fails.
                    return false; // Failed to change stock.
                }
            }
        }
        catch (Exception ex)
        {
            transaction.Rollback(); // Rollback the transaction and return false if the update fails.
            Console.WriteLine(ex.Message); // Print the error.
            return false;
        }
    }
    private static bool SQLCheckProductStock(int productId, int requiredAmount, SqlConnection connection, SqlTransaction transaction)
    {
        string query = @"SELECT stock FROM Products WHERE Id = @productId;";

        try
        {
            using (SqlCommand command = new SqlCommand(query, connection, transaction))
            {
                command.Parameters.AddWithValue("@productId", productId);

                // Execute the command to fetch the current stock of the product.
                object result = command.ExecuteScalar();
                if (result != null && result != DBNull.Value)
                {
                    int currentStock = (int)result;
                    // Check if the current stock is sufficient.
                    return currentStock >= requiredAmount;
                }
                else
                {
                    // Handle case where product ID is invalid or not found.
                    // In this example, assuming false (insufficient stock).
                    return false;
                }
            }
        }
        catch (Exception ex)
        {
            // Handle any exceptions here (e.g., log the error).
            Console.WriteLine(ex.Message);
            // Return false as we cannot determine the stock status.
            return false;
        }
    }
    public IActionResult OrderConfirmation()
    {
        // Display a confirmation message or view
        return View();
    }
    private Cart GetCart()
    {
        Cart cart = HttpContext.Session.GetObject<Cart>("Cart");
        if (cart == null)
        {
            cart = new Cart();
            HttpContext.Session.SetObject("Cart", cart);
        }
        return cart;
    }

    private void SaveCart(Cart cart)
    {
        HttpContext.Session.SetObject("Cart", cart);
    }
    private void ClearCart(Cart cart) { cart.ClearCart(); }
    public IActionResult UpdateQuantity(int productId, int changeQuantity)
    {
        var product = GetProductById(productId);
        if (product == null)
        {
            return NotFound();
        }

        var cart = GetCart();

        // Find the cart item corresponding to the product
        var cartItem = cart.CartItems.FirstOrDefault(item => item.Product.Id == productId);
        if (cartItem != null)
        {
            // Update the quantity
            int newQuantity = cartItem.Quantity + changeQuantity;
            if (newQuantity <= 0)
            {
                // If the new quantity is zero or negative, remove the item from the cart
                cart.RemoveFromCart(product);
            }
            else
            {
                cartItem.Quantity = newQuantity;
            }

            SaveCart(cart);
        }

        return RedirectToAction("Index");
    }



    public void Ordering(Cart cart)
    {
        // Get the customer ID from session
        var customerId = HttpContext.Session.GetString("CustomerID");

        using (var connection = new SqlConnection(connectionString))
        {
            connection.Open();

            // Get the last OrderId from the database
            var getLastOrderIdQuery = "SELECT ISNULL(MAX(OrdersId), 0) FROM Orders;";
            using (var command = new SqlCommand(getLastOrderIdQuery, connection))
            {
                int lastOrderId = Convert.ToInt32(command.ExecuteScalar());
                lastOrderId++;

                // Insert the new order into the database
                var insertOrderQuery = "INSERT INTO Orders (OrdersId, CustomerId, OrderDate, ProductId, Quantity) VALUES (@OrdersId, @CustomerId, @OrderDate, @ProductId, @Quantity);";

                foreach (var cartItem in cart.CartItems)
                {
                    using (var orderCommand = new SqlCommand(insertOrderQuery, connection))
                    {
                        orderCommand.Parameters.AddWithValue("@OrdersId", lastOrderId);
                        orderCommand.Parameters.AddWithValue("@CustomerId", customerId);
                        orderCommand.Parameters.AddWithValue("@OrderDate", DateTime.Now);
                        orderCommand.Parameters.AddWithValue("@ProductId", cartItem.Product.Id);
                        orderCommand.Parameters.AddWithValue("@Quantity", cartItem.Quantity);
                        orderCommand.ExecuteNonQuery();
                    }
                }
            }
        }

        // Optionally, you can clear the cart after placing the order
        //cart.Clear();

        // Redirect to a success page or return a success message
        // return RedirectToAction("OrderConfirmation");
    }


    ////////////////////////order history display in the custtomer profile /////////////////////////////////////
    public List<Order> GetOrdersForCustomer(int customerId)
    {
        List<Order> orders = new List<Order>();

        using (var connection = new SqlConnection(connectionString))
        {
            connection.Open();

            // Query to retrieve orders for the specified customer ID
            var query = "SELECT OrdersId, CustomerId, ProductId, Quantity, OrderDate FROM Orders WHERE CustomerId = @CustomerId;";

            using (var command = new SqlCommand(query, connection))
            {
                command.Parameters.AddWithValue("@CustomerId", customerId);

                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        Order order = new Order
                        {
                            OrdersId = reader.GetInt32(0),
                            CustomerId = reader.GetInt32(1),
                            productId = reader.GetInt32(2),
                            Quantity = reader.GetInt32(3),
                            OrderDate = reader.GetDateTime(4)
                        };

                        orders.Add(order);
                    }
                }
            }
        }

        return orders;
    }
    public IActionResult Profile()
    {
        var customerId = HttpContext.Session.GetString("CustomerID");
        var orders = GetOrdersForCustomer(int.Parse(customerId));
        // קבלת רשימת OrdersId בלי חזרות מהרשימה ordersList
        List<int> uniqueOrdersIds = orders.Select(order => order.OrdersId).Distinct().ToList();

        return View("CustomerProfile", uniqueOrdersIds);
        //return View("CustomerProfile", orders);
    }
    //public IActionResult CustomerProfile()
    //{

    //    return View("CustomerProfile");
    //}

    //public IActionResult OrderDetails(int id)
    //{

    //    var orders = GetOrdersByOrderId(id);
    //    return View("OrderDetails", orders);
    //}
    public List<Order> GetOrdersByOrderId(int orderId)
    {
        List<Order> orders = new List<Order>();

        using (var connection = new SqlConnection(connectionString))
        {
            connection.Open();

            // Query to retrieve orders for the specified order ID
            var query = "SELECT OrdersId, CustomerId, ProductId, Quantity, OrderDate FROM Orders WHERE OrdersId = @OrdersId;";

            using (var command = new SqlCommand(query, connection))
            {
                command.Parameters.AddWithValue("@OrdersId", orderId);

                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        Order order = new Order
                        {
                            OrdersId = reader.GetInt32(0),
                            CustomerId = reader.GetInt32(1),
                            productId = reader.GetInt32(2),
                            Quantity = reader.GetInt32(3),
                            OrderDate = reader.GetDateTime(4)
                        };

                        orders.Add(order);
                    }
                }
            }
        }

        return orders;
    }


    public List<(Order order, Product product)> GetOrderDetailsByOrderId(int orderId)
    {
        List<(Order order, Product product)> orderDetails = new List<(Order order, Product product)>();

        using (var connection = new SqlConnection(connectionString))
        {
            connection.Open();

            // Query to retrieve order details along with product details for the specified order ID
            var query = @"SELECT o.OrdersId, o.CustomerId, o.ProductId, o.Quantity, o.OrderDate,
                             p.Id, p.Name, p.Price,p.ImageUrl
                      FROM Orders o
                      INNER JOIN Products p ON o.ProductId = p.Id
                      WHERE o.OrdersId = @OrdersId;";

            using (var command = new SqlCommand(query, connection))
            {
                command.Parameters.AddWithValue("@OrdersId", orderId);

                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        Order order = new Order
                        {
                            OrdersId = reader.GetInt32(0),
                            CustomerId = reader.GetInt32(1),
                            productId = reader.GetInt32(2),
                            Quantity = reader.GetInt32(3),
                            OrderDate = reader.GetDateTime(4)
                        };

                        Product product = new Product
                        {
                            Id = reader.GetInt32(5),
                            Name = reader.GetString(6),
                            price = Convert.ToSingle(reader["price"]),
                            imageUrl = reader["imageUrl"].ToString(),

                        };

                        orderDetails.Add((order, product));
                    }
                }
            }
        }

        return orderDetails;
    }
    public IActionResult OrderDetails(int id)
    {
        var orderDetails = GetOrderDetailsByOrderId(id);
        return View("OrderDetails", orderDetails);
    }

}

