using System;
using System.IO;
using System.Reflection;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.SpaServices;
using Microsoft.Extensions.Hosting;
using SeedCore.SpaService;
using SeedCore.SpaService.Internal;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class ApplicationBuilderExtensions
    {
        // public static void UseSeedSpaService(this IApplicationBuilder app, Action<ISeedSpaBuilder> confiruration) { }

        public static void UseSeedSpaService(this IApplicationBuilder app, string npmScript)
        {
            var env = app.ApplicationServices.GetService<IHostEnvironment>();

            var attribute = env.IsDevelopment() ? Assembly.GetCallingAssembly().GetCustomAttribute<SpaProjectAttribute>() : null;

            // 生产环境还要加上对嵌入资源的访问

            app.UseSpa(builder =>
            {
                if (env.IsDevelopment())
                {
                    builder.UseSeedSpaDevelopmentServer(Path.Combine(attribute.Project, attribute.Path), npmScript);
                }
            });
        }

        public static void UseSeedSpaDevelopmentServer(this ISpaBuilder spaBuilder, string clientPath, string npmScript)
        {
            if (spaBuilder == null)
            {
                throw new ArgumentNullException(nameof(spaBuilder));
            }

            object script = spaBuilder.Options;

            SpaServiceMiddleware.Attach(spaBuilder, clientPath, "npm", npmScript, default(int));
        }
    }
}