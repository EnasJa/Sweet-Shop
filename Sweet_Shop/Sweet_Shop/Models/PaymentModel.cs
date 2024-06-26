﻿using System.ComponentModel.DataAnnotations;
using Sweet_Shop.Validation;


namespace Sweet_Shop.Models
{
    public class PaymentModel
    {
        public Cart Cart { get; set; }

        [Required(ErrorMessage = "Card Number is required")]
        [RegularExpression(@"^\d{16}$", ErrorMessage = "Please enter a valid 16-digit card number")]
        public string CardNumber { get; set; }

        [Required(ErrorMessage = "Expiry Date is required")]
        [RegularExpression(@"^(0[1-9]|1[0-2])\/[0-9]{4}$", ErrorMessage = "Please enter a valid expiry date (MM/YYYY)")]
        [ValidExpiryDate(ErrorMessage = "Expiry date must be in the future.")]
        public string ExpiryDate { get; set; }

        [Required(ErrorMessage = "CVC is required")]
        [RegularExpression(@"^\d{3}$", ErrorMessage = "Please enter a valid 3-digit CVC")]
        public string CVC { get; set; }

        [RegularExpression(@"^\d{9}$", ErrorMessage = "Please enter a valid 9-digit Cardholder ID")]
        [Required(ErrorMessage = "Cardholder Name is required")]
        public string CardholderID { get; set; }
        public PaymentModel()
        {
            CardNumber = "";
            ExpiryDate = "";
            CVC = "";
            CardholderID = "";
        }
        public PaymentModel(Cart cart,string cardNumber, string expiryDate, string cVC, string cardholderID)
        {
            this.Cart = cart;   
            CardNumber = cardNumber;
            ExpiryDate = expiryDate;
            CVC = cVC;
            CardholderID = cardholderID;
        }


        // Custom validation method to check if expiry date is in the future
        public bool IsExpiryDateValid()
        {
            if (DateTime.TryParseExact(ExpiryDate, "MM/yyyy", null, System.Globalization.DateTimeStyles.None, out DateTime expiryDateTime))
            {
                // Compare with current date
                return expiryDateTime > DateTime.Now;
            }
            return false;

        }

    }
}
