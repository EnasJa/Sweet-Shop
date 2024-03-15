using System.ComponentModel.DataAnnotations;

namespace Sweet_Shop.Models
{
    public class CustomerModel
    {
       
        [Required]
        public int CustomerID { get; set; }
        [Required]
        public string FirstName { get; set; }
        [Required]
        public string LastName { get; set; }
        [Required]
        public string Email { get; set; }
        [Required]
        public string Phone { get; set; }
        [Required]
        public string CAddress { get; set; }
        
        public CustomerModel()
        {
            CustomerID = 2565364;
            FirstName = "enas";
            LastName = "jaber";
            Email = "wejhdfj";
            Phone = "3546657";
            CAddress = "dgdh";
        }
    }
}
