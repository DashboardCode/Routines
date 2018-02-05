using DashboardCode.AdminkaV1.DataAccessEfCore;
using DashboardCode.Routines.Configuration;
#if NETCOREAPP1_1 || NETCOREAPP2_0
    using DashboardCode.AdminkaV1.Injected.NETStandard;
    using DashboardCode.Routines.Configuration.NETStandard;
#else
    using DashboardCode.AdminkaV1.Injected.NETFramework;
    using DashboardCode.Routines.Configuration.NETFramework;
#endif

namespace DashboardCode.AdminkaV1.Injected.SqlServer.Test
{
    public class ZoningSharedSourceManager
    {
        ConfigurationManagerLoader configurationManagerLoader;

        public ZoningSharedSourceManager(ConfigurationManagerLoader configurationManagerLoader) =>
            this.configurationManagerLoader= configurationManagerLoader;

        public ZoningSharedSourceManager() =>
            this.configurationManagerLoader = new ConfigurationManagerLoader();

        public AdminkaStorageConfiguration GetConfiguration() =>
            new SqlServerAdmikaConfigurationFacade(configurationManagerLoader).ResolveAdminkaStorageConfiguration();

        public IConfigurationFactory GetConfigurationFactory() =>
            new ConfigurationFactory(configurationManagerLoader);
    }
}