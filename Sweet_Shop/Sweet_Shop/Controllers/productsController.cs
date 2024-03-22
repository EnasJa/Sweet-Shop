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
                            return RedirectToAction("viewproducts");
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






        //public IActionResult ViewProductsByCategory(string category)
        //{
        //    List<Product> products = GetProductsByCategory(category);
        //    ViewBag.Category = category;
        //    return View(products);
        //}

        //private List<Product> GetProductsByCategory(string category)
        //{
        //    List<Product> products = new List<Product>();

        //    using (SqlConnection connection = new SqlConnection(connectionString))
        //    {
        //        string query = "SELECT * FROM Products";

        //        if (!string.IsNullOrEmpty(category))
        //        {
        //            query += " WHERE category = @Category";
        //        }

        //        SqlCommand command = new SqlCommand(query, connection);

        //        if (!string.IsNullOrEmpty(category))
        //        {
        //            command.Parameters.AddWithValue("@Category", category);
        //        }

        //        connection.Open();
        //        SqlDataReader reader = command.ExecuteReader();

        //        while (reader.Read())
        //        {
        //            int id = reader.GetInt32(reader.GetOrdinal("Id"));
        //            string name = reader.GetString(reader.GetOrdinal("Name"));
        //            double price = reader.GetDouble(reader.GetOrdinal("Price"));
        //            int stock = reader.GetInt32(reader.GetOrdinal("Stock"));
        //            double salePrice = reader.GetDouble(reader.GetOrdinal("SalePrice"));
        //            string imageUrl = reader.GetString(reader.GetOrdinal("ImageUrl"));
        //            string productCategory = reader.GetString(reader.GetOrdinal("Category"));
        //            bool isOnSale = reader.GetBoolean(reader.GetOrdinal("IsOnSale"));

        //            Product product = new Product
        //            {
        //                Id = id,
        //                Name = name,
        //                price = (float)price,
        //                stock = stock,
        //                salePrice = (float)salePrice,
        //                imageUrl = imageUrl,
        //                category = productCategory,
        //                IsOnSale = isOnSale
        //            };

        //            products.Add(product);
        //        }

        //        reader.Close();
        //        connection.Close();
        //    }

        //    return products;
        //}








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



        //public IActionResult UpdateProduct(int id, Product updatedProduct)
        //{
        //    if (id != updatedProduct.Id)
        //    {
        //        return BadRequest("ID mismatch between URL and product data.");
        //    }

        //    // Check if the product exists
        //    Product existingProduct = GetProductById(id);
        //    if (existingProduct == null)
        //    {
        //        return NotFound();
        //    }

        //    using (SqlConnection connection = new SqlConnection(connectionString))
        //    {
        //        connection.Open();

        //        string query = "UPDATE Products SET price = @Price, salePrice = @SalePrice, IsOnSale = @IsOnSale, stock = @Stock WHERE Id = @Id";

        //        using (SqlCommand command = new SqlCommand(query, connection))
        //        {
        //            command.Parameters.AddWithValue("@Price", updatedProduct.price);
        //            command.Parameters.AddWithValue("@SalePrice", updatedProduct.salePrice);
        //            command.Parameters.AddWithValue("@IsOnSale", updatedProduct.IsOnSale);
        //            command.Parameters.AddWithValue("@Stock", updatedProduct.stock);
        //            command.Parameters.AddWithValue("@Id", id);

        //            int rowsAffected = command.ExecuteNonQuery();

        //            if (rowsAffected == 0)
        //            {
        //                return NotFound();
        //            }
        //        }
        //    }

        //    return RedirectToAction(nameof(Index));
        //}


        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public IActionResult UpdateProduct(int id)
        //{
        //    Product product = GetProductById(id);

        //    using (SqlConnection connection = new SqlConnection(connectionString))
        //    {
        //        connection.Open();

        //        string query = "UPDATE Products SET price = @Price, salePrice = @SalePrice, IsOnSale = @IsOnSale, stock = @Stock WHERE Id = @Id";

        //        using (SqlCommand command = new SqlCommand(query, connection))
        //        {
        //            command.Parameters.AddWithValue("@Price", product.price);
        //            command.Parameters.AddWithValue("@SalePrice", product.salePrice);
        //            command.Parameters.AddWithValue("@IsOnSale", product.IsOnSale);
        //            command.Parameters.AddWithValue("@Stock", product.stock);
        //            command.Parameters.AddWithValue("@Id", id);

        //            int rowsAffected = command.ExecuteNonQuery();

        //            if (rowsAffected == 0)
        //            {
        //                return NotFound();
        //            }
        //        }
        //    }
        //    return View();  
        //    //return RedirectToAction(nameof(Index));
        //}


        public IActionResult UpdateProduct(int id, Product updatedProduct)
        {
            if (id != updatedProduct.Id)
            {
                return BadRequest(); // Id mismatch, return bad request
            }


            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                string query = "UPDATE Products SET price = @Price, salePrice = @SalePrice, IsOnSale = @IsOnSale, stock = @Stock WHERE Id = @Id";

                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Price", updatedProduct.price);
                    command.Parameters.AddWithValue("@SalePrice", updatedProduct.salePrice);
                    command.Parameters.AddWithValue("@IsOnSale", updatedProduct.IsOnSale);
                    command.Parameters.AddWithValue("@Stock", updatedProduct.stock);
                    command.Parameters.AddWithValue("@Id", id);

                    int rowsAffected = command.ExecuteNonQuery();

                    if (rowsAffected == 0)
                    {
                        return NotFound(); // No rows updated, product not found
                    }
                }
            }

            // Redirect to a view or action method indicating success
            return View();
        }

    }


}





