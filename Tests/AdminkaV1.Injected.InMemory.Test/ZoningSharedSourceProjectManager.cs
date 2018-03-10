using DashboardCode.Routines.Configuration;
using DashboardCode.AdminkaV1.DataAccessEfCore;

#if NETCOREAPP2_0
    using DashboardCode.AdminkaV1.Injected.NETStandard;
#else
    using DashboardCode.AdminkaV1.Injected.NETFramework;
#endif

namespace DashboardCode.AdminkaV1.Injected.InMemory.Test
{
    public class ZoningSharedSourceProjectManager
    {
        public static AdminkaStorageConfiguration GetConfiguration(string databaseName) =>
            new InMemoryAdmikaConfigurationFacade(databaseName).ResolveAdminkaStorageConfiguration();

        public static IConfigurationContainerFactory GetConfigurationFactory() =>
            new ConfigurationContainerFactory();
    }
}