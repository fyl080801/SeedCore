using OrchardCore.Modules;
using Microsoft.Extensions.DependencyInjection;
using OrchardCore.Scripting;
using SeedModules.Scripting.Providers;
using OrchardCore.Scripting.JavaScript;

namespace SeedModules.Scripting
{
    public class Startup : StartupBase
    {
        public override void ConfigureServices(IServiceCollection services)
        {
            services.AddScripting();
            services.AddJavaScriptEngine();
            services.AddSingleton<IGlobalMethodProvider, LogProvider>();
        }
    }
}
