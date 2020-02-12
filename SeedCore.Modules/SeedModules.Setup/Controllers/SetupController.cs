using System.IO;
using System.Net;
using System.Text;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Hosting;

namespace SeedModules.Setup.Controllers
{
    public class SetupController : Controller
    {
        private readonly IHostEnvironment _environment;

        public SetupController(IHostEnvironment environment)
        {
            _environment = environment;
        }

        public IActionResult Index()
        {
            // if (_environment.IsDevelopment())
            // {
            //     var request = (HttpWebRequest)WebRequest.Create("http://localhost:8080/");
            //     request.Method = "GET";
            //     request.ServicePoint.Expect100Continue = false;
            //     request.Timeout = 500;

            //     //接收响应内容
            //     var response = (HttpWebResponse)request.GetResponse();
            //     // var responseString = new StreamReader(response.GetResponseStream(), Encoding.GetEncoding("utf-8")).ReadToEnd();
            //     // var result = responseString.ToString();
            //     return this.File(response.GetResponseStream(), "text/html");//.StatusCode(200, response);//.Ok(result);// Content(result);
            // }
            return Content("aaa");
        }
    }
}