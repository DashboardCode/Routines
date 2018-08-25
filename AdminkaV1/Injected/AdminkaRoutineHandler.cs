using System;
using System.Globalization;
using System.Threading.Tasks;
using System.Security.Principal;

using DashboardCode.Routines;
using DashboardCode.Routines.Injected;
using DashboardCode.Routines.Configuration;

using DashboardCode.AdminkaV1.LoggingDom;
using DashboardCode.AdminkaV1.DataAccessEfCore;
using DashboardCode.AdminkaV1.DataAccessEfCore.Services;
using DashboardCode.AdminkaV1.AuthenticationDom;
using DashboardCode.AdminkaV1.Injected.Logging;
using DashboardCode.AdminkaV1.Injected.Diagnostics;
using DashboardCode.Routines.Storage;

namespace DashboardCode.AdminkaV1.Injected
{
    public class AdminkaRoutineHandler //: AdminkaStorageRoutineHandler
    {
        readonly AdminkaStorageRoutineHandler storageRoutineHandler;
        readonly IAuthenticationLogging authenticationLogging;
        public readonly UserContext UserContext;

        #region constructors without usercontext
        public AdminkaRoutineHandler(
            ApplicationSettings applicationSettings,
            Func<Guid, MemberTag, (IMemberLogger, IAuthenticationLogging)> composeLoggers,
            string @namespace, string controller, string action,
            object input
        ) : this(
                applicationSettings.AdminkaStorageConfiguration,
                applicationSettings.PerformanceCounters,
                applicationSettings.ConfigurationContainerFactory,
                composeLoggers,
                @namespace, controller, action,
                input)
        {
        }

        private AdminkaRoutineHandler(
            AdminkaStorageConfiguration adminkaStorageConfiguration,
            IPerformanceCounters performanceCounters,
            IConfigurationContainerFactory configurationContainerFactory,
            Func<Guid, MemberTag, (IMemberLogger, IAuthenticationLogging)> composeLoggers,
            string @namespace, string controller, string action,
            object input
        ) : this(
                adminkaStorageConfiguration,
                performanceCounters,
                composeLoggers,
                InjectedManager.CreateContainerFactory(configurationContainerFactory),
                Guid.NewGuid(),
                new MemberTag(@namespace, controller, action),
                InjectedManager.GetDefaultIdentity(),
                input)
        {
        }
        // Used in mvc app (identity get from request context)
        public AdminkaRoutineHandler(
            AdminkaStorageConfiguration adminkaStorageConfiguration,
            IPerformanceCounters performanceCounters,
            IConfigurationContainerFactory configurationContainerFactory,
            Func<Guid, MemberTag, (IMemberLogger, IAuthenticationLogging)> composeLoggers,
            Guid correlationToken,
            MemberTag memberTag,
            IIdentity identity,
            object input
        ) : this(
                adminkaStorageConfiguration,
                performanceCounters,
                composeLoggers,
                InjectedManager.CreateContainerFactory(configurationContainerFactory),
                correlationToken,
                memberTag,
                identity,
                input)
        {
        }
        // Used as stage to prepare containerFactory, userContest
        private AdminkaRoutineHandler(
             AdminkaStorageConfiguration adminkaStorageConfiguration,
             IPerformanceCounters performanceCounters,
             Func<Guid, MemberTag, (IMemberLogger, IAuthenticationLogging)> composeLoggers,
             ContainerFactory<UserContext> containerFactory,
             Guid correlationToken,
             MemberTag memberTag,
             IIdentity identity,
             object input
        ) : this(
                adminkaStorageConfiguration,
                performanceCounters,
                InjectedManager.DefaultRoutineTagTransformException,
                composeLoggers,
                containerFactory,
                correlationToken,
                memberTag,
                InjectedManager.GetUserContext(
                    new AdminkaRoutineLogger(
                        correlationToken, InjectedManager.DefaultRoutineTagTransformException, composeLoggers, performanceCounters), 
                    composeLoggers, adminkaStorageConfiguration, 
                    containerFactory,
                    memberTag, identity, CultureInfo.CurrentCulture),
                input)
        {
        }
        #endregion

        #region constructors with usercontext

        // Used in tests and benchmarks: predefined UserContext with custom logging to list, but with performance counters
        // TODO: benchmark should be used with full authentication
        public AdminkaRoutineHandler(
            ApplicationSettings applicationSettings,
            Func<Guid, MemberTag, (IMemberLogger, IAuthenticationLogging)> composeLoggers,
            MemberTag memberTag,
            UserContext userContext,
            object input
            ) : this(
                applicationSettings.AdminkaStorageConfiguration,
                applicationSettings.PerformanceCounters,
                applicationSettings.ConfigurationContainerFactory,
                composeLoggers,
                memberTag,
                userContext,
                input)
        {
        }

        // used in in memory routine - with in memory databases - "configuration string" can be different for every routine  
        public AdminkaRoutineHandler(
            AdminkaStorageConfiguration adminkaStorageConfiguration,
            IPerformanceCounters performanceCounters,
            IConfigurationContainerFactory configurationContainerFactory,
            Func<Guid, MemberTag, (IMemberLogger, IAuthenticationLogging)> composeLoggers,
            MemberTag memberTag,
            UserContext userContext,
            object input
            ) : this(
                adminkaStorageConfiguration,
                performanceCounters,
                InjectedManager.DefaultRoutineTagTransformException,
                composeLoggers,
                InjectedManager.CreateContainerFactory(configurationContainerFactory),
                Guid.NewGuid(),
                memberTag,
                userContext,
                input)
        {
        }

        //  Used in apps: predefined UserContext and default NLOG logger
        public AdminkaRoutineHandler(
            AdminkaStorageConfiguration adminkaStorageConfiguration,
            IPerformanceCounters performanceCounters,
            IAuthenticationLogging authenticationLogging,
            IConfigurationContainerFactory configurationFactory,
            MemberTag memberTag,
            UserContext userContext,
            object input
            ) : this(
                adminkaStorageConfiguration,
                performanceCounters,
                InjectedManager.DefaultRoutineTagTransformException,
                InjectedManager.ComposeNLogMemberLoggerFactory(authenticationLogging),
                InjectedManager.CreateContainerFactory(configurationFactory),
                Guid.NewGuid(),
                memberTag,
                userContext,
                input)
        {
        }

        // used by wcf (predefined UserContext with its own transformException)
        public AdminkaRoutineHandler(
            AdminkaStorageConfiguration adminkaStorageConfiguration,
            IPerformanceCounters performanceCounters,
            IAuthenticationLogging authenticationLogging,
            IConfigurationContainerFactory configurationFactory,
            Func<Exception, Guid, MemberTag, Func<Exception, string>, Exception> routineTransformException,
            Guid correlationToken,
            MemberTag memberTag,
            UserContext userContext,
            object input
            ) : this(
                adminkaStorageConfiguration,
                performanceCounters,
                routineTransformException,
                InjectedManager.ComposeNLogMemberLoggerFactory(authenticationLogging),
                InjectedManager.CreateContainerFactory(configurationFactory),
                correlationToken,
                memberTag,
                userContext,
                input)
        {
        }

        // Used as stage to prepare containerFactory, memberTag and usercontext
        private AdminkaRoutineHandler(
            AdminkaStorageConfiguration adminkaStorageConfiguration,
            IPerformanceCounters performanceCounters,
            Func<Exception, Guid, MemberTag, Func<Exception, string>, Exception> routineTransformException,
            Func<Guid, MemberTag, (IMemberLogger, IAuthenticationLogging)> composeLoggers,
            ContainerFactory<UserContext> containerFactory,
            Guid correaltionToken,
            MemberTag memberTag,
            UserContext userContext,
            object input
        ) : this(
                adminkaStorageConfiguration,
                new AdminkaRoutineLogger(correaltionToken, routineTransformException, composeLoggers, performanceCounters),
                containerFactory,
                memberTag, 
                userContext,
                input)
        {
        }
        #endregion

        // Used as stage to prepare container
        internal AdminkaRoutineHandler(
            AdminkaStorageConfiguration adminkaStorageConfiguration,
            AdminkaRoutineLogger routineLogger,
            ContainerFactory<UserContext> containerFactory,
            MemberTag memberTag,
            UserContext userContext,
            object input) :
            this(
                adminkaStorageConfiguration,
                userContext,
                routineLogger.CreateTransients(memberTag, containerFactory, userContext, input)
                )
        {
        }

        internal protected AdminkaRoutineHandler(
            AdminkaStorageConfiguration adminkaStorageConfiguration,
            UserContext userContext,
            RoutineLoggingTransients routineLoggingTransients
        )
        {
            this.UserContext = userContext;
            this.authenticationLogging = routineLoggingTransients.AuthenticationLogging;
            this.storageRoutineHandler = new AdminkaStorageRoutineHandler(
                    adminkaStorageConfiguration,
                    userContext,  // need usercontext for setting audit properties 
                    InjectedManager.EntityMetaServiceContainer,
                    routineLoggingTransients.Verbose,
                    routineLoggingTransients.RoutineHandler
                );
        }

        public UserRoutineHandler<UserContext> UserRoutineHandler { get { return storageRoutineHandler; }  }
        public StorageRoutineHandler<UserContext, AdminkaDbContext> StorageRoutineHandler { get { return storageRoutineHandler; } }

        public void HandleServicesContainer(Action<ITraceService> action) =>
            storageRoutineHandler.Handle(closure => action(new TraceService(storageRoutineHandler.CreateDbContextHandler(closure))));

        public TOutput HandleServicesContainer<TOutput>(Func<ITraceService, TOutput> func) =>
            storageRoutineHandler.Handle(closure => func(new TraceService(storageRoutineHandler.CreateDbContextHandler(closure))));

        public Task<TOutput> HandleServicesContainerAsync<TOutput>(Func<ITraceService, Task<TOutput>> func) =>
            storageRoutineHandler.HandleAsync(closure => func(new TraceService(storageRoutineHandler.CreateDbContextHandler(closure))));

        public TOutput HandleServicesContainer<TOutput>(
            Func<IAuthenticationService, RoutineClosure<UserContext>, Action<Guid, MemberTag, string>, TOutput> func) =>
            storageRoutineHandler.Handle(
                closure => func(
                    new AuthenticationService(storageRoutineHandler.CreateDbContextHandler(closure)), 
                    closure, authenticationLogging.TraceAuthentication));

        #region Handle Remote Services
        public void HandleRemoteServicesContainer(Action<ITraceService> action) =>
            storageRoutineHandler.Handle(closure => action(new LoggingDom.WcfClient.TraceServiceProxy()));

        public TOutput HandleRemoteServicesContainer<TOutput>(Func<ITraceService, TOutput> func) =>
            storageRoutineHandler.Handle(closure => func(new LoggingDom.WcfClient.TraceServiceProxy()));

        public Task<TOutput> HandleRemoteServicesContainerAsync<TOutput>(Func<ITraceServiceAsync, Task<TOutput>> func) =>
            storageRoutineHandler.HandleAsync(closure => func(new LoggingDom.WcfClient.TraceServiceAsyncProxy())); 
        #endregion
    }
}