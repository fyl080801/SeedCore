using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading.Tasks;
using System.Net;
using System.IO;
using System.Text;

namespace Microsoft.AspNetCore.Mvc
{
    public static class ControllerExtensions
    {
        public static async Task<IActionResult> SpaAsync(this Controller controller, string file)
        {
            var env = controller.ControllerContext.HttpContext.RequestServices.GetService<IHostEnvironment>();
            var assemblyName = controller.GetType().Assembly.GetName().Name;

            if (env.IsDevelopment())
            {
                var baseUriTaskFactory = SeedSpaBuilderExtensions.UrlHash[assemblyName] as Func<Task<Uri>>;
                if (baseUriTaskFactory == null)
                {
                    return await Task.FromResult(controller.View());
                }
                else
                {
                    var request = (HttpWebRequest)WebRequest.Create($"{await baseUriTaskFactory()}{assemblyName}/{file}");
                    request.Method = controller.Request.Method;
                    request.ContentType = controller.Request.ContentType;

                    using (var response = (HttpWebResponse)request.GetResponse())
                    {
                        using (var responseStream = response.GetResponseStream())
                        {
                            var responseReader = new StreamReader(responseStream, Encoding.GetEncoding(response.CharacterSet));
                            var responseText = responseReader.ReadToEnd();
                            return await Task.FromResult(controller.Content(responseText, response.ContentType));
                        }
                    }
                }
            }

            return await Task.FromResult(controller.View($"~/Areas/{assemblyName}/ClientApp/dist/{file}"));
        }

        public static IActionResult Spa(this Controller controller, string file)
        {
            return controller.SpaAsync(file).Result;
        }
    }
}