using System;
using System.Globalization;
using System.Security.Principal;
using Vse.AdminkaV1.DomAuthentication;
using Vse.AdminkaV1.Injected.Configuration;
using Vse.AdminkaV1.Injected.Logging;
using Vse.Routines;

namespace Vse.AdminkaV1.Injected
{
    public class AuthenticationService
    {
        private readonly Func<RoutineTag, IResolver, RoutineLoggingTransients> loggingTransientsFactory;
        private readonly RepositoryHandlerFactory repositoryHandlerFactory;
        private readonly UserContext systemUserContext;
        public AuthenticationService(
            Func<RoutineTag, IResolver, RoutineLoggingTransients> loggingTransientsFactory,
            RepositoryHandlerFactory repositoryHandlerFactory)
        {
            this.loggingTransientsFactory = loggingTransientsFactory;
            this.repositoryHandlerFactory = repositoryHandlerFactory;
            systemUserContext = new UserContext("Authentication");
        }

        public UserContext GetUserContext(RoutineTag routineTag, IIdentity identity, CultureInfo cultureInfo)
        {
            var authenticationRoutineTag = new RoutineTag(routineTag.CorrelationToken, this);
            Func<UserContext, IResolver> specifyResolver;
            var basicResolver = authenticationRoutineTag.GetResolver(out specifyResolver);
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
                        string firstName, secondName, loginName;
                        var groups = identity.GetGroups(out loginName, out firstName, out secondName);
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