using Microsoft.AspNetCore.Mvc;

namespace Vse.AdminkaV1.Web.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            //var routine = new MvcRoutine(this, new { });
            //return routine.Handle(
            //    container =>
            //    {
                    return View();
                //});

        }

        public IActionResult About()
        {
            //var routine = new MvcRoutine(this, new { });
            //return routine.Handle(
            //    container =>
            //    {
                    ViewData["Message"] = "Your application description page.";
                    return View();
                //});
        }

        public IActionResult Contact()
        {
            //var routine = new MvcRoutine(this, new { });
            //return routine.Handle(
            //    container =>
            //    {
                    ViewData["Message"] = "Your contact page.";
                    return View();
                //});
        }

        public IActionResult Error()
        {
            //var routine = new MvcRoutine(this, new { });
            //return routine.Handle(
            //    container =>
            //    {
                    return View();
                //});
        }
    }
}
