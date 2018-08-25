using DashboardCode.Routines.Configuration.Standard;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;

namespace DashboardCode.AdminkaV1.Injected.AspCore.MvcApp
{
    public class ConfigurableController : Controller
    {
        public readonly List<RoutineResolvable> RoutineResolvables;
        public readonly ApplicationSettingsStandard ApplicationSettingsStandard;
        public ConfigurableController(ApplicationSettingsStandard applicationSettingsStandard, List<RoutineResolvable> routineResolvables) : base()
        {
            RoutineResolvables = routineResolvables;
            ApplicationSettingsStandard = applicationSettingsStandard;
        }
    }
}