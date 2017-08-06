using System;
using System.Security.Principal;
using System.Threading.Tasks;
using System.Globalization;
using DashboardCode.Routines;
using DashboardCode.Routines.Injected;
using DashboardCode.AdminkaV1.Injected.Logging;

namespace DashboardCode.AdminkaV1.Injected
{
    public class AdminkaRoutine : UserRoutine<UserContext>
    {
        readonly RepositoryHandlerFactory repositoryHandlerFactory;
        protected readonly UserContext userContext;
        #region constructors without usercontext
        public AdminkaRoutine(
            MemberTag memberTag,
            IApplicationFactory applicationFactory,
            object input
        ) : this(
              new RoutineGuid(Guid.NewGuid(), memberTag),
              InjectedManager.GetDefaultIdentity(),
              InjectedManager.ComposeNLogTransients(InjectedManager.Markdown, InjectedManager.DefaultRoutineTagTransformException),
              new ConfigurationContainerFactory(applicationFactory),
              input)
        {
        }
        public AdminkaRoutine(
            RoutineGuid routineGuid,
            IApplicationFactory applicationFactory,
            object input
        ) : this(
              routineGuid,
              InjectedManager.GetDefaultIdentity(),
              InjectedManager.ComposeNLogTransients(InjectedManager.Markdown, InjectedManager.DefaultRoutineTagTransformException),
              new ConfigurationContainerFactory(applicationFactory),
              input)
        {
        }
        public AdminkaRoutine(
            string @namespace, string controller, string action,
            Func<RoutineGuid, IContainer, RoutineLoggingTransients> loggingTransientsFactory,
            IApplicationFactory applicationFactory,
            object input
        ) : this(
              new RoutineGuid(Guid.NewGuid(), @namespace, controller, action),
              InjectedManager.GetDefaultIdentity(),
              loggingTransientsFactory,
              new ConfigurationContainerFactory(applicationFactory),
              input)
        {
        }
        public AdminkaRoutine(
            RoutineGuid routineGuid,
            IIdentity identity,
            Func<RoutineGuid, IContainer, RoutineLoggingTransients> loggingTransientsFactory,
            IApplicationFactory applicationFactory,
            object input
        ) : this(
              routineGuid,
              identity,
              loggingTransientsFactory,
              new ConfigurationContainerFactory(applicationFactory),
              input)
        {
        }
        protected AdminkaRoutine(
             RoutineGuid routineGuid,
             IIdentity identity,
             Func<RoutineGuid, IContainer, RoutineLoggingTransients> loggingTransientsFactory,
             ConfigurationContainerFactory configurationContainerFactory,
             object input
        ) : this(
              routineGuid,
              identity,
              loggingTransientsFactory,
              configurationContainerFactory.ResolveRepositoryHandlerFactory(),
              configurationContainerFactory,
              input)
        {
        }
        private AdminkaRoutine(
             RoutineGuid routineGuid,
             IIdentity identity,
             Func<RoutineGuid, IContainer, RoutineLoggingTransients> loggingTransientsFactory,
             RepositoryHandlerFactory repositoryHandlerFactory,
             ConfigurationContainerFactory configurationContainerFactory,
             object input
        ) : this(
              routineGuid,
              identity,
              CultureInfo.CurrentCulture,
              loggingTransientsFactory,
              repositoryHandlerFactory,
              new AuthenticationService(loggingTransientsFactory, repositoryHandlerFactory, configurationContainerFactory),
              configurationContainerFactory,
              input)
        {
        }
        private AdminkaRoutine(
            RoutineGuid routineGuid,
            Func<RoutineGuid, IContainer, RoutineLoggingTransients> loggingTransientsFactory,
            RepositoryHandlerFactory repositoryHandlerFactory,
            ConfigurationContainerFactory configurationContainerFactory,
            object input
        ) : this(
              routineGuid,
              InjectedManager.GetDefaultIdentity(), 
              CultureInfo.CurrentCulture,
              loggingTransientsFactory,
              repositoryHandlerFactory,
              new AuthenticationService(loggingTransientsFactory, repositoryHandlerFactory, configurationContainerFactory),
              configurationContainerFactory,
              input)
        {
        }
        private AdminkaRoutine(
            RoutineGuid routineGuid,
            IIdentity identity,
            CultureInfo cultureInfo,
            Func<RoutineGuid, IContainer, RoutineLoggingTransients> loggingTransientsFactory,
            RepositoryHandlerFactory repositoryHandlerFactory,
            AuthenticationService authenticationServices,
            ConfigurationContainerFactory configurationContainerFactory,
            object input
            ) : this(
                  routineGuid,
                  authenticationServices.GetUserContext(routineGuid, identity, cultureInfo),
                  loggingTransientsFactory, 
                  repositoryHandlerFactory,
                  configurationContainerFactory,
                  input)
        {
        }
        #endregion

        #region constructors with usercontext
        public AdminkaRoutine(
            string @namespace, string controller, string action,
            UserContext userContext,
            IApplicationFactory configuration,
            object input
            ) : this(
                    new RoutineGuid(Guid.NewGuid(), @namespace, controller, action),
                    userContext,
                    new ConfigurationContainerFactory(configuration),
                    input)
        {
        }
        public AdminkaRoutine(
            string @namespace, string controller, string action,
            UserContext userContext,
            Func<RoutineGuid, IContainer, RoutineLoggingTransients> loggingTransientsFactory,
            IApplicationFactory configuration,
            object input
            ) : this(
                    @namespace, controller, action,
                    userContext,
                    loggingTransientsFactory,
                    new ConfigurationContainerFactory(configuration),
                    input)
        {
        }
        public AdminkaRoutine(
            MemberTag memberTag,
            UserContext userContext,
            IApplicationFactory configuration,
            object input
            ) : this(new RoutineGuid(Guid.NewGuid(), memberTag),
                    userContext,
                    new ConfigurationContainerFactory(configuration),
                    input)
        {
        }
        public AdminkaRoutine(
            RoutineGuid routineGuid,
            UserContext userContext,
            IApplicationFactory configuration,
            object input
            ) : this(routineGuid,
            userContext,
            new ConfigurationContainerFactory(configuration),
            input)
        {
        }

        public AdminkaRoutine(
            RoutineGuid routineGuid,
            UserContext userContext,
            Func<Exception, RoutineGuid, Func<Exception, string>, Exception> routineTransformException,
            IApplicationFactory configuration,
            object input
            ) : this(routineGuid,
            userContext,
            routineTransformException,
            new ConfigurationContainerFactory(configuration),
            input)
        {
        }

        public AdminkaRoutine(
            RoutineGuid routineGuid,
            UserContext userContext,
            ConfigurationContainerFactory configurationContainerFactory,
            object input
            ) : this(routineGuid,
                    userContext,
                    configurationContainerFactory.CreateContainer(routineGuid, userContext),
                    InjectedManager.ComposeNLogTransients(InjectedManager.Markdown,InjectedManager.DefaultRoutineTagTransformException),
                    configurationContainerFactory.ResolveRepositoryHandlerFactory(),
                    input)
        {
        }

        public AdminkaRoutine(
            string @namespace, string controller, string action,
            UserContext userContext,
            Func<RoutineGuid, IContainer, RoutineLoggingTransients> loggingTransientsFactory,
            ConfigurationContainerFactory configurationContainerFactory,
            object input
            ) : this(
                    new RoutineGuid(Guid.NewGuid(), @namespace, controller, action), 
                    userContext, 
                    loggingTransientsFactory,
                    configurationContainerFactory.ResolveRepositoryHandlerFactory(),
                    configurationContainerFactory,
                    input)
        {
        }

        protected AdminkaRoutine(
            RoutineGuid routineGuid,
            UserContext userContext,
            Func<Exception, RoutineGuid, Func<Exception, string>, Exception> routineTransformException,
            ConfigurationContainerFactory configurationContainerFactory,
            object input
        ) : this(
           routineGuid,
           userContext,
           configurationContainerFactory.CreateContainer(routineGuid, userContext),
           InjectedManager.ComposeNLogTransients(InjectedManager.Markdown,routineTransformException),
           configurationContainerFactory.ResolveRepositoryHandlerFactory(),
           input)
        {
        }

        protected AdminkaRoutine(
            RoutineGuid routineGuid,
            UserContext userContext,
            Func<RoutineGuid, IContainer, RoutineLoggingTransients> loggingTransientsFactory,
            RepositoryHandlerFactory repositoryHandlerFactory,
            ConfigurationContainerFactory configurationContainerFactory,
            object input
            ) : this(
                routineGuid,
                userContext,
                configurationContainerFactory.CreateContainer(routineGuid,  userContext),
                loggingTransientsFactory,
                repositoryHandlerFactory,
                input)
        {
        }
        #endregion

        internal AdminkaRoutine(
            RoutineGuid routineGuid,
            UserContext userContext,
            IContainer reslover,
            Func<RoutineGuid, IContainer, RoutineLoggingTransients> loggingTransientsFactory,
            RepositoryHandlerFactory repositoryHandlerFactory,
            object input
            ) : this(
                routineGuid, 
                userContext, 
                reslover,
                loggingTransientsFactory(routineGuid, reslover),
                repositoryHandlerFactory, 
                input)
        {
        }
        internal AdminkaRoutine(
            RoutineGuid routineGuid,
            UserContext userContext,
            IContainer resolver,
            RoutineLoggingTransients routineLoggingTransients,
            RepositoryHandlerFactory repositoryHandlerFactory,
            object input
            ) : base(
                routineLoggingTransients.BasicRoutineLoggingAdapter,
                routineLoggingTransients.TransformException,
                verbose => new Routine<UserContext>(userContext, routineGuid, verbose, resolver),
                repositoryHandlerFactory,
                input
                )
        {
            this.userContext = userContext;
            this.repositoryHandlerFactory = repositoryHandlerFactory;
        }


        #region Handle with data access
        public void Handle(Action<Routine<UserContext>, DataAccessFactory> action) =>
            Handle(state => action(state, repositoryHandlerFactory.CreateDataAccessFactory(state)));

        public TOutput Handle<TOutput>(Func<Routine<UserContext>, DataAccessFactory, TOutput> func) =>
            Handle(state =>func(state, repositoryHandlerFactory.CreateDataAccessFactory(state)));
        
        public async Task<TOutput> HandleAsync<TOutput>(Func<Routine<UserContext>, DataAccessFactory, TOutput> func) =>
            await HandleAsync(state => func(state, repositoryHandlerFactory.CreateDataAccessFactory(state)));
        #endregion
    }
}
