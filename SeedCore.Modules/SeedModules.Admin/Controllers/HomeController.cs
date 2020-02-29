using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SeedCore.Data;

namespace SeedModules.Admin.Controllers
{
    public class HomeController : Controller
    {
        private readonly IDbContext _dbcontext;

        public HomeController(IDbContext dbcontext)
        {
            _dbcontext = dbcontext;
        }

        [AppendAntiforgeryToken]
        [Authorize]
        public IActionResult Index()
        {
            return this.Spa("index.html");
        }
    }
}