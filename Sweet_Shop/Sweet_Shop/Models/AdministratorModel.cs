using System.ComponentModel.DataAnnotations;

namespace Sweet_Shop.Models
{
    public class AdministratorModle
    {
        [Required(ErrorMessage = "Admin ID is required")]
        public string AdminID { get; set; }

        [Required(ErrorMessage = "Password is required")]
        public string Password { get; set; }
        public AdministratorModle()
        {
            AdminID = "";
            Password = "";
        }
        public AdministratorModle(string adminID, string password)
        {
            AdminID = adminID;
            Password = password;
        }
        public bool IsAdmin
        {
            get
            {
                // Example logic: check if AdminID starts with "admin_" to determine admin status
                return AdminID.StartsWith("admin_");
            }
        }
    }
}
