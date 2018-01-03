using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

namespace DashboardCode.AdminkaV1.Injected.AspCore.MvcApp
{
    public class ConfigurableController : Controller
    {
        public IConfigurationRoot ConfigurationRoot {get; private set;}
        public ConfigurableController(IConfigurationRoot configurationRoot): base() =>
            ConfigurationRoot = configurationRoot;
    }
}