using Microsoft.AspNetCore.Mvc;
using Project.Controllers;
using Sweet_Shop.Models;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using Microsoft.Extensions.Configuration;

namespace Sweet_Shop.Controllers
{
    public class PaymentController : Controller
    {
        private readonly IConfiguration _configuration;
        public string connectionString { get; set; }


        public PaymentController(IConfiguration configuration)
        {
            _configuration = configuration;
            connectionString = _configuration.GetConnectionString("database");
        }
        public IActionResult Index()
        {
            return View();
        }
        public IActionResult PaymentPage()
        {
            return View("PaymentPage", new PaymentModel());

        }

        [HttpPost]
        public IActionResult cheackPayment(PaymentModel payment)
        {
            // Check if the model state is valid
            if (!ModelState.IsValid)
            {
                // Model state is not valid, return the view with validation errors
                return View("PaymentPage");
            }
            return RedirectToAction("MainPage", "Customer"); // Redirect to a success page
        }
        // productsController.cs

        //[HttpPost]
        //public IActionResult PaymentProcess([FromBody] List<Product> cartItems)
        //{
        //    try
        //    {
        //        // Iterate through cart items and update stock in the database
        //        foreach (var item in cartItems)
        //        {
        //            // Get the product ID and quantity from the cart item
        //            int productId = item.Id; // Assuming the product ID is stored in the 'Id' property
        //            int quantity = item.quantity; // Assuming the quantity is stored in the 'Quantity' property

        //            // Retrieve product from database
        //            Product product =GetProductById(productId);

        //            if (product != null)
        //            {
        //                // Update stock in the database
        //                int newStock = product.stock - quantity;
        //                if (newStock >= 0)
        //                {
        //                    UpdateStock(productId, newStock);
        //                }
        //                else
        //                {
        //                    // Handle insufficient stock error (e.g., notify user)
        //                    return BadRequest("Insufficient stock for one or more items.");
        //                }
        //            }
        //            else
        //            {
        //                // Handle product not found error
        //                return NotFound("One or more products not found.");
        //            }
        //        }

        //        // Clear cart items from localStorage after successful update
        //        // Note: This should be handled on the client-side after receiving a successful response from the server
        //        // localStorage.removeItem("cartItems");

        //        // Payment process successful
        //        return Ok();
        //    }
        //    catch (Exception ex)
        //    {
        //        // Handle exceptions (e.g., log error)
        //        return StatusCode(500, "An error occurred during payment process.");
        //    }
        //}

        //// Method to update the stock of the product in the database
        //private void UpdateStock(int productId, int newStock)
        //{
        //    // Implement the logic to update stock in the database
        //    // Example:
        //    using (SqlConnection connection = new SqlConnection(connectionString))
        //    {
        //        connection.Open();
        //        string query = "UPDATE Products SET Stock = @newStock WHERE Id = @productId";
        //        SqlCommand command = new SqlCommand(query, connection);
        //        command.Parameters.AddWithValue("@newStock", newStock);
        //        command.Parameters.AddWithValue("@productId", productId);
        //        command.ExecuteNonQuery();
        //    }
        //}
        //public Product GetProductById(int id)
        //{
        //    // Example logic to retrieve product from database
        //    using (SqlConnection connection = new SqlConnection(connectionString))
        //    {
        //        string query = "SELECT Id, Name, price, stock, salePrice, imageUrl, category, IsOnSale FROM Products WHERE Id = @Id";

        //        SqlCommand command = new SqlCommand(query, connection);
        //        command.Parameters.AddWithValue("@Id", id);
        //        connection.Open();
        //        SqlDataReader reader = command.ExecuteReader();

        //        if (reader.Read())
        //        {
        //            Product product = new Product
        //            {
        //                Id = Convert.ToInt32(reader["Id"]),
        //                Name = reader["Name"].ToString(),
        //                price = Convert.ToSingle(reader["price"]),
        //                stock = Convert.ToInt32(reader["stock"]),
        //                salePrice = Convert.ToSingle(reader["salePrice"]),
        //                imageUrl = reader["imageUrl"].ToString(),
        //                category = reader["category"].ToString(),
        //                IsOnSale = Convert.ToBoolean(reader["IsOnSale"])
        //            };
        //            reader.Close();
        //            connection.Close();
        //            return product;
        //        }

        //        reader.Close();
        //        connection.Close();
        //        return null; // Product not found
        //    }
        //}
        public IActionResult CartPage()
        {

            return View("CartPage");
        }
    }


}
