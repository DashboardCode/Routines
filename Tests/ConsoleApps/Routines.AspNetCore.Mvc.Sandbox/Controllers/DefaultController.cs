using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http.Extensions;

namespace Routines.AspNetCore.Mvc.Sandbox.Controllers
{
    public class DefaultController : Controller
    {
        public IActionResult Index()
        {
            var queryString = this.HttpContext.Request.GetDisplayUrl();
            Task.Factory.StartNew(
                    () =>  System.Threading.Thread.Sleep(4000)
                ).ContinueWith(
                    (notused) => Console.WriteLine($"API finishing: {queryString}")
            );
            Console.WriteLine("HTTP finishing");
            return Content(Guid.NewGuid().ToString()); 
        }
    }
}