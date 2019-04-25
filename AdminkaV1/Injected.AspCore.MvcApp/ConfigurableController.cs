using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using DashboardCode.Routines.Configuration.Standard;

namespace DashboardCode.AdminkaV1.Injected.AspCore.MvcApp
{
    public class ConfigurableController : Controller
    {
        public readonly List<RoutineResolvable> RoutineResolvables;
        public readonly ApplicationSettings ApplicationSettings;
        public ConfigurableController(ApplicationSettings applicationSettings, List<RoutineResolvable> routineResolvables) : base()
        {
            RoutineResolvables = routineResolvables;
            ApplicationSettings = applicationSettings;
        }
    }
}