using System;
using Microsoft.AspNetCore.SpaServices;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;
using System.IO;
using System.Linq;
using System.Xml;
using System.Xml.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Diagnostics;
using SeedCore.SpaService.Targets;

namespace SeedModules.Setup.Extensions
{
    public static class SpaExtensions
    {
        public static void UseSeedSpaDevelopmentServer(this ISpaBuilder spaBuilder, string npmScript)
        {
            if (spaBuilder == null)
            {
                throw new ArgumentNullException(nameof(spaBuilder));
            }

            var spaOptions = spaBuilder.Options;

            var assembly = Assembly.GetExecutingAssembly();
            var xxx = assembly.GetCustomAttribute<SpaProjectAttribute>();

            SpaMiddleware.Attach(spaBuilder, Path.Combine(xxx.Project, xxx.Path), npmScript);
        }
    }
}