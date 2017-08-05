namespace DashboardCode.Routines.Configuration.NETFramework.Test
{
    public class ConfigurationNETFramework
    {
        public SpecifiableConfigurationContainer GetSpecifiableConfigurationContainer(MemberTag memberTag)
        {
            return RoutinesConfigurationManager.GetConfigurationContainer(memberTag);
        }
    }
}
