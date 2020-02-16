using Microsoft.AspNetCore.Builder;

namespace SeedCore.SpaService
{
    public class DefaultSpaBuilder : ISeedSpaBuilder
    {
        private IApplicationBuilder _application;
        private SeedSpaOptions _options;

        public DefaultSpaBuilder(IApplicationBuilder application, SeedSpaOptions options)
        {
            _application = application;
            _options = options;
        }

        public IApplicationBuilder ApplicationBuilder => _application;

        public SeedSpaOptions Options => _options;
    }
}