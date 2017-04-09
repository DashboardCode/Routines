using Vse.Routines.Configuration;
using Vse.Routines.Configuration.NETFramework;

namespace Vse.AdminkaV1.Injected.NETFramework.Test
{
    public class ConfigurationNETFramework : IAppConfiguration
    {
        public SpecifiableConfigurationContainer GetConfigurationContainer(string @namespace, string @class, string member)
        {
            return RoutinesConfigurationManager.GetConfigurationContainer(@namespace, @class, member);
        }

        public string GetConnectionString()
        {
            return RoutinesConfigurationManager.GetConnectionString("adminka");
        }
    }
}
