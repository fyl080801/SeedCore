using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using OrchardCore.Environment.Shell;
using OrchardCore.Setup.Services;
using SeedCore.Data;
using SeedModules.Setup.Models;

namespace SeedModules.Setup.Controllers
{
    public class SetupController : Controller
    {
        private readonly ISetupService _setupService;
        private readonly ShellSettings _shellSettings;
        private readonly IEnumerable<DatabaseProvider> _databaseProviders;

        public SetupController(
            ISetupService setupService,
            ShellSettings shellSettings,
            IEnumerable<DatabaseProvider> databaseProviders)
        {
            _setupService = setupService;
            _shellSettings = shellSettings;
            _databaseProviders = databaseProviders;
        }

        [AppendAntiforgery]
        public IActionResult Index(string token)
        {
            return this.Spa("index.html");
        }

        [HttpGet]
        public IActionResult DatabaseProviders()
        {
            return Json(_databaseProviders);
        }

        [HttpPost]
        public async Task<IActionResult> Execute([FromBody]SetupModel model)
        {
            model.DatabaseProviders = _databaseProviders;

            var setupContext = new SetupContext()
            {
                AdminEmail = "",
                AdminPassword = "",
                AdminUsername = "",
                DatabaseConnectionString = "",
                DatabaseProvider = "",
                DatabaseTablePrefix = "",
                SiteName = "",
                SiteTimeZone = "",
                Recipe = null,
                EnabledFeatures = null,
                ShellSettings = _shellSettings,
                Errors = new Dictionary<string, string>()
            };

            if (!string.IsNullOrEmpty(_shellSettings["ConnectionString"]))
            {
                setupContext.DatabaseConnectionString = _shellSettings["ConnectionString"];
                setupContext.DatabaseProvider = _shellSettings["DatabaseProvider"];
                setupContext.DatabaseTablePrefix = _shellSettings["TablePrefix"];
            }
            else
            {
                setupContext.DatabaseConnectionString = model.ConnectionString;
                setupContext.DatabaseProvider = model.DatabaseProvider;
                setupContext.DatabaseTablePrefix = model.TablePrefix;
            }

            // var result = await _setupService.SetupAsync(setupContext);

            // return Json(new { result });

            return await Task.FromResult(Json(new { text = "success" }));
        }
    }
}