using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System;
using System.Linq;
using System.Threading.Tasks;
//using Microsoft.EntityFrameworkCore;
///
using Microsoft.AspNetCore.Mvc;
using Sweet_Shop.Models;
using Microsoft.Extensions.Configuration;
using System.Data.SqlClient;
using Sweet_Shop.ViewModels;
using System;
using System.Reflection;
using Microsoft.AspNetCore.Mvc.Routing;
using Sweet_Shop.Extensions;
using System.Data;
//using Dapper;

namespace Project.Controllers
{

    public class productsController : Controller
    {


        //}

        private readonly IConfiguration _configuration;
        public string connectionString { get; set; }


        public productsController(IConfiguration configuration)
        {
            _configuration = configuration;
            connectionString = _configuration.GetConnectionString("database");
        }
        public IActionResult Index()
        {
            return View();
        }

        //public IActionResult AddProduct(Product product)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        try
        //        {
        //            using (SqlConnection connection = new SqlConnection(connectionString))
        //            {
        //                connection.Open();
        //                string query = "INSERT INTO Products (Name, Price, Stock, SalePrice, ImageUrl, Category, IsOnSale) " +
        //                               "VALUES (@Name, @Price, @Stock, @SalePrice, @ImageUrl, @Category, @IsOnSale)";

        //                SqlCommand command = new SqlCommand(query, connection);
        //                command.Parameters.AddWithValue("@Name", product.Name);
        //                command.Parameters.AddWithValue("@Price", product.price);
        //                command.Parameters.AddWithValue("@Stock", product.stock);
        //                command.Parameters.AddWithValue("@SalePrice", product.salePrice);
        //                command.Parameters.AddWithValue("@ImageUrl", product.imageUrl);
        //                command.Parameters.AddWithValue("@Category", product.category);
        //                command.Parameters.AddWithValue("@IsOnSale", product.IsOnSale);

        //                int rowsAffected = command.ExecuteNonQuery();

        //                if (rowsAffected > 0)
        //                {
        //                    // Insertion successful
        //                    return RedirectToAction("manageProducts");
        //                }
        //                else
        //                {
        //                    // Insertion failed
        //                    ViewBag.Message = "Failed to insert product data.";
        //                    return View("addProduct", product);
        //                }
        //            }
        //        }
        //        catch (Exception ex)
        //        {
        //            // Handle exceptions, log errors, etc.
        //            ViewBag.Message = "An error occurred while processing the request.";
        //            return View("addProduct", product);
        //        }
        //    }
        //    ViewBag.Categories = new List<string> { "Raw materials for baking ", "Icing and decorating cakes", "Cake packaging", "Baking Tools" };
        //    return View("addProduct", product);
        //}
        private bool SQLCheckProduct(string productName, string imageUrl)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                // Check if product with the given name already exists
                string queryName = "SELECT COUNT(*) FROM Products WHERE Name = @name;";
                using (SqlCommand commandName = new SqlCommand(queryName, connection))
                {
                    commandName.Parameters.AddWithValue("@name", productName);
                    int countName = (int)commandName.ExecuteScalar();
                    if (countName > 0)
                    {
                        // Product with the same name already exists
                        return true;
                    }
                }

                // Check if product with the given image URL already exists
                string queryImageUrl = "SELECT COUNT(*) FROM Products WHERE ImageUrl = @imageUrl;";
                using (SqlCommand commandImageUrl = new SqlCommand(queryImageUrl, connection))
                {
                    commandImageUrl.Parameters.AddWithValue("@imageUrl", imageUrl);
                    int countImageUrl = (int)commandImageUrl.ExecuteScalar();
                    if (countImageUrl > 0)
                    {
                        // Product with the same image URL already exists
                        return true;
                    }
                }

                // If neither product with the given name nor image URL exists, return false
                return false;
            }
        }


        public IActionResult AddProduct(Product product)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    // Check if the product already exists
                    if (SQLCheckProduct(product.Name, product.imageUrl))
                    {
                        ViewBag.Message = "Product with the same name or image URL already exists.";
                        ViewBag.Categories = new List<string> { "Raw materials for baking", "Icing and decorating cakes", "Cake packaging", "Baking Tools" };
                        return View("addProduct", product);
                    }

                    using (SqlConnection connection = new SqlConnection(connectionString))
                    {
                        connection.Open();
                        string query = "INSERT INTO Products (Name, Price, Stock, SalePrice, ImageUrl, Category, IsOnSale) " +
                                       "VALUES (@Name, @Price, @Stock, @SalePrice, @ImageUrl, @Category, @IsOnSale)";

                        SqlCommand command = new SqlCommand(query, connection);
                        command.Parameters.AddWithValue("@Name", product.Name);
                        command.Parameters.AddWithValue("@Price", product.price);
                        command.Parameters.AddWithValue("@Stock", product.stock);
                        command.Parameters.AddWithValue("@SalePrice", product.salePrice);
                        command.Parameters.AddWithValue("@ImageUrl", product.imageUrl);
                        command.Parameters.AddWithValue("@Category", product.category);
                        command.Parameters.AddWithValue("@IsOnSale", product.IsOnSale);

                        int rowsAffected = command.ExecuteNonQuery();

                        if (rowsAffected > 0)
                        {
                            // Insertion successful
                            return RedirectToAction("manageProducts");
                        }
                        else
                        {
                            // Insertion failed
                            ViewBag.Message = "Failed to insert product data.";
                            return View("addProduct", product);
                        }
                    }
                }
                catch (Exception ex)
                {
                    // Handle exceptions, log errors, etc.
                    ViewBag.Message = "An error occurred while processing the request.";
                    return View("addProduct", product);
                }
            }

            // If model state is not valid, return the form with validation errors
            ViewBag.Categories = new List<string> { "Raw materials for baking ", "Icing and decorating cakes", "Cake packaging", "Baking Tools" };
            return View("addProduct", product);
        }


        public IActionResult Details(int id)
        {
            // Assuming you have a method to fetch a product by its ID from the database
            Product product = GetProductById(id);

            if (product == null)
            {
                return NotFound(); // Product not found, return 404 Not Found status
            }

            return View(product);
        }

        // Sample method to retrieve a product from the database (replace with your actual logic)
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

        public IActionResult ViewProducts(string sortOrder, string category, string IsOnSale, string searchString, string priceRange)
        {
            // Retrieve customer ID from session
            var customerId = HttpContext.Session.GetString("CustomerID");

            // Pass the customer ID to the view
            ViewData["CustomerId"] = customerId;
            List<Product> products = new List<Product>();
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                string query = "SELECT Id, Name, price, stock, salePrice, imageUrl, category, IsOnSale FROM Products";

                // Build the WHERE clause
                string whereClause = " WHERE 1=1"; // Default condition to avoid syntax error
                List<SqlParameter> parameters = new List<SqlParameter>();

                // Apply category filter if specified
                if (!string.IsNullOrEmpty(category))
                {
                    whereClause += " AND category = @Category";
                    parameters.Add(new SqlParameter("@Category", category));
                }

                // Apply IsOnSale filter if specified
                if (!string.IsNullOrEmpty(IsOnSale))
                {
                    whereClause += " AND IsOnSale = @IsOnSale";
                    parameters.Add(new SqlParameter("@IsOnSale", IsOnSale));
                }

                // Apply search filter if specified
                if (!string.IsNullOrEmpty(searchString))
                {
                    whereClause += " AND Name LIKE @SearchString";
                    parameters.Add(new SqlParameter("@SearchString", "%" + searchString + "%"));
                }

                // Apply price range filter if specified
                if (!string.IsNullOrEmpty(priceRange))
                {
                    switch (priceRange)
                    {
                        case "1-20":
                            if (IsOnSale == "0")
                                whereClause += " AND price BETWEEN 1 AND 20";
                            else
                                whereClause += " AND saleprice BETWEEN 1 AND 20";
                            break;
                        case "21-40":
                            if (IsOnSale == "0")
                                whereClause += " AND price BETWEEN 21 AND 40";
                            else
                                whereClause += " AND saleprice BETWEEN 21 AND 40";
                            break;
                        case "41-60":
                            if (IsOnSale == "0")
                                whereClause += " AND price BETWEEN 41 AND 60";
                            else
                                whereClause += " AND saleprice BETWEEN 41 AND 60";
                            break;
                        case "61-80":
                            if (IsOnSale == "0")
                                whereClause += " AND price BETWEEN 61 AND 80";
                            else
                                whereClause += " AND saleprice BETWEEN 61 AND 80";

                            break;
                        default:
                            break;
                    }
                }

                // Apply sorting based on sortOrder parameter
                string orderByClause = string.Empty;
                switch (sortOrder)
                {
                    case "price_asc":
                        orderByClause = " ORDER BY price ASC";
                        break;
                    case "price_desc":
                        orderByClause = " ORDER BY price DESC";
                        break;
                    case "name_asc":
                        orderByClause = " ORDER BY Name ASC";
                        break;
                    case "name_desc":
                        orderByClause = " ORDER BY Name DESC";
                        break;
                    default:
                        break; // No specific sorting
                }

                query += whereClause + orderByClause;

                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.AddRange(parameters.ToArray());

                connection.Open();
                SqlDataReader reader = command.ExecuteReader();

                while (reader.Read())
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

                    products.Add(product);
                }

                reader.Close();
            }

            return View(products);
        }

        public IActionResult manageProducts(string sortOrder, string category, string IsOnSale)
        {
            List<Product> products = new List<Product>();
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                string query = "SELECT Id, Name, price, stock, salePrice, imageUrl, category, IsOnSale FROM Products";

                // Build the WHERE clause
                string whereClause = string.Empty;
                if (!string.IsNullOrEmpty(category))
                {
                    whereClause = " WHERE category = @Category";
                }

                // Apply IsOnSale filter if specified
                if (!string.IsNullOrEmpty(IsOnSale))
                {
                    if (string.IsNullOrEmpty(whereClause))
                    {
                        whereClause = " WHERE IsOnSale = @IsOnSale";
                    }
                    else
                    {
                        whereClause += " AND IsOnSale = @IsOnSale";
                    }
                }

                // Apply sorting based on sortOrder parameter
                string orderByClause = string.Empty;
                switch (sortOrder)
                {
                    case "price_asc":
                        orderByClause = " ORDER BY price ASC";
                        break;
                    case "price_desc":
                        orderByClause = " ORDER BY price DESC";
                        break;
                    case "name_asc":
                        orderByClause = " ORDER BY Name ASC";
                        break;
                    case "name_desc":
                        orderByClause = " ORDER BY Name DESC";
                        break;
                    default:
                        break; // No specific sorting
                }

                query += whereClause + orderByClause;

                SqlCommand command = new SqlCommand(query, connection);

                // Add parameters for category and IsOnSale if specified
                if (!string.IsNullOrEmpty(category))
                {
                    command.Parameters.AddWithValue("@Category", category);
                }

                if (!string.IsNullOrEmpty(IsOnSale))
                {
                    // Assuming IsOnSale is boolean in the database, convert IsOnSale string to boolean
                    bool isOnSaleValue = IsOnSale.Equals("true", StringComparison.OrdinalIgnoreCase);
                    command.Parameters.AddWithValue("@IsOnSale", isOnSaleValue);
                }

                connection.Open();
                SqlDataReader reader = command.ExecuteReader();

                while (reader.Read())
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

                    products.Add(product);
                }

                reader.Close();
                connection.Close();
            }
            return View(products);
        }


        public IActionResult showProductEdit(int id)
        {
            Product product = GetProductById(id);
            if (product != null)
            {
                return View("Edit", product);
            }
            return RedirectToAction("manageProducts");
        }

        public IActionResult UpdateProduct(Product updatedProduct)
        {
            if (ModelState.IsValid)
            {
                if (updatedProduct == null)
                {


                    Console.WriteLine("1234567890");

                }

                if (Update(updatedProduct)) // Assuming UpdateProduct is a static method in Product class
                {

                    List<Notification> notifications = GetNotifications(updatedProduct.Id);
                    UpdateNotifications(notifications);

                    Console.WriteLine("The product with id = " + updatedProduct.Id + " has been updated.");

                    // Optionally, perform additional actions after successful update
                    return RedirectToAction("manageProducts"); // Redirect to home page or another action
                }
                else
                {
                    ViewBag.ErrorMessage = "Unable to update product, please try again later.";
                    return View("Edit", updatedProduct); // Return to edit view with error message
                }
            }
            else
            {
                return View("Edit", updatedProduct); // Return to edit view with validation errors
            }
        }
        /////////////////////////////////////////////////////////////ANFAL//////////////////////////////////

        // Method to retrieve notifications for a specific customer
        public List<Notification> GetNotifications(int productId)
        {
            List<Notification> notifications = new List<Notification>();

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                string query = "SELECT Id, productId,Message, IsRead, CustomerId FROM Notifications  WHERE productId = @productId";

                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@productId", productId);

                connection.Open();
                SqlDataReader reader = command.ExecuteReader();

                while (reader.Read())
                {
                    Notification notification = new Notification
                    {
                        Id = (int)reader["Id"],
                        productId = reader["productId"].ToString(),
                        Message = reader["Message"].ToString(),
                        IsRead = (bool)reader["IsRead"],
                        CustomerId = reader["CustomerId"].ToString()
                    };

                    notifications.Add(notification);
                }

                reader.Close();
            }

            return notifications; // Return the notifications to a view
        }

        // Method to update the IsRead property of notifications
        public void UpdateNotifications(List<Notification> notificationIds)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                foreach (Notification notificationId in notificationIds)
                {
                    string query = "UPDATE Notifications SET IsRead = 1 WHERE Id = @NotificationId";
                    SqlCommand command = new SqlCommand(query, connection);
                    command.Parameters.AddWithValue("@NotificationId", notificationId.Id) ;
                    command.ExecuteNonQuery();
                }
            }

        }


        [HttpPost]
        public IActionResult SetNotifications(string   productId, string CustomerId)
        {
            // Create a new Notification object
            var notification = new Notification
            {
                productId = productId,
                Message = $"The product {productId} is available.",
                IsRead = false,
                CustomerId = CustomerId // Assuming CustomerId is passed from the form
            };

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open(); // Open the connection

                string getMaxIdQuery = "SELECT MAX(Id) FROM Notifications";
                SqlCommand getMaxIdCommand = new SqlCommand(getMaxIdQuery, connection);
                int currentMaxId = Convert.ToInt32(getMaxIdCommand.ExecuteScalar());

                // Increment the current maximum Id by one to generate the Id for the new notification
                int newNotificationId = currentMaxId + 1;

                // Insert the new notification with the generated Id
                string insertQuery = "INSERT INTO Notifications (Id, productId, Message, IsRead, CustomerId) VALUES (@Id, @productId, @Message, @IsRead, @CustomerId)";
                SqlCommand insertCommand = new SqlCommand(insertQuery, connection);
                insertCommand.Parameters.AddWithValue("@Id", newNotificationId);
                insertCommand.Parameters.AddWithValue("@productId", productId);
                insertCommand.Parameters.AddWithValue("@Message", $"The product {productId} is available.");
                insertCommand.Parameters.AddWithValue("@IsRead", false); // Assuming the notification is initially unread
                insertCommand.Parameters.AddWithValue("@CustomerId", CustomerId);
                insertCommand.ExecuteNonQuery();

                connection.Close(); // Close the connection
            }

            return RedirectToAction("NotificationConfirmation");
        }
        public IActionResult NotificationConfirmation()
        {
        
            return View("Anfal");

        }

        public IActionResult NotificationsPage()
        {
            // Get the current customer ID from the session
            string  currentCustomerId = HttpContext.Session.GetString("CustomerID");

            // Retrieve notifications for the current customer where IsRead = 1
            List<Notification> notifications = GetNotificationsByCustomerId(currentCustomerId);

            // Pass filtered notifications to the view
            return View("Notificationspages", notifications);
        }
        public List<Notification> GetNotificationsByCustomerId(string customerId)
        {
            List<Notification> notifications = new List<Notification>();

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                string query = "SELECT Id, productId, Message, IsRead, CustomerId FROM Notifications WHERE CustomerId = @CustomerId AND IsRead = 1";
                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@CustomerId", customerId);

                connection.Open();
                SqlDataReader reader = command.ExecuteReader();

                while (reader.Read())
                {
                    Notification notification = new Notification
                    {
                        Id = (int)reader["Id"],
                        productId = reader["productId"].ToString(),
                        Message = reader["Message"].ToString(),
                        IsRead = (bool)reader["IsRead"],
                        CustomerId = reader["CustomerId"].ToString()
                    };

                    notifications.Add(notification);
                }

                reader.Close();
            }

            return notifications;
        }
    
    /////////////////////////////////////////////////////////////ANFAL//////////////////////////////////
    private bool Update(Product product)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    string query = @"UPDATE Products SET Name = @name, Price = @price, Stock = @stock, 
                             SalePrice = @salePrice, ImageUrl = @imageUrl, Category = @category, 
                             IsOnSale = @isOnSale WHERE Id = @id";

                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@name", product.Name);
                        command.Parameters.AddWithValue("@price", product.price); // Corrected property name
                        command.Parameters.AddWithValue("@stock", product.stock); // Corrected property name
                        command.Parameters.AddWithValue("@salePrice", product.salePrice); // Corrected property name
                        command.Parameters.AddWithValue("@imageUrl", product.imageUrl);
                        command.Parameters.AddWithValue("@category", product.category);
                        command.Parameters.AddWithValue("@isOnSale", product.IsOnSale);
                        command.Parameters.AddWithValue("@id", product.Id);

                        int rowsAffected = command.ExecuteNonQuery();
                        return rowsAffected > 0; // true if rows were updated, false otherwise
                    }
                }
            }
            catch (Exception ex)
            {
                // Handle the exception (log, rethrow, etc.)
                Console.WriteLine("Error updating product: " + ex.Message);
                return false; // Update failed
            }
        }







        //GET: Product/Delete/5
        public IActionResult Delete(int id)
        {
            var product = GetProductById(id);
            if (product == null)
            {
                return NotFound(); // Product not found
            }

            return View(product);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int id)
        {
            if (DeleteProduct(id)) // delete product from database 
            {
                Console.WriteLine("The product with id = " + id + " has been deleted from the website.");
                return RedirectToAction("ManageProducts");
            }
            else
            {
                ViewBag.ErrorMessage = "Unable to remove product, try again later.";
                return RedirectToAction("ManageProducts");
            }
        }


        private bool DeleteProduct(int id)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                string query = "DELETE FROM Products WHERE Id = @id;";

                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@id", id);

                    int rowsAffected = command.ExecuteNonQuery();
                    return rowsAffected > 0; // Returns true if rows were affected (product deleted)
                }
            }
        }
        //private void UpdateNotificationMessage(Product updatedProduct)
        //{
        //    // Retrieve the dictionary of product notification messages from session
        //    Dictionary<int, string> productNotifications = HttpContext.Session.GetObject<Dictionary<int, string>>("ProductNotifications");

        //    if (productNotifications != null)
        //    {
        //        // Check if the product's ID exists in the dictionary
        //        if (productNotifications.ContainsKey(updatedProduct.Id))
        //        {
        //            // Update the notification message for the product
        //            productNotifications[updatedProduct.Id] = $"The product '{updatedProduct.Name}' is now available for sale.";
        //        }

        //        // Save the updated dictionary back to session
        //        HttpContext.Session.SetObject("ProductNotifications", productNotifications);
        //    }
        //}



        //////////////////////////////////notify///////////////////////
        ///
        [HttpPost]
        public IActionResult Notify(int productId, string productName)
        {
            // Call your update stock function passing the productId
            //CreateNotification(productId);

            // Optionally, you can use the additional information like productName for further processing
            // For example, you might want to log the product name that the user requested notification for

            // Redirect the user to a different page after the stock is updated or any other action you want
            return RedirectToAction("Index");
        }














        //public IActionResult EditStock(int id)
        //{
        //    int flag = 0;
        //    Product product = GetProductById(id);

        //    if (product != null)
        //    {
        //        if (product.stock == 0)
        //        {
        //            flag = 1;
        //        }
        //        return View("EditStock", product);
        //    }
        //    return RedirectToAction("manageProducts");
        //}


        //// Method to update the stock of the product
        //public bool UpdateStock(int newStock)
        //{
        //    try
        //    {
        //        using (SqlConnection connection = new SqlConnection(connectionString))
        //        {
        //            connection.Open();
        //            string query = @"UPDATE Products SET Stock = @newStock WHERE Id = @id";

        //            using (SqlCommand command = new SqlCommand(query, connection))
        //            {
        //                command.Parameters.AddWithValue("@newStock", newStock);

        //                int rowsAffected = command.ExecuteNonQuery();
        //                return rowsAffected > 0; // Return true if the update was successful
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        // Handle the exception (log, rethrow, etc.)
        //        Console.WriteLine("Error updating stock: " + ex.Message);
        //        return false; // Update failed
        //    }
      //}
    }

}







