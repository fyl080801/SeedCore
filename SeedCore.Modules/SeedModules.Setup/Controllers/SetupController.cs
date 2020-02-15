using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Hosting;

namespace SeedModules.Setup.Controllers
{
    public class SetupController : Controller
    {
        private readonly IHostEnvironment _environment;

        public SetupController(IHostEnvironment environment)
        {
            _environment = environment;
        }

        public IActionResult Index()
        {
            return Content("aaa");
        }

        public IActionResult Text()
        {
            return Content("aaaasdsdsd");
        }
    }
}