using Microsoft.AspNetCore.Http;
using System;
using System.Globalization;
using System.Linq;
using System.Security.Principal;
using Vse.AdminkaV1.Injected;
using Vse.AdminkaV1.Injected.Logging;
using Vse.Routines;

namespace Vse.AdminkaV1.Web.MvcCoreApp
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
            RoutineTag routineTag,
            IIdentity identity,
            CultureInfo cultureInfo,
            string connectionString,
            RepositoryHandlerFactory repositoryHandlerFactory,
            Func<RoutineTag, IResolver, RoutineLoggingTransients> loggingTransientsFactory,
            IAppConfiguration configuration
            )
        {
            // get userContextGuid from session
            // search in cash for userContextGuid

            UserContext userContext = null;
            var userJson = httpContext.Session.GetString("User");
            if (userJson != null)
            {
                var user = InjectedManager.DeserializeJson<DomAuthentication.User>(userJson);
                userContext = new UserContext(user);
            }
            else
            {
                var authenticationSerivce = new AuthenticationService(loggingTransientsFactory, repositoryHandlerFactory, configuration);
                userContext = authenticationSerivce.GetUserContext(routineTag, identity, cultureInfo);
                userJson = InjectedManager.SerializeToJson(userContext.User, 1, false);
                httpContext.Session.SetString("User", userJson);
            }
            return userContext;
        }
    }
}
