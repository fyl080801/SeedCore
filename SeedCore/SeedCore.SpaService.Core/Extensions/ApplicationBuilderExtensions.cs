using System;
using System.IO;
using System.Reflection;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.SpaServices;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using SeedCore.SpaService;
using SeedCore.SpaService.Internal;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class ApplicationBuilderExtensions
    {
        public static void UseSpaDevelopment(this IApplicationBuilder app, Action<ISeedSpaBuilder> configuration)
        {
            if (configuration == null)
            {
                throw new ArgumentNullException(nameof(configuration));
            }

            var spaBuilder = new DefaultSpaBuilder(
                app,
                Assembly.GetCallingAssembly(),
                new SeedSpaOptions(app.ApplicationServices.GetService<IOptions<SeedSpaOptions>>().Value),
                app.ApplicationServices.GetService<IOptions<SpaOptions>>().Value);

            configuration.Invoke(spaBuilder);
        }
    }
}