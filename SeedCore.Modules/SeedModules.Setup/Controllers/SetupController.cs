using System.Net.Mime;
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
            return this.Spa("index.html");
        }

        [HttpGet]
        public IActionResult Text()
        {
            return Json(new { text = "aaaa" });
        }
    }
}