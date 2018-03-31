using DashboardCode.Routines.Configuration.NETFramework;

namespace DashboardCode.AdminkaV1.Injected
{
    public class ApplicationSettings : ApplicationSettingsBase
    {
        public ApplicationSettings()
            :base(new ConfigurationManagerLoader(), new ConnectionStringMap(), new AppSettings())
        {
        }
    }
}