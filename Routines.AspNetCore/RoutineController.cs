using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

namespace DashboardCode.Routines.AspNetCore
{
    public class RoutineController : Controller
    {
        public IConfigurationRoot ConfigurationRoot { get; private set;}
        public RoutineController(IConfigurationRoot configurationRoot): base()
        {
            ConfigurationRoot = configurationRoot;
        }
    }
}