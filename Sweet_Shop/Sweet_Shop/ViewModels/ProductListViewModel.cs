using System.Collections.Generic;
using Sweet_Shop.Models;

namespace Sweet_Shop.ViewModels
{
    public class ProductListViewModel
    {
        public IEnumerable<Product> Products { get; set; }
        // Add other properties as needed, e.g., for search, filtering, pagination

    }
}
