using Microsoft.AspNetCore.Mvc;

namespace SeedModules.Setup.Controllers
{
    public class SetupController : Controller
    {
        public IActionResult Index()
        {
            return Content("aaaa");
        }
    }
}