using System.Configuration;

namespace Vse.Routines.Configuration
{
    public static class RoutinesConfigurationManager
    {
        public static SpecifiableConfigurationContainer  GetConfigurationContainer(string @namespace, string @class, string operationName)
        {
            var routinesConfigurationSection = (RoutinesConfigurationSection)ConfigurationManager.GetSection("routinesConfiguration");
            var rangedRoutines = routinesConfigurationSection.RangedRoutines(@namespace, @class, operationName);
            var configurationContainer = new SpecifiableConfigurationContainer (rangedRoutines);
            return configurationContainer;
        }
    }
}
