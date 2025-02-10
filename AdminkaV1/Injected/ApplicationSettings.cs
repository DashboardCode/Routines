using System;
using DashboardCode.Routines.Configuration;
using DashboardCode.AdminkaV1.Injected.Logging;
using DashboardCode.AdminkaV1.Injected.Telemetry;

namespace DashboardCode.AdminkaV1.Injected
{

    // Connection string and instance name a cached there (readed only once)
    // There is a leak abstraction: how configurationManagerLoader works with disk on concreate platform (read on each call or now)
    // but all should support it.
    // ASP Core MVC implementation uses its "options" method to access routine configuration and uses ASP core recommended way to 
    // track changes (means share one instance between all processes)
    public class ApplicationSettings
    {
        public AdminkaStorageConfiguration AdminkaStorageConfiguration { get; private set; }
        public IPerformanceCounters PerformanceCounters { get; private set; }
        public IConfigurationContainerFactory ConfigurationContainerFactory { get; private set; }
        public readonly IUnhandledExceptionLogging UnhandledExceptionLogger;
        public readonly bool UseAdAuthorization;
        public readonly bool UseStandardDeveloperErrorPage;
        public readonly bool ForceDetailsOnCustomErrorPage;
        public readonly string InternalUsersDomain;
        //public readonly ActiveDirectoryService ActiveDirectoryService;

        public ApplicationSettings(
            //IConnectionStringMap connectionStringMap,
            IAppSettings appSettings,
            IConfigurationContainerFactory configurationContainerFactory,
            IUnhandledExceptionLogging unhandledExceptionLogger,
            AdminkaStorageConfiguration adminkaStorageConfiguration
            )
        {
            UnhandledExceptionLogger = unhandledExceptionLogger;
            UseAdAuthorization = bool.Parse(appSettings.GetValue("UseAdAuthorization") ?? "false");
            UseStandardDeveloperErrorPage = bool.Parse(appSettings.GetValue("UseStandardDeveloperErrorPage") ?? "false");
            ForceDetailsOnCustomErrorPage = bool.Parse(appSettings.GetValue("ForceDetailsOnCustomErrorPage") ?? "false");
            InternalUsersDomain = appSettings.GetValue("InternalUsersDomain");
            //ActiveDirectoryService = new ActiveDirectoryService(appSettings.GetValue("InternalUsersAdGroup"));
            AdminkaStorageConfiguration = adminkaStorageConfiguration;
            ConfigurationContainerFactory = configurationContainerFactory;

            //AuthenticationLogging = new NLogAuthenticationLogging();
            var instanceName = appSettings.GetValue("InstanceName");
            if (!string.IsNullOrEmpty(instanceName))
            {
                try
                {
                    if (InjectedManager.OperatingSystemIsWindows())
                    {
                        PerformanceCounters = new PerformanceCounters("DashboardCode Adminka", instanceName);
                    } else
                    {
                        PerformanceCounters = new PerformanceCountersStub();
                    }
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