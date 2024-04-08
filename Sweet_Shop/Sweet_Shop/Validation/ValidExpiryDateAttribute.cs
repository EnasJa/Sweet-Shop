using System;
using System.ComponentModel.DataAnnotations;

namespace Sweet_Shop.Validation
{
    public class ValidExpiryDateAttribute : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (value is string expiryDate)
            {
                if (DateTime.TryParseExact(expiryDate, "MM/yyyy", null, System.Globalization.DateTimeStyles.None, out DateTime expiryDateTime))
                {
                    if (expiryDateTime > DateTime.Now)
                    {
                        return ValidationResult.Success;
                    }
                }
            }
            return new ValidationResult("Expiry date must be in the future.");
        }
    }
}
