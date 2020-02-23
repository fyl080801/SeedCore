namespace Microsoft.Extensions.DependencyInjection
{
    public static class ServiceCollectionExtensions
    {
        public static OrchardCoreBuilder AddSeedCore(this IServiceCollection services)
        {
            var builder = services.AddOrchardCore();

            return builder;
        }
    }
}