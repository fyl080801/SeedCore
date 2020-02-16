using System;
using System.IO;
using System.Reflection;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using SeedCore.SpaService;
using SeedCore.SpaService.Internal;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class ApplicationBuilderExtensions
    {
        public static void UseSeedSpa(this IApplicationBuilder app, Action<ISeedSpaBuilder> configuration)
        {
            if (configuration == null)
            {
                throw new ArgumentNullException(nameof(configuration));
            }

            var optionsProvider = app.ApplicationServices.GetService<IOptions<SeedSpaOptions>>();
            var options = new SeedSpaOptions(optionsProvider.Value);

            var spaBuilder = new DefaultSpaBuilder(app, options);
            configuration.Invoke(spaBuilder);

            // 发布后才用的到
            // SpaDefaultPageMiddleware.Attach(spaBuilder);
        }
    }
}