﻿using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Sweet_Shop.Extensions;
using Sweet_Shop.Models;
using System.Data;
using System.Data.SqlClient;

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

        return RedirectToAction("Index");
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

    //public IActionResult Checkout()
    //{
    //    var cart = GetCart();
    //    if (cart.CartItems.Any())
    //    {
    //        using (SqlConnection connection = new SqlConnection(connectionString))
    //        {
    //            connection.Open();
    //            using (SqlTransaction transaction = connection.BeginTransaction())
    //            {
    //                try
    //                {
    //                    foreach (var cartItem in cart.CartItems)
    //                    {
    //                        var productId = cartItem.Product.Id;
    //                        var quantity = cartItem.Quantity;
    //                        // Update the stock of each product in the cart
    //                        bool stockUpdated = CartController.SQLCheckProductStock(productId, -quantity, connection, transaction);
    //                        if (!stockUpdated)
    //                        {
    //                            // Rollback the transaction if stock update fails
    //                            transaction.Rollback();
    //                            // Optionally, you can display an error message or handle the failure
    //                            return RedirectToAction("OrderFailed");
    //                        }
    //                    }
    //                }
    //                    var payment = new PaymentModel
    //                    {
    //                        Cart = cart
    //                    };
    //                return View(payment);
    //            }
    //         return RedirectToAction("Index");
    //            }
    //        } } }
    public IActionResult Checkout()
    {
        var cart = GetCart();
        bool insufficientStock = false; // Flag to track insufficient stock

        if (cart.CartItems.Any())
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                using (SqlTransaction transaction = connection.BeginTransaction())
                {
                    try
                    {
                        foreach (var cartItem in cart.CartItems)
                        {
                            var productId = cartItem.Product.Id;
                            var quantity = cartItem.Quantity;
                            // Check if the stock is sufficient for the quantity in the cart
                            bool stockUpdated = CartController.SQLCheckProductStock(productId, quantity, connection, transaction);
                            if (!stockUpdated)
                            {
                                insufficientStock = true; // Set flag to true if stock is insufficient
                                break; // Exit the loop as soon as insufficient stock is found
                            }
                        }

                        // If there's insufficient stock, set error message and return to view
                        if (insufficientStock)
                        {
                            ViewData["ErrorMessage"] = "One or more products in your cart have insufficient stock.";
                            return View("Index"); // Return to the view with error message
                        }

                        // If all stock checks were successful, proceed to checkout
                        var payment = new PaymentModel
                        {
                            Cart = cart
                        };
                        return View(payment);
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
    }



    [HttpPost]
            public IActionResult PlaceOrder(PaymentModel Payment)
            {
                if (!ModelState.IsValid)
                {
                    var cart = GetCart();
                    if (cart.CartItems.Any())
                    {
                        using (SqlConnection connection = new SqlConnection(connectionString))
                        {
                            connection.Open();
                            using (SqlTransaction transaction = connection.BeginTransaction())
                            {
                                try
                                {
                                    foreach (var cartItem in cart.CartItems)
                                    {
                                        var productId = cartItem.Product.Id;
                                        var quantity = cartItem.Quantity;
                                        // Update the stock of each product in the cart
                                        bool stockUpdated = CartController.SQLUpdateProductStock(productId, -quantity, connection, transaction);
                                        if (!stockUpdated)
                                        {
                                            // Rollback the transaction if stock update fails
                                            transaction.Rollback();
                                            // Optionally, you can display an error message or handle the failure
                                            return RedirectToAction("OrderFailed");
                                        }
                                    }
                                    // If all stock updates were successful, process the order
                                    // You can save the order details to a database here

                                    // Clear the cart after successful order placement
                                    //Payment.Cart.ClearCart();
                                    SaveCart(Payment.Cart);

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
                }

                // If the model state is not valid, return the checkout view with validation errors
                return View("Checkout", Payment);
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
        }
    