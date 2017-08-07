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
        private readonly Func<RoutineGuid, IContainer, RoutineLoggingTransients> loggingTransientsFactory;
        private readonly RepositoryHandlerFactory repositoryHandlerFactory;
        private readonly UserContext systemUserContext;
        private readonly ConfigurationContainerFactory configurationContainerFactory;
        public AuthenticationService(
            Func<RoutineGuid, IContainer, RoutineLoggingTransients> loggingTransientsFactory,
            RepositoryHandlerFactory repositoryHandlerFactory,
            ConfigurationContainerFactory configurationContainerFactory)
        {
            this.loggingTransientsFactory = loggingTransientsFactory;
            this.repositoryHandlerFactory = repositoryHandlerFactory;
            systemUserContext = new UserContext("Authentication");
            this.configurationContainerFactory = configurationContainerFactory;
        }

        public UserContext GetUserContext(RoutineGuid routineGuid, IIdentity identity, CultureInfo cultureInfo)
        {
            var authenticationRoutineGuid = new RoutineGuid(routineGuid.CorrelationToken, new MemberTag(this));
            var specifyResolver = configurationContainerFactory.ComposeContainerFactory(authenticationRoutineGuid);
            var systemUserContextResolver = specifyResolver(systemUserContext);

            var adConfiguration = systemUserContextResolver.Resolve<AdConfiguration>();
            bool useAdAuthorization = adConfiguration.UseAdAuthorization;
            var input = new
            {
                PrincipalAuthenticationType = identity.AuthenticationType,
                PrincipalName = identity.Name,
                PrincipalIsAuthenticated = identity.IsAuthenticated,
                PrincipalType = identity.GetType().FullName,
                UseAdAuthorization = useAdAuthorization
            };
            var routine = new AdminkaRoutineHandler(
                authenticationRoutineGuid,
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
                        var fakeAdConfiguration = systemUserContextResolver.Resolve<FakeAdConfiguration>();
                        user = authenticationService.GetUser(fakeAdConfiguration.FakeAdUser, null, null, fakeAdConfiguration.FakeAdGroups);
                    }
                    var loggingTransients = loggingTransientsFactory(routineGuid, systemUserContextResolver);
                    loggingTransients.AuthenticationLoggingAdapter.TraceAuthentication(routineGuid, user.LoginName);
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