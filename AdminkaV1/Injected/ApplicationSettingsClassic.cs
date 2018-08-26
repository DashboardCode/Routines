using DashboardCode.Routines;
using DashboardCode.Routines.Configuration;
using DashboardCode.Routines.Configuration.Classic;

namespace DashboardCode.AdminkaV1.Injected
{
    public class ApplicationSettingsClassic : ApplicationSettings
    {
        private class Deserializer : IGWithConstructorFactory<string>
        {
            public TOutput Create<TOutput>(string input) where TOutput : new()
            {
                return InjectedManager.DeserializeJson<TOutput>(input);
            }
        }
        readonly static Deserializer deserializer = new Deserializer();
        public ApplicationSettingsClassic()
                : base( new ConnectionStringMap(), new AppSettings(), ResetConfigurationContainerFactory())
        {
        }

        public static IConfigurationContainerFactory ResetConfigurationContainerFactory()
        {
            return new ConfigurationContainerFactory<string>(new ConfigurationManagerLoader(), deserializer);
        }
    }
}


