using Microsoft.AspNetCore.Mvc;

namespace SeedModules.Controllers
{
    public class AccountController : Controller
    {
        public IActionResult Login()
        {
            return Content("login");
        }
    }
}