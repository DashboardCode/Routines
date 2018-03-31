using Microsoft.Extensions.Configuration;
using DashboardCode.Routines.Configuration.NETStandard;
using DashboardCode.Routines.Configuration;

namespace DashboardCode.AdminkaV1.Injected
{
    public class ApplicationSettings : ApplicationSettingsBase
    {
        static (IConfigurationManagerLoader, IConnectionStringMap, IAppSettings) Create(IConfigurationRoot configurationRoot)
        {
            if (configurationRoot == null)
            {
                ConfigurationBuilder configurationBuilder = new ConfigurationBuilder();
                configurationBuilder.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
                configurationRoot = configurationBuilder.Build();
            }
            var configurationManagerLoader = new ConfigurationManagerLoader(configurationRoot);
            var connectionStringMap = new ConnectionStringMap(configurationRoot);
            var appSettings = new AppSettings(configurationRoot);
            return (configurationManagerLoader, connectionStringMap, appSettings);
        }

        public ApplicationSettings(IConfigurationRoot configurationRoot = null):base(Create(configurationRoot))
        {
            
        }
    }
}