using Microsoft.Extensions.Configuration;
using DashboardCode.Routines.Configuration.Standard;
using DashboardCode.Routines.Configuration;
using DashboardCode.Routines;

namespace DashboardCode.AdminkaV1.Injected
{
    public class ApplicationSettingsStandard : ApplicationSettings
    {
        private class Deserializer : IGWithConstructorFactory<IConfigurationSection>
        {
            public TOutput Create<TOutput>(IConfigurationSection input) where TOutput : new()
            {
                var t = new TOutput();
                input.Bind(t);
                return t;
            }
        }

        static (IConnectionStringMap, IAppSettings, IConfigurationContainerFactory) Create(IConfigurationRoot configurationRoot)
        {
            if (configurationRoot == null)
            {
                ConfigurationBuilder configurationBuilder = new ConfigurationBuilder();
                configurationBuilder.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
                configurationRoot = configurationBuilder.Build();
            }
            var deserializer = new Deserializer();
            
            var connectionStringMap = new ConnectionStringMap(configurationRoot);
            var appSettings = new AppSettings(configurationRoot);

            var configurationContainerFactory = ResetConfigurationContainerFactory(configurationRoot);
            return (connectionStringMap, appSettings, configurationContainerFactory);
        }

        public ApplicationSettingsStandard(IConfigurationRoot configurationRoot = null):base(Create(configurationRoot))
        {
            
        }

        readonly static Deserializer deserializer = new Deserializer();
        public static IConfigurationContainerFactory ResetConfigurationContainerFactory(IConfigurationRoot configurationRoot)
        {
            var configurationManagerLoader = new ConfigurationManagerLoader(configurationRoot, deserializer);
            return ResetConfigurationContainerFactory(configurationManagerLoader);
        }

        public static IConfigurationContainerFactory ResetConfigurationContainerFactory(IConfigurationManagerLoader<IConfigurationSection> configurationManagerLoader)
        {
            return new ConfigurationContainerFactory<IConfigurationSection>(configurationManagerLoader, deserializer);
        }
    }
}


