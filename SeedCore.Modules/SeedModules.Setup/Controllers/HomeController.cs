using Microsoft.AspNetCore.Mvc;

namespace SeedModules.Setup.Controllers
{
    public class HomeController : Controller
    {
        [AppendAntiforgeryToken]
        public IActionResult Index()
        {
            return View();
        }
    }
}