using System;
using System.Globalization;
using System.Threading.Tasks;
using System.Security.Principal;

using DashboardCode.Routines;
using DashboardCode.Routines.Injected;
using DashboardCode.AdminkaV1.Injected.Logging;
using DashboardCode.AdminkaV1.DataAccessEfCore;
using DashboardCode.AdminkaV1.LoggingDom;
using DashboardCode.AdminkaV1.DataAccessEfCore.Services;
using DashboardCode.AdminkaV1.AuthenticationDom;

namespace DashboardCode.AdminkaV1.Injected
{
    public class AdminkaRoutineHandler : UserRoutineHandler<UserContext>
    {
        readonly AdminkaDataAccessFacade adminkaDataAccessFacade;
        protected readonly UserContext userContext;
        #region constructors without usercontext
        public AdminkaRoutineHandler(
            MemberTag memberTag,
            IAdmikaConfigurationFacade applicationFacade,
            object input
        ) : this(
              new RoutineGuid(Guid.NewGuid(), memberTag),
              InjectedManager.GetDefaultIdentity(),
              InjectedManager.ComposeNLogTransients(InjectedManager.Markdown, InjectedManager.DefaultRoutineTagTransformException),
              new AdminkaDataAccessFacade(InjectedManager.StorageMetaService, new AdminkaDbContextFactory(applicationFacade.ResolveAdminkaStorageConfiguration()).Create),
              new ContainerFactory(applicationFacade),
              input)
        {
        }
        public AdminkaRoutineHandler(
            RoutineGuid routineGuid,
            IAdmikaConfigurationFacade applicationFacade,
            object input
        ) : this(
              routineGuid,
              InjectedManager.GetDefaultIdentity(),
              InjectedManager.ComposeNLogTransients(InjectedManager.Markdown, InjectedManager.DefaultRoutineTagTransformException),
              new AdminkaDataAccessFacade(InjectedManager.StorageMetaService, new AdminkaDbContextFactory(applicationFacade.ResolveAdminkaStorageConfiguration()).Create),
              new ContainerFactory(applicationFacade),
              input)
        {
        }
        public AdminkaRoutineHandler(
            string @namespace, string controller, string action,
            Func<RoutineGuid, IContainer, RoutineLoggingTransients> loggingTransientsFactory,
            IAdmikaConfigurationFacade applicationFacade,
            object input
        ) : this(
              new RoutineGuid(Guid.NewGuid(), @namespace, controller, action),
              InjectedManager.GetDefaultIdentity(),
              loggingTransientsFactory,
              new AdminkaDataAccessFacade(InjectedManager.StorageMetaService, new AdminkaDbContextFactory(applicationFacade.ResolveAdminkaStorageConfiguration()).Create),
              new ContainerFactory(applicationFacade),

              input)
        {
        }
        public AdminkaRoutineHandler(
            RoutineGuid routineGuid,
            IIdentity identity,
            Func<RoutineGuid, IContainer, RoutineLoggingTransients> loggingTransientsFactory,
            IAdmikaConfigurationFacade applicationFacade,
            object input
        ) : this(
              routineGuid,
              identity,
              loggingTransientsFactory,
              new AdminkaDataAccessFacade(InjectedManager.StorageMetaService, new AdminkaDbContextFactory(applicationFacade.ResolveAdminkaStorageConfiguration()).Create),
              new ContainerFactory(applicationFacade),
              input)
        {
        }
        private AdminkaRoutineHandler(
             RoutineGuid routineGuid,
             IIdentity identity,
             Func<RoutineGuid, IContainer, RoutineLoggingTransients> loggingTransientsFactory,
             AdminkaDataAccessFacade adminkaDataAccessFacade,
             ContainerFactory configurationContainerFactory,
             object input
        ) : this(
              routineGuid,
              identity,
              CultureInfo.CurrentCulture,
              loggingTransientsFactory,
              adminkaDataAccessFacade,
              new UserContextFactory(loggingTransientsFactory, adminkaDataAccessFacade, configurationContainerFactory),
              configurationContainerFactory,
              input)
        {
        }
        private AdminkaRoutineHandler(
            RoutineGuid routineGuid,
            Func<RoutineGuid, IContainer, RoutineLoggingTransients> loggingTransientsFactory,
            AdminkaDataAccessFacade adminkaDataAccessFacade,
            ContainerFactory configurationContainerFactory,
            object input
        ) : this(
              routineGuid,
              InjectedManager.GetDefaultIdentity(),
              CultureInfo.CurrentCulture,
              loggingTransientsFactory,
              adminkaDataAccessFacade,
              new UserContextFactory(loggingTransientsFactory, adminkaDataAccessFacade, configurationContainerFactory),
              configurationContainerFactory,
              input)
        {
        }
        private AdminkaRoutineHandler(
            RoutineGuid routineGuid,
            IIdentity identity,
            CultureInfo cultureInfo,
            Func<RoutineGuid, IContainer, RoutineLoggingTransients> loggingTransientsFactory,
            AdminkaDataAccessFacade adminkaDataAccessFacade,
            UserContextFactory authenticationServices,
            ContainerFactory configurationContainerFactory,
            object input
            ) : this(
                  routineGuid,
                  authenticationServices.Create(routineGuid, identity, cultureInfo),
                  loggingTransientsFactory,
                  adminkaDataAccessFacade,
                  configurationContainerFactory,
                  input)
        {
        }
        #endregion

        #region constructors with usercontext
        public AdminkaRoutineHandler(
            string @namespace, string controller, string action,
            UserContext userContext,
            IAdmikaConfigurationFacade applicationFacade,
            object input
            ) : this(
                    new RoutineGuid(Guid.NewGuid(), @namespace, controller, action),
                    userContext,
                    new ContainerFactory(applicationFacade),
                    new AdminkaDataAccessFacade(InjectedManager.StorageMetaService, new AdminkaDbContextFactory(applicationFacade.ResolveAdminkaStorageConfiguration()).Create),
                    input)
        {
        }
        public AdminkaRoutineHandler(
            string @namespace, string controller, string action,
            UserContext userContext,
            Func<RoutineGuid, IContainer, RoutineLoggingTransients> loggingTransientsFactory,
            IAdmikaConfigurationFacade applicationFacade,
            object input
            ) : this(
                    @namespace, controller, action,
                    userContext,
                    loggingTransientsFactory,
                    new ContainerFactory(applicationFacade),
                    new AdminkaDataAccessFacade(InjectedManager.StorageMetaService, new AdminkaDbContextFactory(applicationFacade.ResolveAdminkaStorageConfiguration()).Create),
                    input)
        {
        }
        public AdminkaRoutineHandler(
            MemberTag memberTag,
            UserContext userContext,
            IAdmikaConfigurationFacade applicationFacade,
            object input
            ) : this(new RoutineGuid(Guid.NewGuid(), memberTag),
                    userContext,
                    new ContainerFactory(applicationFacade),
                    new AdminkaDataAccessFacade(InjectedManager.StorageMetaService, new AdminkaDbContextFactory(applicationFacade.ResolveAdminkaStorageConfiguration()).Create),
                    input)
        {
        }
        public AdminkaRoutineHandler(
            RoutineGuid routineGuid,
            UserContext userContext,
            IAdmikaConfigurationFacade applicationFacade,
            object input
            ) : this(routineGuid,
            userContext,
            new ContainerFactory(applicationFacade),
            new AdminkaDataAccessFacade(InjectedManager.StorageMetaService, new AdminkaDbContextFactory(applicationFacade.ResolveAdminkaStorageConfiguration()).Create),
            input)
        {
        }

        public AdminkaRoutineHandler(
            RoutineGuid routineGuid,
            UserContext userContext,
            Func<Exception, RoutineGuid, Func<Exception, string>, Exception> routineTransformException,
            IAdmikaConfigurationFacade applicationFacade,
            object input
            ) : this(routineGuid,
            userContext,
            routineTransformException,
            new ContainerFactory(applicationFacade),
            new AdminkaDataAccessFacade(InjectedManager.StorageMetaService, new AdminkaDbContextFactory(applicationFacade.ResolveAdminkaStorageConfiguration()).Create),
            input)
        {
        }

        private protected AdminkaRoutineHandler(
            RoutineGuid routineGuid,
            UserContext userContext,
            ContainerFactory configurationContainerFactory,
            AdminkaDataAccessFacade adminkaDataAccessFacade,
            object input
            ) : this(routineGuid,
                    userContext,
                    configurationContainerFactory.CreateContainer(routineGuid, userContext),
                    InjectedManager.ComposeNLogTransients(InjectedManager.Markdown, InjectedManager.DefaultRoutineTagTransformException),
                    adminkaDataAccessFacade,
                    input)
        {
        }

        private protected AdminkaRoutineHandler(
            string @namespace, string controller, string action,
            UserContext userContext,
            Func<RoutineGuid, IContainer, RoutineLoggingTransients> loggingTransientsFactory,
            ContainerFactory configurationContainerFactory,
            AdminkaDataAccessFacade adminkaDataAccessFacade,
            object input
            ) : this(
                    new RoutineGuid(Guid.NewGuid(), @namespace, controller, action),
                    userContext,
                    loggingTransientsFactory,
                    adminkaDataAccessFacade,
                    configurationContainerFactory,
                    input)
        {
        }

        private protected AdminkaRoutineHandler(
            RoutineGuid routineGuid,
            UserContext userContext,
            Func<Exception, RoutineGuid, Func<Exception, string>, Exception> routineTransformException,
            ContainerFactory configurationContainerFactory,
            AdminkaDataAccessFacade adminkaDataAccessFacade,
            object input
        ) : this(
           routineGuid,
           userContext,
           configurationContainerFactory.CreateContainer(routineGuid, userContext),
           InjectedManager.ComposeNLogTransients(InjectedManager.Markdown, routineTransformException),
           adminkaDataAccessFacade,
           input)
        {
        }

        private protected AdminkaRoutineHandler(
            RoutineGuid routineGuid,
            UserContext userContext,
            Func<RoutineGuid, IContainer, RoutineLoggingTransients> loggingTransientsFactory,
            AdminkaDataAccessFacade adminkaDataAccessFacade,
            ContainerFactory configurationContainerFactory,
            object input
            ) : this(
                routineGuid,
                userContext,
                configurationContainerFactory.CreateContainer(routineGuid, userContext),
                loggingTransientsFactory,
                adminkaDataAccessFacade,
                input)
        {
        }
        #endregion

        internal AdminkaRoutineHandler(
            RoutineGuid routineGuid,
            UserContext userContext,
            IContainer container,
            Func<RoutineGuid, IContainer, RoutineLoggingTransients> loggingTransientsFactory,
            AdminkaDataAccessFacade adminkaDataAccessFacade,
            object input
            ) : this(
                routineGuid,
                userContext,
                container,
                loggingTransientsFactory(routineGuid, container),
                adminkaDataAccessFacade,
                input)
        {
        }

        internal AdminkaRoutineHandler(
            RoutineGuid routineGuid,
            UserContext userContext,
            IContainer container,
            RoutineLoggingTransients routineLoggingTransients,
            AdminkaDataAccessFacade adminkaDataAccessFacade,
            object input
            ) : base(
                routineLoggingTransients.BasicRoutineLoggingAdapter,
                routineLoggingTransients.TransformException,
                verbose => new RoutineClosure<UserContext>(userContext, routineGuid, verbose, container),
                adminkaDataAccessFacade.RepositoryHandlerFactory,
                adminkaDataAccessFacade.OrmHandlerFactory,
                input
                )
        {
            this.userContext = userContext;
            this.adminkaDataAccessFacade = adminkaDataAccessFacade;
        }

        #region Handle with AdminkaDbContext
        public void HandleDbContext(AdminkaDbContextAction action) =>
            Handle(adminkaDataAccessFacade.ComposeAdminkaDbContextActionHandled(action));

        public TOutput HandleDbContext<TOutput>(AdminkaDbContextFunc<TOutput> func) =>
            Handle(adminkaDataAccessFacade.ComposeAdminkaDbContextFuncHandled(func));

        public async Task<TOutput> HandleDbContextAsync<TOutput>(AdminkaDbContextFunc<TOutput> func) =>
            await HandleAsync(adminkaDataAccessFacade.ComposeAdminkaDbContextFuncHandled(func));


        public void HandleDbContext(AdminkaDbContextUniAction action) =>
            Handle(adminkaDataAccessFacade.ComposeAdminkaDbContextActionHandled(action));

        public TOutput HandleDbContext<TOutput>(AdminkaDbContextUniFunc<TOutput> func) =>
            Handle(adminkaDataAccessFacade.ComposeAdminkaDbContextFuncHandled(func));

        public async Task<TOutput> HandleDbContextAsync<TOutput>(AdminkaDbContextUniFunc<TOutput> func) =>
            await HandleAsync(adminkaDataAccessFacade.ComposeAdminkaDbContextFuncHandled(func));

        #endregion

        #region Handle with Handler's Factory
        public void HandleOrmFactory(AdminkaDbContextOrmFactoryAction action) =>
            Handle(adminkaDataAccessFacade.ComposeAdminkaDbContextOrmFactoryActionHandled(action));

        public TOutput HandleOrmFactory<TOutput>(AdminkaDbContextOrmFactoryFunc<TOutput> func) =>
            Handle(adminkaDataAccessFacade.ComposeAdminkaDbContextOrmFactoryFuncHandled(func));
        
        public async Task<TOutput> HandleOrmFactoryAsync<TOutput>(AdminkaDbContextOrmFactoryFunc<TOutput> func) =>
            await HandleAsync(adminkaDataAccessFacade.ComposeAdminkaDbContextOrmFactoryFuncHandled(func));

        public void HandleRepositoryFactory(AdminkaDbContextRepositoryFactoryAction action) =>
            Handle(adminkaDataAccessFacade.ComposeAdminkaDbContextRepositoryFactoryActionHandled(action));

        public TOutput HandleRepositoryFactory<TOutput>(AdminkaDbContextRepositoryFactoryFunc<TOutput> func) =>
            Handle(adminkaDataAccessFacade.ComposeAdminkaDbContextRepositoryFactoryFuncHandled(func));

        public async Task<TOutput> HandleRepositoryFactoryAsync<TOutput>(AdminkaDbContextRepositoryFactoryFunc<TOutput> func) =>
            await HandleAsync(adminkaDataAccessFacade.ComposeAdminkaDbContextRepositoryFactoryFuncHandled(func));



        public void HandleOrmFactory(AdminkaDbContextOrmFactoryUniAction action) =>
            Handle(adminkaDataAccessFacade.ComposeAdminkaDbContextOrmFactoryActionHandled(action));

        public TOutput HandleOrmFactory<TOutput>(AdminkaDbContextOrmFactoryUniFunc<TOutput> func) =>
            Handle(adminkaDataAccessFacade.ComposeAdminkaDbContextOrmFactoryFuncHandled(func));

        public async Task<TOutput> HandleOrmFactoryAsync<TOutput>(AdminkaDbContextOrmFactoryUniFunc<TOutput> func) =>
            await HandleAsync(adminkaDataAccessFacade.ComposeAdminkaDbContextOrmFactoryFuncHandled(func));

        public void HandleRepositoryFactory(AdminkaDbContextRepositoryFactoryUniAction action) =>
            Handle(adminkaDataAccessFacade.ComposeAdminkaDbContextRepositoryFactoryActionHandled(action));

        public TOutput HandleRepositoryFactory<TOutput>(AdminkaDbContextRepositoryFactoryUniFunc<TOutput> func) =>
            Handle(adminkaDataAccessFacade.ComposeAdminkaDbContextRepositoryFactoryFuncHandled(func));

        public async Task<TOutput> HandleRepositoryFactoryAsync<TOutput>(AdminkaDbContextRepositoryFactoryUniFunc<TOutput> func) =>
            await HandleAsync(adminkaDataAccessFacade.ComposeAdminkaDbContextRepositoryFactoryFuncHandled(func));

        //public delegate void AdminkaDbContextUniAction(AdminkaDbContext adminkaDbContext);

        //public delegate TOutput AdminkaDbContextUniFunc<TOutput>(AdminkaDbContext adminkaDbContext);

        //public delegate void AdminkaDbContextOrmFactoryUniAction(ReliantOrmHandlerGFactory<UserContext, AdminkaDbContext> adminkaDbContextFactory);

        //public delegate TOutput AdminkaDbContextOrmFactoryUniFunc<TOutput>(ReliantOrmHandlerGFactory<UserContext, AdminkaDbContext> adminkaDbContextFactory);

        //public delegate void AdminkaDbContextRepositoryFactoryUniAction(ReliantRepositoryHandlerGFactory<UserContext, AdminkaDbContext> adminkaDbContextFactory);

        //public delegate TOutput AdminkaDbContextRepositoryFactoryUniFunc<TOutput>(ReliantRepositoryHandlerGFactory<UserContext, AdminkaDbContext> adminkaDbContextFactory);
        #endregion

        #region Handle Business Layer Services
        public void HandleServicesContainer(Action<ITraceService> action) =>
            Handle(closure => action(new TraceService(adminkaDataAccessFacade.CreateDbContextHandler(closure))));

        public TOutput HandleServicesContainer<TOutput>(Func<ITraceService, TOutput> func) =>
            Handle(closure => func(new TraceService(adminkaDataAccessFacade.CreateDbContextHandler(closure))));

        public async Task<TOutput> HandleServicesContainerAsync<TOutput>(Func<ITraceService, TOutput> func) =>
            await HandleAsync(closure => func(new TraceService(adminkaDataAccessFacade.CreateDbContextHandler(closure))));

        public void HandleServicesContainer(Action<IAuthenticationService> action) =>
            Handle(closure => action(new AuthenticationService(adminkaDataAccessFacade.CreateDbContextHandler(closure))));

        public TOutput HandleServicesContainer<TOutput>(Func<IAuthenticationService, TOutput> func) =>
            Handle(closure => func(new AuthenticationService(adminkaDataAccessFacade.CreateDbContextHandler(closure) )));

        public async Task<TOutput> HandleServicesContainerAsync<TOutput>(Func<IAuthenticationService, TOutput> func) =>
            await HandleAsync(closure => func(new AuthenticationService(adminkaDataAccessFacade.CreateDbContextHandler(closure) )));
        #endregion

        #region Handle Remote Services
        public void HandleRemoteServicesContainer(Action<ITraceService> action) =>
            Handle(state => action(new LoggingDom.WcfClient.TraceServiceProxy()));

        public TOutput HandleRemoteServicesContainer<TOutput>(Func<ITraceService, TOutput> func) =>
            Handle(state => func(new LoggingDom.WcfClient.TraceServiceProxy()));

        public async Task<TOutput> HandleRemoteServicesContainerAsync<TOutput>(Func<ITraceService, TOutput> func) =>
            await HandleAsync(state => func(new LoggingDom.WcfClient.TraceServiceProxy())); // TODO: use TraceServiceAsyncProxy
        #endregion
    }
}