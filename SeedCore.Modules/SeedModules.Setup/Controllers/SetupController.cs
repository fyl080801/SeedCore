using System.Collections.Generic;
using System.Linq;
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

        public SetupController(
            ISetupService setupService,
            ShellSettings shellSettings)
        {
            _setupService = setupService;
            _shellSettings = shellSettings;
        }

        [AppendAntiforgery]
        public IActionResult Index(string token)
        {
            return this.Spa("index.html");
        }

        [HttpPost]
        public async Task<IActionResult> Execute([FromBody]SetupModel model)
        {
            var recipes = await _setupService.GetSetupRecipesAsync();

            var setupContext = new SetupContext()
            {
                AdminEmail = model.Email,
                AdminPassword = model.Password,
                AdminUsername = model.UserName,
                SiteName = model.SiteName,
                SiteTimeZone = model.SiteTimeZone,
                Recipe = recipes.FirstOrDefault(e => e.Name == model.RecipeName),
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

            var result = await _setupService.SetupAsync(setupContext);

            return Json(new { result });

            // return await Task.FromResult(Json(new { text = "success" }));
        }
    }
}