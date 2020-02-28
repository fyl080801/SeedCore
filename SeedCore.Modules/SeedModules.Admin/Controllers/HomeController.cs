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

        // [Authorize]
        public IActionResult Index()
        {
            // var table = _dbcontext.Set<TestTable>();
            // table.Add(new TestTable() { Name = "aaa" });
            // _dbcontext.SaveChanges();
            return Content("admin");
        }
    }
}