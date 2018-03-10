using System;
using System.Globalization;
using System.Threading.Tasks;
using System.Security.Principal;

using DashboardCode.Routines;
using DashboardCode.Routines.Injected;
using DashboardCode.Routines.Configuration;

using DashboardCode.AdminkaV1.Injected.Logging;
using DashboardCode.AdminkaV1.LoggingDom;
using DashboardCode.AdminkaV1.DataAccessEfCore;
using DashboardCode.AdminkaV1.DataAccessEfCore.Services;
using DashboardCode.AdminkaV1.AuthenticationDom;

namespace DashboardCode.AdminkaV1.Injected
{
    public class AdminkaRoutineHandler : AdminkaStorageRoutineHandler
    {
        #region constructors without usercontext
        public AdminkaRoutineHandler(
            AdminkaStorageConfiguration adminkaStorageConfiguration,
            IConfigurationContainerFactory configurationFactory,
            Func<RoutineLogger, MemberTag, ContainerFactory<UserContext>, UserContext, object, RoutineLoggingTransients> loggingTransientsFactory,
            MemberTag memberTag,
            object input
        ) : this(
                adminkaStorageConfiguration,
                loggingTransientsFactory,
                InjectedManager.CreateContainerFactory(configurationFactory),
                Guid.NewGuid(),
                memberTag,
                InjectedManager.GetDefaultIdentity(),
                input)
        {
        }
        public AdminkaRoutineHandler(
            AdminkaStorageConfiguration adminkaStorageConfiguration,
            IConfigurationContainerFactory       configurationFactory,
            Func<RoutineLogger, MemberTag, ContainerFactory<UserContext>, UserContext, object, RoutineLoggingTransients> loggingTransientsFactory,
            string @namespace, string controller, string action,
            object input
        ) : this(
                adminkaStorageConfiguration,
                loggingTransientsFactory,
                InjectedManager.CreateContainerFactory(configurationFactory),
                Guid.NewGuid(),
                new MemberTag(@namespace, controller, action),
                InjectedManager.GetDefaultIdentity(),
                input)
        {
        }
        // Used in mvc app (identity get from request context)
        public AdminkaRoutineHandler(
            AdminkaStorageConfiguration adminkaStorageConfiguration,
            IConfigurationContainerFactory configurationFactory,
            Func<RoutineLogger, MemberTag, ContainerFactory<UserContext>, UserContext, object, RoutineLoggingTransients> loggingTransientsFactory,
            Guid correlationToken,
            MemberTag memberTag,
            IIdentity identity,
            object input
        ) : this(
                adminkaStorageConfiguration,
                loggingTransientsFactory,
                InjectedManager.CreateContainerFactory(configurationFactory),
                correlationToken,
                memberTag,
                identity,
                input)
        {
        }
        // Used as stage to prepare containerFactory, memberTag
        private AdminkaRoutineHandler(
             AdminkaStorageConfiguration adminkaStorageConfiguration,
             Func<RoutineLogger, MemberTag, ContainerFactory<UserContext>, UserContext, object, RoutineLoggingTransients> loggingTransientsFactory,
             ContainerFactory<UserContext> containerFactory,
             Guid correlationToken,
             MemberTag memberTag,
             IIdentity identity,
             object input
        ) : this(
                adminkaStorageConfiguration,
                loggingTransientsFactory,
                containerFactory,
                correlationToken,
                memberTag,
                InjectedManager.GetUserContext(new RoutineLogger(correlationToken), loggingTransientsFactory, adminkaStorageConfiguration, containerFactory,
                    memberTag, identity, CultureInfo.CurrentCulture),
                input)
        {
        }
        #endregion

        #region constructors with usercontext

        // Used in tests: predefined UserContext with custom logging to list
        public AdminkaRoutineHandler(
            AdminkaStorageConfiguration adminkaStorageConfiguration,
            IConfigurationContainerFactory configurationFactory,
            Func<RoutineLogger, MemberTag, ContainerFactory<UserContext>, UserContext, object, RoutineLoggingTransients> loggingTransientsFactory,
            MemberTag memberTag,
            UserContext userContext,
            object input
            ) : this(
                adminkaStorageConfiguration,
                loggingTransientsFactory,
                InjectedManager.CreateContainerFactory(configurationFactory),
                Guid.NewGuid(),
                memberTag,
                userContext,
                input)
        {
        }

        //  Used in apps: predefined UserContext and default NLOG logger
        public AdminkaRoutineHandler(
            AdminkaStorageConfiguration adminkaStorageConfiguration,
            IConfigurationContainerFactory configurationFactory,
            MemberTag memberTag,
            UserContext userContext,
            object input
            ) : this(
                adminkaStorageConfiguration,
                InjectedManager.ComposeNLogTransients(InjectedManager.DefaultRoutineTagTransformException),
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
            IConfigurationContainerFactory configurationFactory,
            Func<Exception, Guid, MemberTag, Func<Exception, string>, Exception> routineTransformException,
            Guid correlationToken,
            MemberTag memberTag,
            UserContext userContext,
            object input
            ) : this(
                adminkaStorageConfiguration,
                new RoutineLogger(correlationToken),
                InjectedManager.ComposeNLogTransients(routineTransformException),
                InjectedManager.CreateContainerFactory(configurationFactory),
                memberTag,
                userContext,
                input)
        {
        }

        // Used as stage to prepare containerFactory, memberTag and usercontext
        private AdminkaRoutineHandler(
            AdminkaStorageConfiguration adminkaStorageConfiguration,
            Func<RoutineLogger, MemberTag, ContainerFactory<UserContext>, UserContext, object, RoutineLoggingTransients> loggingTransientsFactory,
            ContainerFactory<UserContext> containerFactory,
            Guid correaltionToken,
            MemberTag memberTag,
            UserContext userContext,
            object input
        ) : this(
                adminkaStorageConfiguration,
                new RoutineLogger(correaltionToken),
                loggingTransientsFactory,
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
            RoutineLogger routineLogger,
            Func<RoutineLogger, MemberTag, ContainerFactory<UserContext>, UserContext, object, RoutineLoggingTransients> loggingTransientsFactory,
            ContainerFactory<UserContext> containerFactory,
            MemberTag memberTag,
            UserContext userContext,
            object input) :
            this(
                adminkaStorageConfiguration,
                userContext,
                loggingTransientsFactory(routineLogger, memberTag, containerFactory, userContext, input)
                )
        {
        }


        IAuthenticationLogging authenticationLogging;
        internal protected AdminkaRoutineHandler(
            AdminkaStorageConfiguration adminkaStorageConfiguration,
            UserContext userContext,
            RoutineLoggingTransients routineLoggingTransients
        ) : base(
                adminkaStorageConfiguration,
                InjectedManager.EntityMetaServiceContainer,
                userContext,  // need usercontext for setting audit properties 
                routineLoggingTransients.Verbose,
                routineLoggingTransients.RoutineHandler)
        {
            this.authenticationLogging = routineLoggingTransients.AuthenticationLogging;
        }

        public void HandleServicesContainer(Action<ITraceService> action) =>
            Handle(closure => action(new TraceService(CreateDbContextHandler(closure))));

        public TOutput HandleServicesContainer<TOutput>(Func<ITraceService, TOutput> func) =>
            Handle(closure => func(new TraceService(CreateDbContextHandler(closure))));

        public Task<TOutput> HandleServicesContainerAsync<TOutput>(Func<ITraceService, Task<TOutput>> func) =>
            HandleAsync(closure => func(new TraceService(CreateDbContextHandler(closure))));

        public TOutput HandleServicesContainer<TOutput>(
            Func<IAuthenticationService, RoutineClosure<UserContext>, Action<Guid, MemberTag, string>, TOutput> func) =>
            Handle(closure => func(new AuthenticationService(CreateDbContextHandler(closure)), closure, authenticationLogging.TraceAuthentication));

        #region Handle Remote Services
        public void HandleRemoteServicesContainer(Action<ITraceService> action) =>
            Handle(closure => action(new LoggingDom.WcfClient.TraceServiceProxy()));

        public TOutput HandleRemoteServicesContainer<TOutput>(Func<ITraceService, TOutput> func) =>
            Handle(closure => func(new LoggingDom.WcfClient.TraceServiceProxy()));

        public Task<TOutput> HandleRemoteServicesContainerAsync<TOutput>(Func<ITraceServiceAsync, Task<TOutput>> func) =>
            HandleAsync(closure => func(new LoggingDom.WcfClient.TraceServiceAsyncProxy())); 
        #endregion
    }
}