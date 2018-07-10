using DashboardCode.Routines.Configuration.Classic;

namespace DashboardCode.AdminkaV1.Injected
{
    public class ApplicationSettingsClassic : ApplicationSettings
    {
        public ApplicationSettingsClassic()
                : base(new ConfigurationManagerLoader(), new ConnectionStringMap(), new AppSettings())
        {
        }
    }
}


