using System;
using System.Globalization;
using System.Security.Principal;
using DashboardCode.AdminkaV1.DomAuthentication;
using DashboardCode.AdminkaV1.Injected.Configuration;
using DashboardCode.AdminkaV1.Injected.Logging;
using DashboardCode.Routines;
using DashboardCode.Routines.Configuration;

namespace DashboardCode.AdminkaV1.Injected
{
    public class AuthenticationService
    {
        private readonly Func<MemberGuid, IResolver, RoutineLoggingTransients> loggingTransientsFactory;
        private readonly RepositoryHandlerFactory repositoryHandlerFactory;
        private readonly UserContext systemUserContext;
        private readonly IAppConfiguration appConfiguration;
        public AuthenticationService(
            Func<MemberGuid, IResolver, RoutineLoggingTransients> loggingTransientsFactory,
            RepositoryHandlerFactory repositoryHandlerFactory,
            IAppConfiguration appConfiguration)
        {
            this.loggingTransientsFactory = loggingTransientsFactory;
            this.repositoryHandlerFactory = repositoryHandlerFactory;
            this.appConfiguration = appConfiguration;
            systemUserContext = new UserContext("Authentication");
        }

        public UserContext GetUserContext(MemberGuid routineTag, IIdentity identity, CultureInfo cultureInfo)
        {
            var authenticationRoutineTag = new MemberGuid(routineTag.CorrelationToken, this);
            var basicResolver = authenticationRoutineTag.GetResolver(appConfiguration, out Func<UserContext, IResolver> specifyResolver);
            var resolver = specifyResolver(systemUserContext);
            var adConfiguration = resolver.Resolve<AdConfiguration>();
            bool useAdAuthorization = adConfiguration.UseAdAuthorization;
            var input = new
            {
                PrincipalAuthenticationType = identity.AuthenticationType,
                PrincipalName = identity.Name,
                PrincipalIsAuthenticated = identity.IsAuthenticated,
                PrincipalType = identity.GetType().FullName,
                UseAdAuthorization = useAdAuthorization
            };
            var systemUserContextResolver = specifyResolver(systemUserContext);
            var routine = new AdminkaRoutine(
                authenticationRoutineTag,
                systemUserContext,
                systemUserContextResolver,
                loggingTransientsFactory,
                repositoryHandlerFactory,
                input);
            var userContext = routine.Handle(state =>
            {
                try
                {
                    
                    var dataAccessServices = repositoryHandlerFactory.CreateDataAccessFactory(state); 
                    var dbContextManager = dataAccessServices.CreateDbContextHandler();
                    var servicesContainer = new ServicesContainer(dbContextManager);
                    var authenticationService = new DataAccessEfCore.Services.AuthenticationService(dbContextManager); 
                    var user = default(User);
                    if (useAdAuthorization)
                    {
                        var groups = InjectedManager.GetGroups(identity, out string loginName, out string firstName, out string secondName);
                        user = authenticationService.GetUser(loginName, firstName, secondName, groups);
                    }
                    else
                    {
                        var fakeAdConfiguration = resolver.Resolve<FakeAdConfiguration>();
                        user = authenticationService.GetUser(fakeAdConfiguration.FakeAdUser, null, null, fakeAdConfiguration.FakeAdGroups);
                    }
                    var loggingTransients = loggingTransientsFactory(routineTag, resolver);
                    loggingTransients.AuthenticationLoggingAdapter.TraceAuthentication(routineTag, user.LoginName);
                    return new UserContext(user, cultureInfo);
                }
                catch (Exception ex)
                {
                    throw new UserContextException("User authetication and authorization service generates an error because of configuration or network connection problems", ex);
                }
            });
            return userContext;
        }
    }
}