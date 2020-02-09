using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace SeedCore.Web
{
    public class Program
    {
        // public static void Main(string[] args) =>
        //     BuildHost(args).RunAsync();

        // public static IHost BuildHost(string[] args) =>
        //     Host.CreateDefaultBuilder(args)
        //         .ConfigureLogging(logging => logging.ClearProviders())
        //         .ConfigureWebHostDefaults(webBuilder => webBuilder
        //             .UseStartup<Startup>()
        //             .UseNLogWeb())
        //         .Build();

        public static Task Main(string[] args)
            => BuildHost(args).RunAsync();

        public static IHost BuildHost(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder => webBuilder.UseStartup<Startup>())
                .Build();
    }
}
