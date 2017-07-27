namespace DashboardCode.Routines.Configuration.NETFramework.Test
{
    public class ConfigurationNETFramework
    {
        public SpecifiableConfigurationContainer GetSpecifiableConfigurationContainer(string @namespace, string type, string member)
        {
            return RoutinesConfigurationManager.GetConfigurationContainer(@namespace, type, member);
        }
    }
}
