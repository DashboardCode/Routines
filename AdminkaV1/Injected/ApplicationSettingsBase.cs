using System;

using DashboardCode.Routines.Configuration;

using DashboardCode.AdminkaV1.DataAccessEfCore;
using DashboardCode.AdminkaV1.Injected.Logging;
using DashboardCode.AdminkaV1.Injected.Diagnostics;

namespace DashboardCode.AdminkaV1.Injected
{
    // Connection string and instance name a cached there (readed only once)
    // There is a leak abstraction: how configurationManagerLoader works with disk on concreate platform (read on each call or now)
    // but all should support it.
    // ASP Core MVC implementation uses its "options" method to access routine configuration and uses ASP core recommended way to 
    // track changes (means share one instance between all processes)
    public abstract class ApplicationSettingsBase
    {
        public readonly AdminkaStorageConfiguration AdminkaStorageConfiguration;
        public readonly Func<string, AdminkaStorageConfiguration> CreateMigrationAdminkaStorageConfiguration;
        public readonly ConfigurationContainerFactory ConfigurationContainerFactory;
        public readonly IPerformanceCounters PerformanceCounters;
        public readonly NLogAuthenticationLogging AuthenticationLogging;

        public ApplicationSettingsBase(
            (IConfigurationManagerLoader configurationManagerLoader,
            IConnectionStringMap connectionStringMap,
            IAppSettings appSettings) tulpe
            ):this(tulpe.configurationManagerLoader, tulpe.connectionStringMap, tulpe.appSettings)
        {

        }

        public ApplicationSettingsBase(
            IConfigurationManagerLoader configurationManagerLoader,
            IConnectionStringMap connectionStringMap,
            IAppSettings appSettings
            )
        {
            var connectionString = connectionStringMap.GetConnectionString("AdminkaConnectionString");
            AdminkaStorageConfiguration = new AdminkaStorageConfiguration(connectionString, null, StorageType.SQLSERVER);
            CreateMigrationAdminkaStorageConfiguration = (migrationAssembly) =>
                 new AdminkaStorageConfiguration(connectionString, migrationAssembly, StorageType.SQLSERVER);
            ConfigurationContainerFactory = new ConfigurationContainerFactory(configurationManagerLoader);
            AuthenticationLogging = new NLogAuthenticationLogging();
            var instanceName = appSettings.GetValue("InstanceName");
            if (!string.IsNullOrEmpty(instanceName))
            {
                try
                {
                    PerformanceCounters = new PerformanceCounters("DashboardCode Adminka", instanceName);
                }
                catch
                {
                    PerformanceCounters = new PerformanceCountersStub();
                }
            }
            else
            {
                PerformanceCounters = new PerformanceCountersStub();
            }
        }
    }
}