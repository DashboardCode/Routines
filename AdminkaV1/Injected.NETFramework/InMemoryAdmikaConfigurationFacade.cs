using DashboardCode.Routines;
using DashboardCode.Routines.Configuration;

using DashboardCode.AdminkaV1.DataAccessEfCore;

namespace DashboardCode.AdminkaV1.Injected.NETFramework
{
    public class ConfigurationFactory : IConfigurationFactory
    {
        readonly IConfigurationManagerLoader configurationManagerLoader;

        public ConfigurationFactory(IConfigurationManagerLoader configurationManagerLoader ) =>
            this.configurationManagerLoader = configurationManagerLoader;

        public ConfigurationContainer ComposeSpecify(MemberTag memberTag, string @for) =>
          new ConfigurationContainer(configurationManagerLoader, memberTag, @for);
    }

    public class InMemoryAdmikaConfigurationFacade //: IAdminkaConfigurationFacade
    {
        readonly string databaseName;
        //readonly IConfigurationManagerLoader configurationManagerLoader;
        public InMemoryAdmikaConfigurationFacade(string databaseName)
        {
            this.databaseName = databaseName;
            //configurationManagerLoader = new ConfigurationManagerLoader();
        }

        //public ConfigurationContainer ComposeSpecify(MemberTag memberTag, string @for) =>
        //    new ConfigurationContainer(configurationManagerLoader, memberTag, @for);

        public AdminkaStorageConfiguration ResolveAdminkaStorageConfiguration() =>
            new AdminkaStorageConfiguration(databaseName, null, StorageType.INMEMORY);
    }
}