using Microsoft.Extensions.Configuration;
using Vse.Routines.Configuration;
using Vse.Routines.Configuration.NETStandard;

namespace Vse.AdminkaV1.Web.MvcCoreApp
{
    public class MvcAppConfiguration : IAppConfiguration
    {
        public IConfigurationRoot ConfigurationRoot { get; private set; }
        public MvcAppConfiguration(IConfigurationRoot configurationRoot){
            this.ConfigurationRoot=configurationRoot;
        }

        public SpecifiableConfigurationContainer GetConfigurationContainer(string @namespace, string @class, string member)
        {
            return RoutinesConfigurationManager.GetConfigurationContainer(ConfigurationRoot, @namespace, @class, member);
        }

        public string GetConnectionString()
        {
            return RoutinesConfigurationManager.GetConnectionString(ConfigurationRoot, "adminka");
        }
    }
}
