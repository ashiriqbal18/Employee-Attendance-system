using final.Models;
using final.Repository;
using Microsoft.AspNetCore.Mvc;
using final.helper;

namespace final.Controllers
{
    public class LoginController : Controller
    {
        private readonly IEmployeeRepository _employeeRepository;

        public LoginController(IEmployeeRepository employeeRepository)
        {
            _employeeRepository = employeeRepository;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public JsonResult Login(string email, string password)
        {
            var user = _employeeRepository.GetByEmail(email);

            if (user != null)
            {
                string decryptedPassword = EncryptionHelper.Decrypt(user.Password);
                if (decryptedPassword == password)
                {
                    // 🔐 Session (for MVC)
                    HttpContext.Session.SetInt32("UserId", user.Id);
                    HttpContext.Session.SetString("Role", user.Role);
                    HttpContext.Session.SetString("Name", user.Name);

                    // 🔐 JWT (for API)
                    var token = JwtHelper.GenerateToken(user.Id.ToString(), user.Role, this.HttpContext.RequestServices.GetRequiredService<IConfiguration>());
                    
                    string redirectUrl = "";
                    
                    if (user.Role == "Admin")
                    {
                        redirectUrl = Url.Action("Index", "Employee");
                    }
                    else if (user.Role == "Manager")
                    {
                        redirectUrl = Url.Action("Team", "Employee");
                    }
                    else if (user.Role == "Employee")
                    {
                        redirectUrl = Url.Action("Profile", "Employee");
                    }
                    
                    return Json(new
                    {
                        success = true,
                        message = "Login successful",
                        redirectUrl = redirectUrl
                    });
                }
            }

            return Json(new { success = false, message = "Invalid login credentials" });
        }


        [HttpPost]
        public JsonResult Logout()
        {
            HttpContext.Session.Clear();

            return Json(new
            {
                success = true,
                message = "Logged out successfully",
                redirectUrl = Url.Action("Index", "Login")
            });
        }
        [HttpGet]
        public IActionResult Login()
        {
            return RedirectToAction("Index");
        }
             

    }
}

