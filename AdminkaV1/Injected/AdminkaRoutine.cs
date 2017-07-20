using System;
using System.Security.Principal;
using System.Threading.Tasks;
using System.Globalization;
using Vse.Routines;
using Vse.Routines.Injected;
using Vse.AdminkaV1.Injected.Logging;

namespace Vse.AdminkaV1.Injected
{
    public class AdminkaRoutine : UserRoutine<UserContext>
    {
        readonly RepositoryHandlerFactory repositoryHandlerFactory;
        protected readonly UserContext userContext;
        #region constructors without usercontext
        public AdminkaRoutine(
            RoutineTag routineTag,
            IAppConfiguration configuration,
            object input
        ) : this(
            routineTag,
            InjectedManager.ComposeNLogTransients(InjectedManager.Markdown,InjectedManager.DefaultRoutineTagTransformException),
            new RepositoryHandlerFactory(InjectedManager.GetStorageMetaService(configuration)),
            configuration,
            input)
        {
        }
        public AdminkaRoutine(
            string @namespace, string controller, string action,
            Func<RoutineTag, IResolver, RoutineLoggingTransients> loggingTransientsFactory,
            IAppConfiguration configuration,
            object input
        ) : this(
           new RoutineTag(Guid.NewGuid(), @namespace, controller, action),
           loggingTransientsFactory,
           new RepositoryHandlerFactory(InjectedManager.GetStorageMetaService(configuration)),
           configuration,
           input)
        {
        }

        protected AdminkaRoutine(
             RoutineTag routineTag,
             IIdentity identity,
             Func<RoutineTag, IResolver, RoutineLoggingTransients> loggingTransientsFactory,
             RepositoryHandlerFactory repositoryHandlerFactory,
             IAppConfiguration configuration,
             object input
        ) : this(
           routineTag,
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
            RoutineTag routineTag,
            Func<RoutineTag, IResolver, RoutineLoggingTransients> loggingTransientsFactory,
            RepositoryHandlerFactory repositoryHandlerFactory,
            IAppConfiguration configuration,
            object input
        ) : this(
           routineTag,
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
            RoutineTag routineTag,
            IIdentity identity,
            CultureInfo cultureInfo,
            Func<RoutineTag, IResolver, RoutineLoggingTransients> loggingTransientsFactory,
            RepositoryHandlerFactory repositoryHandlerFactory,
            AuthenticationService authenticationServices,
            IAppConfiguration configuration,
            object input
            ) : this(
                routineTag,
                authenticationServices.GetUserContext(routineTag, identity, cultureInfo),
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
                    new RoutineTag(Guid.NewGuid(), @namespace, controller, action), 
                    userContext,
                    InjectedManager.ComposeNLogTransients(InjectedManager.Markdown,InjectedManager.DefaultRoutineTagTransformException),
                    new RepositoryHandlerFactory(InjectedManager.GetStorageMetaService(configuration)),
                    configuration,
                    input)
        {
        }

        public AdminkaRoutine(
            RoutineTag routineTag,
            UserContext userContext,
            IAppConfiguration configuration,
            object input
            ) : this(routineTag,
                    userContext,
                    routineTag.GetSpecifiedResolver(userContext, configuration),
                    InjectedManager.ComposeNLogTransients(InjectedManager.Markdown,InjectedManager.DefaultRoutineTagTransformException),
                    new RepositoryHandlerFactory(InjectedManager.GetStorageMetaService(configuration)),
                    input)
        {
        }

        public AdminkaRoutine(
            string @namespace, string controller, string action,
            UserContext userContext,
            Func<RoutineTag, IResolver, RoutineLoggingTransients> loggingTransientsFactory,
            IAppConfiguration configuration,
            object input
            ) : this(
                    new RoutineTag(Guid.NewGuid(), @namespace, controller, action), 
                    userContext, 
                    loggingTransientsFactory,
                    new RepositoryHandlerFactory(InjectedManager.GetStorageMetaService(configuration)),
                    configuration,
                    input)
        {
        }

        protected AdminkaRoutine(
            RoutineTag routineTag,
            UserContext userContext,
            Func<Exception, RoutineTag, Func<Exception, string>, Exception> routineTransformException,
            IAppConfiguration configuration,
            object input
        ) : this(
           routineTag,
           userContext,
           routineTag.GetSpecifiedResolver(userContext, configuration),
           InjectedManager.ComposeNLogTransients(InjectedManager.Markdown,routineTransformException),
           new RepositoryHandlerFactory(InjectedManager.GetStorageMetaService(configuration)),
           input)
        {
        }

        protected AdminkaRoutine(
            RoutineTag routineTag,
            UserContext userContext,
            Func<RoutineTag, IResolver, RoutineLoggingTransients> loggingTransientsFactory,
            RepositoryHandlerFactory repositoryHandlerFactory,
            IAppConfiguration configuration,
            object input
            ) : this(
                routineTag,
                userContext,
                routineTag.GetSpecifiedResolver(userContext, configuration),
                loggingTransientsFactory,
                repositoryHandlerFactory,
                input)
        {
        }
        #endregion

        internal AdminkaRoutine(
            RoutineTag routineTag,
            UserContext userContext,
            IResolver reslover,
            Func<RoutineTag, IResolver, RoutineLoggingTransients> loggingTransientsFactory,
            RepositoryHandlerFactory repositoryHandlerFactory,
            object input
            ) : this(
                routineTag, 
                userContext, 
                reslover,
                loggingTransientsFactory(routineTag, reslover),
                repositoryHandlerFactory, 
                input)
        {
        }
        internal AdminkaRoutine(
            RoutineTag routineTag,
            UserContext userContext,
            IResolver resolver,
            RoutineLoggingTransients routineLoggingTransients,
            RepositoryHandlerFactory repositoryHandlerFactory,
            object input
            ) : base(
                routineLoggingTransients.BasicRoutineLoggingAdapter,
                routineLoggingTransients.TransformException,
                (verbose) => new RoutineState<UserContext>(userContext, routineTag, verbose, resolver),
                repositoryHandlerFactory,
                input
                )
        {
            this.userContext = userContext;
            this.repositoryHandlerFactory = repositoryHandlerFactory;
        }

        #region Handle with data access
        public void Handle(Action<RoutineState<UserContext>, DataAccessFactory> action)
        {
            Handle(state =>
            {
                action(state, repositoryHandlerFactory.CreateDataAccessFactory(state));
            });
        }
        public TOutput Handle<TOutput>(Func<RoutineState<UserContext>, DataAccessFactory, TOutput> func)
        {
            return Handle(state =>
            {
                return func(state, repositoryHandlerFactory.CreateDataAccessFactory(state));
            });
        }
        public async Task<TOutput> HandleAsync<TOutput>(Func<RoutineState<UserContext>, DataAccessFactory, TOutput> func)
        {
            return await HandleAsync(state =>
            {
                return func(state, repositoryHandlerFactory.CreateDataAccessFactory(state));
            });
        }
        #endregion
    }
}
