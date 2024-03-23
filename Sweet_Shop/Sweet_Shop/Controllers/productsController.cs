﻿using Microsoft.AspNetCore.Mvc;
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

        public IActionResult AddProduct(Product product)
        {
            if (ModelState.IsValid)
            {
                try
                {
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
        private Product GetProductById(int id)
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

        public IActionResult ViewProducts(string sortOrder, string category, string IsOnSale)
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






    }
}





