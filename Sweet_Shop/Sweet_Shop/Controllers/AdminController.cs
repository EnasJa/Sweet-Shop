using Microsoft.AspNetCore.Mvc;
using Sweet_Shop.Models;

namespace Sweet_Shop.Controllers
{
    public class AdminController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
        public IActionResult AdministratorLogin()
        {
            return View("administratorLogin", new AdministratorModle());

        }
        [HttpPost]

        public IActionResult AdministratorLogin(AdministratorModle administrator)
        {
            if (ModelState.IsValid)
            {

                if (administrator.AdminID == "0011" && administrator.Password == "0011")
                {
                    HttpContext.Session.SetString("AdminID", administrator.AdminID);
                    // הנתונים נכונים, ננתב לדף הראשי של המנהל
                    return RedirectToAction("AdminDashboard");
                }
                else
                {
                    // הנתונים שגויים, נחזיר לדף ההתחברות עם הודעת שגיאה
                    ModelState.AddModelError(string.Empty, "Invalid Admin ID or Password");
                    return View("administratorLogin", administrator);
                }

            }
            return View("administratorLogin");


        }
        public IActionResult AdminDashboard()
        {
            return View("AdminDashboard");

        }
        public IActionResult LogOut()
        {
            // Clear all session data
            HttpContext.Session.Clear();

            // Redirect to the login page
            return RedirectToAction("AdministratorLogin");
        }
    }
}
