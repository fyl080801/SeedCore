using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.FileProviders.Physical;
using Microsoft.Extensions.Primitives;
using OrchardCore.Modules;
using System.IO;
using System.Linq;

namespace SeedCore.SpaService
{
    class SpaStaticFileProvider : ISpaStaticFileProvider
    {
        public const string SPA_BASE = "ClientApp/dist";

        private readonly IApplicationContext _applicationContext;

        public SpaStaticFileProvider(IApplicationContext applicationContext)
        {
            _applicationContext = applicationContext;
        }

        private Application Application => _applicationContext.Application;

        public IDirectoryContents GetDirectoryContents(string subpath)
        {
            return NotFoundDirectoryContents.Singleton;
        }

        public IFileInfo GetFileInfo(string subpath)
        {
            if (subpath == null)
            {
                return new NotFoundFileInfo(subpath);
            }

            var path = NormalizePath(subpath);

            var index = path.IndexOf('/');

            if (index != -1)
            {
                var application = _applicationContext.Application;

                var module = path.Substring(0, index);

                if (application.Modules.Any(m => m.Name == module))
                {
                    var fileSubPath = NormalizePath(Path.Combine(SPA_BASE, path.Substring(index + 1)));

                    if (module != application.Name)
                    {
                        return application.GetModule(module).GetFileInfo(fileSubPath);
                    }

                    return new PhysicalFileInfo(new FileInfo(application.Root + fileSubPath));
                }
            }

            return new NotFoundFileInfo(subpath);
        }

        public IChangeToken Watch(string filter)
        {
            return NullChangeToken.Singleton;
        }

        private string NormalizePath(string path)
        {
            return path.Replace('\\', '/').Trim('/').Replace("//", "/");
        }
    }
}
