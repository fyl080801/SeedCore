using System.Reflection;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.SpaServices;

namespace SeedCore.SpaService
{
    public class DefaultSpaBuilder : ISeedSpaBuilder
    {
        private IApplicationBuilder _application;
        private SeedSpaOptions _server;
        private SpaOptions _options;
        private Assembly _assembly;

        public DefaultSpaBuilder(IApplicationBuilder application, Assembly assembly, SeedSpaOptions server, SpaOptions options)
        {
            _application = application;
            _server = server;
            _options = options;
            _assembly = assembly;
        }

        public IApplicationBuilder ApplicationBuilder => _application;

        public SeedSpaOptions Server => _server;

        public SpaOptions Options => _options;

        public Assembly Assembly => _assembly;
    }
}