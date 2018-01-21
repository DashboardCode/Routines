using System;
using System.Globalization;
using System.Security.Principal;

using DashboardCode.Routines;
using DashboardCode.AdminkaV1.AuthenticationDom;
using DashboardCode.AdminkaV1.DataAccessEfCore;
using DashboardCode.AdminkaV1.Injected.Configuration;
using DashboardCode.AdminkaV1.Injected.Logging;

namespace DashboardCode.AdminkaV1.Injected
{
    public class UserContextFactory
    {
        private readonly Func<RoutineGuid, IContainer, RoutineLoggingTransients> loggingTransientsFactory;
        private readonly FactoryProxy factoryProxy2;
        private readonly UserContext systemUserContext;
        private readonly ContainerFactory configurationContainerFactory;
        public UserContextFactory(
            Func<RoutineGuid, IContainer, RoutineLoggingTransients> loggingTransientsFactory,
            FactoryProxy ormHandlerFactory,
            ContainerFactory configurationContainerFactory)
        {
            this.loggingTransientsFactory = loggingTransientsFactory;
            this.factoryProxy2 = ormHandlerFactory;
            systemUserContext = new UserContext("Authentication");
            this.configurationContainerFactory = configurationContainerFactory;
        }

        public UserContext Create(RoutineGuid routineGuid, IIdentity identity, CultureInfo cultureInfo)
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
                factoryProxy2,
                input);
            var userContext = routine.Handle(closure =>
            {
                try
                {
                    var adminkaDbContextPrimitiveHandler = factoryProxy2.CreateRepositoryAdminkaDbContextHandlerContainer(closure).Resolve(); 
                    var authenticationService = new DataAccessEfCore.Services.AuthenticationService(adminkaDbContextPrimitiveHandler); 
                    var user = default(User);
                    if (useAdAuthorization)
                    {
                        var groups = InjectedManager.GetGroups(identity, out string loginName, out string firstName, out string secondName);
                        user = authenticationService.GetUser(loginName, firstName, secondName, groups);
                    }
                    else
                    {
                        var fakeAdConfiguration = systemUserContextResolver.Resolve<FakeAdConfiguration>();
                        user = authenticationService.GetUser(fakeAdConfiguration.FakeAdUser, "Anonymous", "Anonymous", fakeAdConfiguration.FakeAdGroups);
                    }
                    var loggingTransients = loggingTransientsFactory(routineGuid, systemUserContextResolver);
                    loggingTransients.AuthenticationLoggingAdapter.TraceAuthentication(routineGuid, user.LoginName);
                    return new UserContext(user, cultureInfo);
                }
                catch (Exception ex)
                {
                    throw new AdminkaDataAccessEfCoreException("User authetication and authorization service generates an error because of configuration or network connection problems", closure, ex);
                }
            });
            return userContext;
        }
    }
}