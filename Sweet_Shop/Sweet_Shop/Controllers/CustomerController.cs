using Microsoft.AspNetCore.Mvc;
using System.Data.SqlClient;
using Sweet_Shop.Models;
using Microsoft.Extensions.Configuration;


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
        //[Route("")]
        public IActionResult MainPage()
        {
            return View("MainPage");
        }



        [Route("")]
        public IActionResult CustomerRegistration()
        {
          

           // CustomerModel customer = new CustomerModel(Request.Form["CustomerID"].ToString(), Request.Form["firstName"].ToString(), Request.Form["lastName"].ToString(), Request.Form["email"].ToString(), Request.Form["phone"].ToString(), Request.Form["address"].ToString());
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
                    string query = "INSERT INTO Customer (CustomerID, FirstName, LastName, Email, Phone, CAddress) VALUES (@value1,@value2, @value3, @value4, @value5, @value6)";

                    SqlCommand command = new SqlCommand(query, connection);
                    command.Parameters.AddWithValue("@value1", customer.CustomerID);
                    command.Parameters.AddWithValue("@value2", customer.FirstName);
                    command.Parameters.AddWithValue("@value3", customer.LastName);
                    command.Parameters.AddWithValue("@value4", customer.Email); 
                    command.Parameters.AddWithValue("@value5", customer.Phone);
                    command.Parameters.AddWithValue("@value6", customer.CAddress);
                

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
    // זהו דרישה חשובה כדי לאפשר נגישות לפעולה באמצעות HTTP POST
        //public IActionResult CustomerRegistration_r(CustomerModel customer)
        //{
        //    // Now you can proceed with your database insertion logic

        //    return RedirectToAction("cheackCustomerRegistration", customer);
        //}