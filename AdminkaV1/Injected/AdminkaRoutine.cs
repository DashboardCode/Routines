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
            IAppConfiguration configuration,
            object input
        ) : this(
              new RoutineGuid(Guid.NewGuid(), memberTag),
              configuration,
              input)
        {
        }
        public AdminkaRoutine(
            RoutineGuid routineGuid,
            IAppConfiguration configuration,
            object input
        ) : this(
              routineGuid,
              InjectedManager.ComposeNLogTransients(InjectedManager.Markdown,InjectedManager.DefaultRoutineTagTransformException),
              new RepositoryHandlerFactory(InjectedManager.GetStorageMetaService(configuration)),
              configuration,
              input)
        {
        }
        public AdminkaRoutine(
            string @namespace, string controller, string action,
            Func<RoutineGuid, IResolver, RoutineLoggingTransients> loggingTransientsFactory,
            IAppConfiguration configuration,
            object input
        ) : this(
              new RoutineGuid(Guid.NewGuid(), @namespace, controller, action),
              loggingTransientsFactory,
              new RepositoryHandlerFactory(InjectedManager.GetStorageMetaService(configuration)),
              configuration,
              input)
        {
        }
        protected AdminkaRoutine(
             RoutineGuid routineGuid,
             IIdentity identity,
             Func<RoutineGuid, IResolver, RoutineLoggingTransients> loggingTransientsFactory,
             RepositoryHandlerFactory repositoryHandlerFactory,
             IAppConfiguration configuration,
             object input
        ) : this(
              routineGuid,
              identity,
              CultureInfo.CurrentCulture,
              loggingTransientsFactory,
              repositoryHandlerFactory,
              new AuthenticationService(loggingTransientsFactory, repositoryHandlerFactory, configuration),
              configuration,
              input)
        {
        }
        private AdminkaRoutine(
            RoutineGuid routineGuid,
            Func<RoutineGuid, IResolver, RoutineLoggingTransients> loggingTransientsFactory,
            RepositoryHandlerFactory repositoryHandlerFactory,
            IAppConfiguration configuration,
            object input
        ) : this(
              routineGuid,
              InjectedManager.GetDefaultIdentity(), 
              CultureInfo.CurrentCulture,
              loggingTransientsFactory,
              repositoryHandlerFactory,
              new AuthenticationService(loggingTransientsFactory, repositoryHandlerFactory, configuration),
              configuration,
              input)
        {
        }
        private AdminkaRoutine(
            RoutineGuid routineGuid,
            IIdentity identity,
            CultureInfo cultureInfo,
            Func<RoutineGuid, IResolver, RoutineLoggingTransients> loggingTransientsFactory,
            RepositoryHandlerFactory repositoryHandlerFactory,
            AuthenticationService authenticationServices,
            IAppConfiguration configuration,
            object input
            ) : this(
                  routineGuid,
                  authenticationServices.GetUserContext(routineGuid, identity, cultureInfo),
                  loggingTransientsFactory, 
                  repositoryHandlerFactory,
                  configuration,
                  input)
        {
        }
        #endregion

        #region constructors with usercontext
        public AdminkaRoutine(
            string @namespace, string controller, string action,
            UserContext userContext,
            IAppConfiguration configuration,
            object input
            ) : this(
                    new RoutineGuid(Guid.NewGuid(), @namespace, controller, action), 
                    userContext,
                    InjectedManager.ComposeNLogTransients(InjectedManager.Markdown,InjectedManager.DefaultRoutineTagTransformException),
                    new RepositoryHandlerFactory(InjectedManager.GetStorageMetaService(configuration)),
                    configuration,
                    input)
        {
        }
        public AdminkaRoutine(
            MemberTag memberTag,
            UserContext userContext,
            IAppConfiguration configuration,
            object input
            ) : this(new RoutineGuid(Guid.NewGuid(), memberTag),
                    userContext,
                    configuration,
                    input)
        {
        }
        public AdminkaRoutine(
            RoutineGuid routineGuid,
            UserContext userContext,
            IAppConfiguration configuration,
            object input
            ) : this(routineGuid,
                    userContext,
                    routineGuid.GetSpecifiedResolver(userContext, configuration),
                    InjectedManager.ComposeNLogTransients(InjectedManager.Markdown,InjectedManager.DefaultRoutineTagTransformException),
                    new RepositoryHandlerFactory(InjectedManager.GetStorageMetaService(configuration)),
                    input)
        {
        }
        public AdminkaRoutine(
            string @namespace, string controller, string action,
            UserContext userContext,
            Func<RoutineGuid, IResolver, RoutineLoggingTransients> loggingTransientsFactory,
            IAppConfiguration configuration,
            object input
            ) : this(
                    new RoutineGuid(Guid.NewGuid(), @namespace, controller, action), 
                    userContext, 
                    loggingTransientsFactory,
                    new RepositoryHandlerFactory(InjectedManager.GetStorageMetaService(configuration)),
                    configuration,
                    input)
        {
        }
        protected AdminkaRoutine(
            RoutineGuid routineGuid,
            UserContext userContext,
            Func<Exception, RoutineGuid, Func<Exception, string>, Exception> routineTransformException,
            IAppConfiguration configuration,
            object input
        ) : this(
           routineGuid,
           userContext,
           routineGuid.GetSpecifiedResolver(userContext, configuration),
           InjectedManager.ComposeNLogTransients(InjectedManager.Markdown,routineTransformException),
           new RepositoryHandlerFactory(InjectedManager.GetStorageMetaService(configuration)),
           input)
        {
        }
        protected AdminkaRoutine(
            RoutineGuid routineGuid,
            UserContext userContext,
            Func<RoutineGuid, IResolver, RoutineLoggingTransients> loggingTransientsFactory,
            RepositoryHandlerFactory repositoryHandlerFactory,
            IAppConfiguration configuration,
            object input
            ) : this(
                routineGuid,
                userContext,
                routineGuid.GetSpecifiedResolver(userContext, configuration),
                loggingTransientsFactory,
                repositoryHandlerFactory,
                input)
        {
        }
        #endregion

        internal AdminkaRoutine(
            RoutineGuid routineGuid,
            UserContext userContext,
            IResolver reslover,
            Func<RoutineGuid, IResolver, RoutineLoggingTransients> loggingTransientsFactory,
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
            IResolver resolver,
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
