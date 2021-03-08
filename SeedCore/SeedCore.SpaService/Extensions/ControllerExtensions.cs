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
        /// <summary>
        /// 
        /// </summary>
        /// <param name="controller"></param>
        /// <param name="module"></param>
        /// <param name="file"></param>
        /// <returns></returns>
        public static async Task<IActionResult> SpaAsync(this Controller controller, string module, string file)
        {
            var env = controller.HttpContext.RequestServices.GetService<IHostEnvironment>();
            var moduleName = module ?? controller.GetType().Assembly.GetName().Name;

            if (env.IsDevelopment() && SeedSpaBuilderExtensions.UrlHash.ContainsKey(moduleName))
            {
                var baseUriTaskFactory = SeedSpaBuilderExtensions.UrlHash[moduleName] as Func<Task<Uri>>;
                var request = (HttpWebRequest)WebRequest.Create($"{await baseUriTaskFactory()}{moduleName}/{file}");
                request.Method = controller.Request.Method;
                request.ContentType = controller.Request.ContentType;

                using (var response = (HttpWebResponse)request.GetResponse())
                {
                    using (var responseStream = response.GetResponseStream())
                    {
                        var responseReader = new StreamReader(responseStream, Encoding.GetEncoding(response.CharacterSet));
                        var responseText = responseReader.ReadToEnd();

                        // var remoteCachePath = $"Views/Areas/{moduleName}/_cache/";
                        // var partialMappedPath = Path.Combine(Environment.CurrentDirectory, remoteCachePath, file);
                        // File.WriteAllText(partialMappedPath, responseText);
                        // View("Cache", "")
                        return await Task.FromResult(controller.Content(responseText, response.ContentType));
                        // return await Task.FromResult(controller.View(partialMappedPath));
                    }
                }
            }

            return await Task.FromResult(controller.View($"~/Areas/{moduleName}/ClientApp/dist/{file}"));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="controller"></param>
        /// <param name="file"></param>
        /// <returns></returns>
        public static async Task<IActionResult> SpaAsync(this Controller controller, string file)
        {
            return await SpaAsync(controller, null, file);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="controller"></param>
        /// <param name="module"></param>
        /// <param name="file"></param>
        /// <returns></returns>
        public static IActionResult Spa(this Controller controller, string module, string file)
        {
            return controller.SpaAsync(module, file).Result;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="controller"></param>
        /// <param name="file"></param>
        /// <returns></returns>
        public static IActionResult Spa(this Controller controller, string file)
        {
            return controller.SpaAsync(file).Result;
        }
    }
}