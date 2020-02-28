using Microsoft.AspNetCore.Mvc;

namespace SeedModules.AdminLogin.Controllers
{
    public class AdminController : Controller
    {
        public IActionResult Login()
        {
            return this.Spa("index.html");
        }
    }
}