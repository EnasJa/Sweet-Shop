using System.ComponentModel.DataAnnotations;
using System.Data.SqlClient;

namespace Sweet_Shop.Models
{
    public class Product
    {
        public int Id { get; set; }
        [MaxLength(255, ErrorMessage = "Name must be between 1 and 255 characters")]

        [Required(ErrorMessage = "Name is required")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Price is required")]
        [Range(0.01, double.MaxValue, ErrorMessage = "Price must be above 0")]
        public float price { get; set; }

        [Required(ErrorMessage = "Stock is required")]
        [Range(0, 1000, ErrorMessage = "Stock must be between 0 and 1000")]
        public int stock { get; set; }

        [Required(ErrorMessage = "Sale Price is required")]
        [Range(0.0, double.MaxValue, ErrorMessage = "Sale Price must be above 0")]
        public float salePrice { get; set; }

        [Required(ErrorMessage = "Image URL is required")]
        [MaxLength(500, ErrorMessage = "Image URL must be between 1 and 500 characters")]
        public string imageUrl { get; set; }


        [Required(ErrorMessage = "Category is required")]
        [RegularExpression("^([A-Za-z\\s]+(,\\s*|$))*$", ErrorMessage = "Category must only contain letters separated by commas")]
        //public string category { get; set; }

        [Display(Name = "Is On Sale")]
        public bool IsOnSale { get; set; }

        //private readonly List<string> allowedCategories = new List<string> { "Raw materials for baking ", "Icing and decorating cakes", "Cake packaging", "Baking Tools" };
        private readonly List<string> allowedCategories = new List<string> { "Raw materials for baking", "Icing and decorating cakes", "Cake packaging", "Baking Tools" };
        private string _category;
        [Required(ErrorMessage = "Category is required")]
        //[RegularExpression("^([A-Za-z\\s]+(,\\s*|$))*$", ErrorMessage = "Category must only contain letters separated by commas")]
        public string category
        {
            get { return _category; }
            set
            {
                //if (!allowedCategories.Contains(value))
                //{
                //    throw new ArgumentException($"Invalid category. Allowed categories are: {string.Join(", ", allowedCategories)}");
                //}
                _category = value;
            }
        }

        public int quantity { get; internal set; }

        public Product() // Default constructor
        {
        }

        public Product(int id, string name, float price, int stock, float salePrice, string imageUrl, string category, bool IsOnSale)
        {
            Id = id;
            Name = name;
            this.price = price;
            this.stock = stock;
            this.salePrice = salePrice;
            this.imageUrl = imageUrl;
            //this.category = category;
            this.IsOnSale = IsOnSale;
            if (!allowedCategories.Contains(category))
            {
                throw new ArgumentException($"Invalid category. Allowed categories are: {string.Join(", ", allowedCategories)}");
            }
            category = category;
        }

        public float GetFinalPrice()
        {
            if (IsOnSale)
            {
                return salePrice;
            }
            return price;
        }

       
    }
}
