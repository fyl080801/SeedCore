using Microsoft.Extensions.DependencyInjection;
using OrchardCore.Modules;

namespace SeedCore.SpaService
{
    public class Startup : StartupBase
    {
        public override int Order => -1024;
        public override int ConfigureOrder => 1024;

        public override void ConfigureServices(IServiceCollection services)
        {

        }
    }
}