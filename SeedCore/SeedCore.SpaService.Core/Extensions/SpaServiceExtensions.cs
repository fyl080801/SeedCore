using System;
using Microsoft.AspNetCore.SpaServices;
using System.IO;
using System.Reflection;
using SeedCore.SpaService.Core;

namespace SeedCore.SpaService.Core.Extensions
{
    public static class SpaServiceExtensions
    {
        public static void UseSeedSpaDevelopmentServer(this ISpaBuilder spaBuilder, string npmScript)
        {
            if (spaBuilder == null)
            {
                throw new ArgumentNullException(nameof(spaBuilder));
            }

            // var spaOptions = spaBuilder.Options;

            var attribute = Assembly.GetCallingAssembly().GetCustomAttribute<SpaProjectAttribute>();

            SpaServiceMiddleware.Attach(spaBuilder, Path.Combine(attribute.Project, attribute.Path), npmScript);
        }
    }
}