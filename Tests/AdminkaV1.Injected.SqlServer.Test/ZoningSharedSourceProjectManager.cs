using DashboardCode.AdminkaV1.DataAccessEfCore;
using DashboardCode.Routines.Configuration;
#if NETCOREAPP2_0
    using DashboardCode.AdminkaV1.Injected.NETStandard;
    using DashboardCode.Routines.Configuration.NETStandard;
#else
    using DashboardCode.AdminkaV1.Injected.NETFramework;
    using DashboardCode.Routines.Configuration.NETFramework;
#endif

namespace DashboardCode.AdminkaV1.Injected.SqlServer.Test
{
    public static class ZoningSharedSourceProjectManager
    {
        static readonly ConfigurationManagerLoader ConfigurationManagerLoader = new ConfigurationManagerLoader();

        public static AdminkaStorageConfiguration GetConfiguration() =>
            new SqlServerAdmikaConfigurationFacade(ConfigurationManagerLoader).ResolveAdminkaStorageConfiguration();

        public static IConfigurationFactory GetConfigurationFactory() =>
            new ConfigurationFactory(ConfigurationManagerLoader);
    }
}