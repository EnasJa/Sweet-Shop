using Microsoft.AspNetCore.Mvc;
using Sweet_Shop.Models;

namespace Sweet_Shop.Controllers
{
    public class ShoppingCartController : Controller
    {
       

        public IActionResult Index()
        {
            return View();
        }

 
        public IActionResult CartPage()
        {
            
            return View("CartPage"); 
        }

    
       
    }
}
