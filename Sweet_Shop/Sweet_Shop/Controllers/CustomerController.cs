﻿using Microsoft.AspNetCore.Mvc;
using System.Data.SqlClient;
using Sweet_Shop.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Http;


namespace Sweet_Shop.Controllers
{
    public class CustomerController : Controller
    {
        public IConfiguration _configuration;
        public string connectionString = "";
        public CustomerController(IConfiguration configuration)
        {
            _configuration = configuration;
            connectionString = _configuration.GetConnectionString("database");
        }
        public IActionResult Index()
        {
            return View();
        }

        [Route("")]
        public IActionResult MainPage()
        {
            return View("MainPage");
        }

        public IActionResult LogInForCustomer()
        {
            return View("LogInForCustomer", new CustomerModel());
        }
        [HttpPost]
        public IActionResult checkLogInForCustomer(CustomerModel customer)
        {

            // בצע שאילתת SQL כדי לבדוק האם ה-ID והסיסמה קיימים בבסיס הנתונים
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                string query = "SELECT CustomerID, FirstName, LastName, Email, Phone, CAddress FROM Customer WHERE CustomerID = @CustomerID AND Password = @Password";

                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@CustomerID", customer.CustomerID);
                command.Parameters.AddWithValue("@Password", customer.Password);

                SqlDataReader reader = command.ExecuteReader();

                if (reader.Read())
                {
                    // Data found, login is successful
                    // Populate the CustomerModel object with retrieved data
                    CustomerModel loggedInCustomer = new CustomerModel(
                        customerId: reader["CustomerID"].ToString(),
                        firstName: reader["FirstName"].ToString(),
                        lastName: reader["LastName"].ToString(),
                        email: reader["Email"].ToString(),
                        phone: reader["Phone"].ToString(),
                        cAddress: reader["CAddress"].ToString(),
                        password: customer.Password // You may want to set the password from the provided parameter
                    );

                    HttpContext.Session.Clear();

                    // Store authenticated customer ID and email in session
                    HttpContext.Session.SetString("CustomerID", reader["CustomerID"].ToString());
                    HttpContext.Session.SetString("CustomerEmail", reader["Email"].ToString());

                    return RedirectToAction("MainPage");
                }
                else
                {
                    // ID or password do not match, return to login page with error message
                    ViewBag.ErrorMessage = "Invalid ID or password";
                    return View("LogInForCustomer");
                }
            }
        
        }

            // ModelState אינו תקין, תחזור לדף ההתחברות עם הודעת שגיאה
          


       //[Route("")]
        public IActionResult CustomerRegistration()
        {
            return View("CustomerRegistration", new CustomerModel());
        }
        [HttpPost]
        public IActionResult cheackCustomerRegistration(CustomerModel customer)
        {
            if (ModelState.IsValid)
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    // Check if the CustomerID already exists in the database
                    string checkQuery = "SELECT COUNT(*) FROM Customer WHERE CustomerID = @customerId";
                    SqlCommand checkCommand = new SqlCommand(checkQuery, connection);
                    checkCommand.Parameters.AddWithValue("@customerId", customer.CustomerID);
                    int existingCustomerCount = (int)checkCommand.ExecuteScalar();

                    if (existingCustomerCount > 0)
                    {
                        // CustomerID already exists, return error message
                        TempData["ErrorMessage"] = "CustomerID already exists. Please choose a different one.";

                        return View("CustomerRegistration", customer);
                    }
                    string query = "INSERT INTO Customer (CustomerID, FirstName, LastName, Email, Phone, CAddress,Password) VALUES (@value1,@value2, @value3, @value4, @value5, @value6,@value7)";

                    SqlCommand command = new SqlCommand(query, connection);
                    command.Parameters.AddWithValue("@value1", customer.CustomerID);
                    command.Parameters.AddWithValue("@value2", customer.FirstName);
                    command.Parameters.AddWithValue("@value3", customer.LastName);
                    command.Parameters.AddWithValue("@value4", customer.Email); 
                    command.Parameters.AddWithValue("@value5", customer.Phone);
                    command.Parameters.AddWithValue("@value6", customer.CAddress);
                    command.Parameters.AddWithValue("@value7", customer.Password);


                    int rowsAffected = command.ExecuteNonQuery();

                    if (rowsAffected > 0)
                    {
                        // Insertion successful
                        return View("MainPage");
                    }
                    else
                    {
                        // Insertion failed
                        ViewBag.Message = "Failed to insert customer data.";
                        return View("CustomerRegistration", customer);
                    }
                }
            }
            return View("CustomerRegistration");
        }
    }
}    
    