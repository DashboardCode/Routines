using System;
using System.Linq;
using System.Globalization;
using System.Security.Principal;
using Microsoft.AspNetCore.Http;

using DashboardCode.Routines;
using DashboardCode.Routines.Logging;
using DashboardCode.Routines.Configuration;

using DashboardCode.AdminkaV1.DataAccessEfCore;
using DashboardCode.AdminkaV1.Injected.Logging;


namespace DashboardCode.AdminkaV1.Injected.AspCore.MvcApp
{
    public static class WebManager
    {
        public static Guid SetupCorrelationToken(this HttpContext httpContext)
        {
            var @value = default(Guid);
            var correlationToken = httpContext.Request.Headers["X-CorrelationToken"].FirstOrDefault();
            if (correlationToken == null)
            {
                @value = Guid.NewGuid();
                httpContext.Request.Headers.Add("X-CorrelationToken", @value.ToString());
            }
            else
            {
                @value = Guid.Parse(correlationToken);
            }
            return @value;
        }

        // TODO: add user context to session,
        // for this there should be service that tracks user privileges changes 
        // in db and reset sessions if there are changes
        public static UserContext GetUserContext(
            HttpContext httpContext,
            MemberTag   memberTag,
            IIdentity   identity,
            CultureInfo cultureInfo,
            string connectionString,
            AdminkaStorageConfiguration adminkaStorageConfiguration,
            AdminkaRoutineLogger routineLogger,
            Func<Guid, MemberTag, (IMemberLogger, IAuthenticationLogging)> loggingTransientsFactory,
            ContainerFactory<UserContext> configurationContainerFactory
            )
        {
            // get userContextGuid from session
            // search in cash for userContextGuid

            UserContext userContext = null;
            var userJson = httpContext.Session.GetString("User");
            if (userJson != null)
            {
                var user = InjectedManager.DeserializeJson<AuthenticationDom.User>(userJson);
                userContext = new UserContext(user);
            }
            else
            {
                //var authenticationSerivce = new UserContextFactory(
                //    loggingTransientsFactory,
                //    storageRoutineTransitions,
                //    adminkaStorageConfiguration,
                //    configurationContainerFactory);
                //userContext = authenticationSerivce.Create(routineTag, identity, cultureInfo);

                userContext = InjectedManager.GetUserContext(
                    routineLogger,
                    loggingTransientsFactory,
                    adminkaStorageConfiguration,
                    configurationContainerFactory,
                    memberTag, 
                    identity, CultureInfo.CurrentCulture
                    );

                userJson = InjectedManager.SerializeToJson(userContext.User, 1, false);
                httpContext.Session.SetString("User", userJson);
            }
            return userContext;
        }
    }
}