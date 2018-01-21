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
        readonly FactoryProxy factoryProxy;
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
              new FactoryProxy(applicationFacade.ResolveAdminkaStorageConfiguration(), InjectedManager.StorageMetaService),
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
              new FactoryProxy(applicationFacade.ResolveAdminkaStorageConfiguration(), InjectedManager.StorageMetaService),
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
              new FactoryProxy(applicationFacade.ResolveAdminkaStorageConfiguration(), InjectedManager.StorageMetaService),
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
              new FactoryProxy(applicationFacade.ResolveAdminkaStorageConfiguration(), InjectedManager.StorageMetaService),
              new ContainerFactory(applicationFacade),
              input)
        {
        }
        private AdminkaRoutineHandler(
             RoutineGuid routineGuid,
             IIdentity identity,
             Func<RoutineGuid, IContainer, RoutineLoggingTransients> loggingTransientsFactory,
             FactoryProxy factoryProxy,
             ContainerFactory configurationContainerFactory,
             object input
        ) : this(
              routineGuid,
              identity,
              CultureInfo.CurrentCulture,
              loggingTransientsFactory,
              factoryProxy,
              new UserContextFactory(loggingTransientsFactory, factoryProxy, configurationContainerFactory),
              configurationContainerFactory,
              input)
        {
        }
        private AdminkaRoutineHandler(
            RoutineGuid routineGuid,
            Func<RoutineGuid, IContainer, RoutineLoggingTransients> loggingTransientsFactory,
            FactoryProxy factoryProxy,
            ContainerFactory configurationContainerFactory,
            object input
        ) : this(
              routineGuid,
              InjectedManager.GetDefaultIdentity(),
              CultureInfo.CurrentCulture,
              loggingTransientsFactory,
              factoryProxy,
              new UserContextFactory(loggingTransientsFactory, factoryProxy, configurationContainerFactory),
              configurationContainerFactory,
              input)
        {
        }
        private AdminkaRoutineHandler(
            RoutineGuid routineGuid,
            IIdentity identity,
            CultureInfo cultureInfo,
            Func<RoutineGuid, IContainer, RoutineLoggingTransients> loggingTransientsFactory,
            FactoryProxy factoryProxy,
            UserContextFactory authenticationServices,
            ContainerFactory configurationContainerFactory,
            object input
            ) : this(
                  routineGuid,
                  authenticationServices.Create(routineGuid, identity, cultureInfo),
                  loggingTransientsFactory,
                  factoryProxy,
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
                    new FactoryProxy(applicationFacade.ResolveAdminkaStorageConfiguration(), InjectedManager.StorageMetaService),
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
                    new FactoryProxy(applicationFacade.ResolveAdminkaStorageConfiguration(), InjectedManager.StorageMetaService),
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
                    new FactoryProxy(applicationFacade.ResolveAdminkaStorageConfiguration(), InjectedManager.StorageMetaService),
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
            new FactoryProxy(applicationFacade.ResolveAdminkaStorageConfiguration(), InjectedManager.StorageMetaService),
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
            new FactoryProxy(applicationFacade.ResolveAdminkaStorageConfiguration(), InjectedManager.StorageMetaService),
            input)
        {
        }

        private protected AdminkaRoutineHandler(
            RoutineGuid routineGuid,
            UserContext userContext,
            ContainerFactory configurationContainerFactory,
            FactoryProxy factoryProxy,
            object input
            ) : this(routineGuid,
                    userContext,
                    configurationContainerFactory.CreateContainer(routineGuid, userContext),
                    InjectedManager.ComposeNLogTransients(InjectedManager.Markdown, InjectedManager.DefaultRoutineTagTransformException),
                    factoryProxy,
                    input)
        {
        }

        private protected AdminkaRoutineHandler(
            string @namespace, string controller, string action,
            UserContext userContext,
            Func<RoutineGuid, IContainer, RoutineLoggingTransients> loggingTransientsFactory,
            ContainerFactory configurationContainerFactory,
            FactoryProxy factoryProxy,
            object input
            ) : this(
                    new RoutineGuid(Guid.NewGuid(), @namespace, controller, action),
                    userContext,
                    loggingTransientsFactory,
                    factoryProxy,
                    configurationContainerFactory,
                    input)
        {
        }

        private protected AdminkaRoutineHandler(
            RoutineGuid routineGuid,
            UserContext userContext,
            Func<Exception, RoutineGuid, Func<Exception, string>, Exception> routineTransformException,
            ContainerFactory configurationContainerFactory,
            FactoryProxy factoryProxy,
            object input
        ) : this(
           routineGuid,
           userContext,
           configurationContainerFactory.CreateContainer(routineGuid, userContext),
           InjectedManager.ComposeNLogTransients(InjectedManager.Markdown, routineTransformException),
           factoryProxy,
           input)
        {
        }

        private protected AdminkaRoutineHandler(
            RoutineGuid routineGuid,
            UserContext userContext,
            Func<RoutineGuid, IContainer, RoutineLoggingTransients> loggingTransientsFactory,
            FactoryProxy factoryProxy,
            ContainerFactory configurationContainerFactory,
            object input
            ) : this(
                routineGuid,
                userContext,
                configurationContainerFactory.CreateContainer(routineGuid, userContext),
                loggingTransientsFactory,
                factoryProxy,
                input)
        {
        }
        #endregion

        internal AdminkaRoutineHandler(
            RoutineGuid routineGuid,
            UserContext userContext,
            IContainer container,
            Func<RoutineGuid, IContainer, RoutineLoggingTransients> loggingTransientsFactory,
            FactoryProxy factoryProxy,
            object input
            ) : this(
                routineGuid,
                userContext,
                container,
                loggingTransientsFactory(routineGuid, container),
                factoryProxy,
                input)
        {
        }

        internal AdminkaRoutineHandler(
            RoutineGuid routineGuid,
            UserContext userContext,
            IContainer container,
            RoutineLoggingTransients routineLoggingTransients,
            FactoryProxy factoryProxy,
            object input
            ) : base(
                routineLoggingTransients.BasicRoutineLoggingAdapter,
                routineLoggingTransients.TransformException,
                verbose => new RoutineClosure<UserContext>(userContext, routineGuid, verbose, container),
                factoryProxy,
                factoryProxy,
                input
                )
        {
            this.userContext = userContext;
            this.factoryProxy = factoryProxy;
        }

        #region Handle with AdminkaDbContext
        public void HandleDbContext(Action<RoutineClosure<UserContext>, AdminkaDbContext> action) =>
            Handle(closure => factoryProxy
                        .CreateRepositoryAdminkaDbContextHandlerContainer(closure)
                        .Resolve().Handle(context => action(closure, context) )
            );

        public TOutput HandleDbContext<TOutput>(Func<RoutineClosure<UserContext>, AdminkaDbContext, TOutput> func) =>
            Handle(closure => factoryProxy
                        .CreateRepositoryAdminkaDbContextHandlerContainer(closure)
                        .Resolve().Handle(context => func(closure, context))
            );

        public async Task<TOutput> HandleDbContextAsync<TOutput>(Func<RoutineClosure<UserContext>, AdminkaDbContext, TOutput> func) =>
            await HandleAsync(closure => factoryProxy
                        .CreateRepositoryAdminkaDbContextHandlerContainer(closure)
                        .Resolve().Handle(context => func(closure, context))
            );
        #endregion

        #region Handle with AdminkaOrmHandlerFactory
        public void HandleOrmFactory(Action<RoutineClosure<UserContext>, AdminkaOrmHandlerFactory> action) =>
            Handle(closure => action(closure, factoryProxy.CreateAdminkaOrmHandlerFactory(closure)));

        public TOutput HandleOrmFactory<TOutput>(Func<RoutineClosure<UserContext>, AdminkaOrmHandlerFactory, TOutput> func) =>
            Handle(closure =>func(closure, factoryProxy.CreateAdminkaOrmHandlerFactory(closure)));
        
        public async Task<TOutput> HandleOrmFactoryAsync<TOutput>(Func<RoutineClosure<UserContext>, AdminkaOrmHandlerFactory, TOutput> func) =>
            await HandleAsync(closure => func(closure, factoryProxy.CreateAdminkaOrmHandlerFactory(closure)));
        #endregion

        #region Handle TraceService
        public void HandleServicesContainer(Action<ITraceService> action) =>
            Handle(state => action(new TraceService(factoryProxy.CreateRepositoryAdminkaDbContextHandlerContainer(state).Resolve() )));

        public TOutput HandleServicesContainer<TOutput>(Func<ITraceService, TOutput> func) =>
            Handle(state => func(new TraceService(factoryProxy.CreateRepositoryAdminkaDbContextHandlerContainer(state).Resolve())));

        public async Task<TOutput> HandleServicesContainerAsync<TOutput>(Func<ITraceService, TOutput> func) =>
            await HandleAsync(state => func(new TraceService(factoryProxy.CreateRepositoryAdminkaDbContextHandlerContainer(state).Resolve())));

        public void HandleServicesContainer(Action<IAuthenticationService> action) =>
            Handle(state => action(new AuthenticationService(factoryProxy.CreateRepositoryAdminkaDbContextHandlerContainer(state).Resolve())));

        public TOutput HandleServicesContainer<TOutput>(Func<IAuthenticationService, TOutput> func) =>
            Handle(state => func(new AuthenticationService(factoryProxy.CreateRepositoryAdminkaDbContextHandlerContainer(state).Resolve())));

        public async Task<TOutput> HandleServicesContainerAsync<TOutput>(Func<IAuthenticationService, TOutput> func) =>
            await HandleAsync(state => func(new AuthenticationService(factoryProxy.CreateRepositoryAdminkaDbContextHandlerContainer(state).Resolve())));

        #endregion
    }
}