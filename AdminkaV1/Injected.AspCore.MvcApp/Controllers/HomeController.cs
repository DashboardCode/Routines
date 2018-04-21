using System.Collections.Generic;
using DashboardCode.Routines.Configuration.NETStandard;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace DashboardCode.AdminkaV1.Injected.AspCore.MvcApp.Controllers
{
    public class HomeController : ConfigurableController
    {
        public HomeController(ApplicationSettings applicationSettings, IOptionsSnapshot<List<RoutineResolvable>> routineResolvablesOption) : base(applicationSettings, routineResolvablesOption.Value)
        {

        }

        public IActionResult Index()
        {
            var handler = new MvcRoutineHandler(this);
            return handler.Handle<IActionResult>((u) =>
            {
                return View();
            });
        }

        public IActionResult Error()
        {
            return View();
        }
    }
}
