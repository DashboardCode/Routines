using System;
using System.Globalization;
using System.Threading.Tasks;
using System.Security.Principal;

using DashboardCode.Routines;
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
            IConfigurationFactory       configurationFactory,
            Func<RoutineGuid, IContainer, RoutineLoggingTransients> loggingTransientsFactory,
            string @namespace, string controller, string action,
            object input
        ) : this(
                adminkaStorageConfiguration,
                loggingTransientsFactory,
                InjectedManager.CreateContainerFactory(configurationFactory),
                new RoutineGuid(Guid.NewGuid(), @namespace, controller, action),
                InjectedManager.GetDefaultIdentity(),
                input)
        {
        }
        // Used in mvc app (identity get from request context)
        public AdminkaRoutineHandler(
            AdminkaStorageConfiguration adminkaStorageConfiguration,
            IConfigurationFactory configurationFactory,
            Func<RoutineGuid, IContainer, RoutineLoggingTransients> loggingTransientsFactory, // TODO
            RoutineGuid routineGuid,
            IIdentity identity,
            object input
        ) : this(
                adminkaStorageConfiguration,
                loggingTransientsFactory,
                InjectedManager.CreateContainerFactory(configurationFactory),
                routineGuid,
                identity,
                input)
        {
        }
        // Used as stage to prepare containerFactory, routineGuid
        private AdminkaRoutineHandler(
             AdminkaStorageConfiguration adminkaStorageConfiguration,
             Func<RoutineGuid, IContainer, RoutineLoggingTransients> loggingTransientsFactory,
             ContainerFactory<UserContext> containerFactory,
             RoutineGuid routineGuid,
             IIdentity identity,
             object input
        ) : this(
                adminkaStorageConfiguration,
                loggingTransientsFactory,
                containerFactory,
                routineGuid,
                InjectedManager.GetUserContext(loggingTransientsFactory, adminkaStorageConfiguration, containerFactory,
                    routineGuid, identity, CultureInfo.CurrentCulture),
                input)
        {
        }
        #endregion

        #region constructors with usercontext

        // Used in tests: predefined UserContext with custom logging to list
        public AdminkaRoutineHandler(
            AdminkaStorageConfiguration adminkaStorageConfiguration,
            IConfigurationFactory configurationFactory,
            Func<RoutineGuid, IContainer, RoutineLoggingTransients> loggingTransientsFactory,
            MemberTag memberTag,
            UserContext userContext,
            object input
            ) : this(
                adminkaStorageConfiguration,
                loggingTransientsFactory,
                InjectedManager.CreateContainerFactory(configurationFactory),
                new RoutineGuid(Guid.NewGuid(), memberTag),
                userContext,
                input)
        {
        }

        //  Used in apps: predefined UserContext and default NLOG logger
        public AdminkaRoutineHandler(
            AdminkaStorageConfiguration adminkaStorageConfiguration,
            IConfigurationFactory configurationFactory,
            MemberTag memberTag,
            UserContext userContext,
            object input
            ) : this(
                adminkaStorageConfiguration,
                InjectedManager.ComposeNLogTransients(InjectedManager.Markdown, InjectedManager.DefaultRoutineTagTransformException),
                InjectedManager.CreateContainerFactory(configurationFactory),
                new RoutineGuid(Guid.NewGuid(), memberTag),
                userContext,
                input)
        {
        }
        
        // used by wcf (predefined UserContext with its own transformException)
        public AdminkaRoutineHandler(
            AdminkaStorageConfiguration adminkaStorageConfiguration,
            IConfigurationFactory configurationFactory,
            Func<Exception, RoutineGuid, Func<Exception, string>, Exception> routineTransformException,
            RoutineGuid routineGuid,
            UserContext userContext,
            object input
            ) : this(
                adminkaStorageConfiguration,
                InjectedManager.ComposeNLogTransients(InjectedManager.Markdown, routineTransformException),
                routineGuid,
                userContext,
                InjectedManager.CreateContainer(configurationFactory, routineGuid, userContext),
                input)
        {
        }

        // Used as stage to prepare containerFactory, routineGuid and usercontext
        protected AdminkaRoutineHandler(
            AdminkaStorageConfiguration adminkaStorageConfiguration,
            Func<RoutineGuid, IContainer, RoutineLoggingTransients> loggingTransientsFactory,
            ContainerFactory<UserContext> containerFactory,
            RoutineGuid routineGuid,
            UserContext userContext,
            object input
        ) : this(
                adminkaStorageConfiguration,
                loggingTransientsFactory,
                routineGuid, 
                userContext,
                containerFactory.CreateContainer(routineGuid, userContext),
                input)
        {
        }
        #endregion

        // Used as stage to prepare container
        internal AdminkaRoutineHandler(
            AdminkaStorageConfiguration adminkaStorageConfiguration,
            Func<RoutineGuid, IContainer, RoutineLoggingTransients> loggingTransientsFactory,
            RoutineGuid routineGuid,
            UserContext userContext,
            IContainer container,
            object input) :
            this(
                adminkaStorageConfiguration,
                routineGuid,
                userContext,
                container,
                loggingTransientsFactory(routineGuid, container),
                input)
        {
        }

        //RoutineGuid routineGuid;
        RoutineLoggingTransients routineLoggingTransients;
        internal protected AdminkaRoutineHandler(
            AdminkaStorageConfiguration adminkaStorageConfiguration,
            RoutineGuid routineGuid,
            UserContext userContext,
            IContainer container,
            RoutineLoggingTransients routineLoggingTransients,
            object input
        ) : base(
                adminkaStorageConfiguration,
                InjectedManager.StorageMetaService,
                routineGuid,
                userContext,
                container,
                routineLoggingTransients.BasicRoutineLoggingAdapter,
                routineLoggingTransients.TransformException,
                input)
        {
            this.routineLoggingTransients = routineLoggingTransients;
        }

        public void HandleServicesContainer(Action<ITraceService> action) =>
            Handle(closure => action(new TraceService(CreateDbContextHandler(closure))));

        public TOutput HandleServicesContainer<TOutput>(Func<ITraceService, TOutput> func) =>
            Handle(closure => func(new TraceService(CreateDbContextHandler(closure))));

        public async Task<TOutput> HandleServicesContainerAsync<TOutput>(Func<ITraceService, TOutput> func) =>
            await HandleAsync(closure => func(new TraceService(CreateDbContextHandler(closure))));


        public TOutput HandleServicesContainer<TOutput>(
            Func<IAuthenticationService, RoutineClosure<UserContext>, Action<RoutineGuid, string>, TOutput> func) =>
            Handle(closure => func(new AuthenticationService(CreateDbContextHandler(closure)), closure, routineLoggingTransients.AuthenticationLoggingAdapter.TraceAuthentication));

        #region Handle Remote Services
        public void HandleRemoteServicesContainer(Action<ITraceService> action) =>
            Handle(closure => action(new LoggingDom.WcfClient.TraceServiceProxy()));

        public TOutput HandleRemoteServicesContainer<TOutput>(Func<ITraceService, TOutput> func) =>
            Handle(closure => func(new LoggingDom.WcfClient.TraceServiceProxy()));

        public async Task<TOutput> HandleRemoteServicesContainerAsync<TOutput>(Func<ITraceService, TOutput> func) =>
            await HandleAsync(closure => func(new LoggingDom.WcfClient.TraceServiceProxy())); // TODO: use TraceServiceAsyncProxy
        #endregion
    }
}