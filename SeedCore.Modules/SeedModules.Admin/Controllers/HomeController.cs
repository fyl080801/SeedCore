using Microsoft.AspNetCore.Mvc;

namespace SeedModules.Admin.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return Content("admin");
        }
    }
}