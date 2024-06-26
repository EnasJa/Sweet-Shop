﻿using System.ComponentModel.DataAnnotations;
using System.Net;
using System.Numerics;
using Sweet_Shop.Models;


namespace Sweet_Shop.Models
{
    public class CustomerModel
    {
       

        [Required(ErrorMessage = "Customer ID is required")]
        [RegularExpression(@"^\d{9}$", ErrorMessage = "Customer ID must contain exactly 9 digits")]
        public string CustomerID { get; set; }

        [Required(ErrorMessage = "First Name is required")]
        [RegularExpression(@"^[A-Z][a-zA-Z]*$", ErrorMessage = "First Name must start with a capital letter and contain only letters")]
        public string FirstName { get; set; }

        [Required(ErrorMessage = "Last Name is required")]
        [RegularExpression(@"^[A-Z][a-zA-Z]*$", ErrorMessage = "Last Name  must start with a capital letter and contain only letters")]
        public string LastName { get; set; }


        [Required(ErrorMessage = "Email is required")]
        [RegularExpression(@"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$", ErrorMessage = "Invalid Email Address")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Phone number is required")]
        [RegularExpression(@"^\d{10}$", ErrorMessage = "Phone number must contain exactly 10 digits")]
        public string Phone { get; set; }
        [Required(ErrorMessage = "Address is required")]
        [RegularExpression(@"^[A-Za-z\s\d.,'-]+$", ErrorMessage = "Invalid Address")]
        public string CAddress { get; set; }

        [Required(ErrorMessage = "Password is required")]
        [RegularExpression(@"^[a-zA-Z0-9]{8,}$", ErrorMessage = "Password must be at least 8 characters long and contain only letters and numbers")]
        public string Password { get; set; }

        public CustomerModel()
        {
            CustomerID = "";
            FirstName = "";
            LastName = "";
            Email = "";
            Phone = "";
            CAddress = "";
            Password = "";
        }


        // Parameterized constructor
        public CustomerModel(string customerId, string firstName, string lastName, string email, string phone, string cAddress,string password)
        {
            CustomerID = customerId;
            FirstName = firstName;
            LastName = lastName;
            Email = email;
            Phone = phone;
            CAddress = cAddress;
            Password = password;
        }
    }
}
