using Microsoft.AspNetCore.Mvc;
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
            connectionString = _configuration.GetConnectionString("connectString");
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
                    string query = "SELECT COUNT(*) FROM Customer WHERE CustomerID = @CustomerID AND Password = @Password";

                    SqlCommand command = new SqlCommand(query, connection);
                    command.Parameters.AddWithValue("@CustomerID", customer.CustomerID);
                    command.Parameters.AddWithValue("@Password", customer.Password);

                    int count = (int)command.ExecuteScalar();

                    if (count > 0)
                    {
                        // ה-ID והסיסמה תואמים, אז הכניסה מוצלחת
                        // שמור כתובת אימייל ב-Session
                       

                        return RedirectToAction("MainPage");
                    }
                    else
                    {
                        // ה-ID או הסיסמה לא תואמים, תחזור לדף ההתחברות עם הודעת שגיאה
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
    