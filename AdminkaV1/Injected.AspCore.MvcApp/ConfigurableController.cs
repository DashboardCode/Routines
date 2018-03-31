using DashboardCode.Routines.Configuration.NETStandard;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;

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