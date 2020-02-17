using SeedCore.SpaService;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class OrchardCoreBuilderExtensions
    {
        public static OrchardCoreBuilder AddSpa(this OrchardCoreBuilder builder)
        {
            return builder.RegisterStartup<Startup>();
        }
    }
}
