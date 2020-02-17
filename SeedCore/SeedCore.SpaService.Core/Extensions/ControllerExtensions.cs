using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace Microsoft.AspNetCore.Mvc
{
    public static class ControllerExtensions
    {
        public static IActionResult Spa(this Controller controller, string file)
        {
            var env = controller.ControllerContext.HttpContext.RequestServices.GetService<IHostEnvironment>();
            var assembly = Assembly.GetCallingAssembly();

            if (env.IsDevelopment())
            {
                return controller.View(); // controller.File(new byte[0], "text/html");
            }

            return controller.View($"~/Areas/{assembly.GetName().Name}/ClientApp/dist/{file}");
        }
    }
}