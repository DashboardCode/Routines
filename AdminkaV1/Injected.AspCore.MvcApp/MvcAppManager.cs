using System;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.AspNetCore.Mvc.RazorPages;

using DashboardCode.Routines;
using DashboardCode.Routines.AspNetCore;
using DashboardCode.Routines.Configuration;
using DashboardCode.AdminkaV1.AuthenticationDom;
using DashboardCode.AdminkaV1.Injected.ActiveDirectory;
using DashboardCode.AdminkaV1.DataAccessEfCore.Services;

namespace DashboardCode.AdminkaV1.Injected.AspCore.MvcApp
{
    public static class MvcAppManager
    {
        public static ComplexRoutineHandler<PerCallContainer<TUserContext>, TUserContext> GetContainerHandler<TUserContext>(
                    ContainerFactory containerFactory,
                    MemberTag memberTag,
                    AspRoutineFeature aspRoutineFeature,
                    Func<object> getInput,
                    TUserContext userContext,
                    ApplicationSettings applicationSettings,
                    Func<TUserContext, string> getConfigurationFor,
                    Func<TUserContext, string> getAuditStamp
            )
        {
            return new AdminkaRoutineHandlerBase<TUserContext>(
                    applicationSettings,
                    userContext,
                    getAuditStamp: getAuditStamp,
                    input: getInput(),
                    correlationToken: aspRoutineFeature.CorrelationToken,
                    traceDocumentBuilder: aspRoutineFeature.TraceDocument.Builder,
                    hasVerboseLoggingPrivilege: false,
                    configurationFor: getAuditStamp(userContext),
                    memberTag: memberTag
                );
        }

        // used on razor pages AD authorisation
        public static Task<(IActionResult forbiddenActionResult, User user, ContainerFactory containerFactory)>
            GetUserAndFailedActionResultInitialisedAsync(
                ApplicationSettings applicationSettings,
                MemberTag memberTag,
                PageModel pageModel,
                AspRoutineFeature aspRoutineFeature,
                IMemoryCache memoryCache
            )
        {
            return GetUserAndFailedActionResultInitialisedAsync(applicationSettings, memberTag,
                    getName: () => pageModel.User.Identity.Name,
                    systemIsInRole: g => pageModel.User.IsInRole(g),
                    aspRoutineFeature,
                    memoryCache,
                    getForbiddenActionResult: () => pageModel.RedirectToPage(
                    "AccessDenied",
                    new {/* area = null,*/ returnUrl = pageModel.Request.Path + pageModel.Request.QueryString }),
                    exceptionToActionResult: null
                );
        }

        public static async Task<(IActionResult forbiddenActionResult, User user, ContainerFactory containerFactory)>
            GetUserAndFailedActionResultInitialisedAsync(
                ApplicationSettings applicationSettings,
                MemberTag memberTag,
                Func<string> getName,
                Func<string, bool> systemIsInRole,
                //Func<IReadOnlyList<string>> getGroups,
                //Func<(string givenName, string surname)> getUserData,
                AspRoutineFeature aspRoutineFeature,
                IMemoryCache memoryCache,
                Func<IActionResult> getForbiddenActionResult,
                Func<Exception, IActionResult> exceptionToActionResult = null
            )
        {

            var containerFactory = InjectedManager.CreateContainerFactory(applicationSettings.ConfigurationContainerFactory);
            Func<string, IContainer> getContainer = @for => containerFactory.CreateContainer(memberTag, @for);

            var routine = new AdminkaAnonymousRoutineHandler(
                applicationSettings,
                applicationSettings.PerformanceCounters,
                applicationSettings.ConfigurationContainerFactory,
                InjectedManager.DefaultRoutineTagTransformException,
                aspRoutineFeature.CorrelationToken,
                aspRoutineFeature.TraceDocument.Builder,
                new MemberTag("MvcAppManager", "GetUserAndFailedActionResultInitialisedAsync"),
                new AnonymousUserContext("Authentication"),
                null);
            IActionResult forbiddenAsActionResult = default;

            var user = await routine.HandleAsync(async (container, closure) =>
            {
                try
                {
                    var useAdAuthorization = applicationSettings.UseAdAuthorization;
                    var isInRole = default(Func<string, bool>);
                    var userNameWithDomain = default(string);
                    
                    if (useAdAuthorization)
                    {
                        userNameWithDomain = getName();
                        var personalContainer = getContainer(userNameWithDomain);
                        var personalContainerGroups = personalContainer.Resolve<FakeAdConfiguration>().FakeAdGroups;
                        if (personalContainer == null || personalContainerGroups.Count > 0)
                            isInRole = g => personalContainerGroups.Contains(g);
                        else
                            isInRole = systemIsInRole;

                        closure.Verbose?.Invoke($"useAdAuthorization:{useAdAuthorization}, adUserName: {userNameWithDomain}");

                        var firstName = default(string);
                        var secondName = default(string);
                        //var (firstName, secondName) = ActiveDirectoryManager.GetUserData(windowsIdentity);

                        return await container.ResolveAdminkaDbContextHandler().HandleDbContextAsync(
                            async (db) =>
                            {
                                return await memoryCache.GetOrCreateAsync(
                                         "USER::" + userNameWithDomain,
                                         async cacheEntry =>
                                         {
                                             cacheEntry
                                                   .SetAbsoluteExpiration(TimeSpan.FromHours(5))
                                                   .SetSlidingExpiration(TimeSpan.FromMinutes(5));
                                             var authenticationService = new AuthenticationService(db);
                                             
                                             var u = await authenticationService.GetUserAsync(userNameWithDomain, firstName, secondName, isInRole);
                                             return u;
                                         }
                                    );
                            }
                        );
                    }
                    else
                    {
                        var fakeAdConfiguration = closure.Resolve<FakeAdConfiguration>();

                        userNameWithDomain = fakeAdConfiguration.FakeAdUser;
                        isInRole = g => fakeAdConfiguration.FakeAdGroups.Contains(g);

                        closure.Verbose?.Invoke($"useAdAuthorization:{useAdAuthorization}, adUserName: {userNameWithDomain}");

                        return await container.ResolveAdminkaDbContextHandler().HandleDbContextAsync(
                            async (db) =>
                            {
                                var authenticationService = new AuthenticationService(db);
                                var userEntity = await authenticationService.GetUserAsync(
                                          fakeAdConfiguration.FakeAdUser, "Anonymous", "Anonymous", isInRole);
                                return userEntity;
                            }
                        );
                    }
                }
                catch (Exception ex)
                {
                    if (exceptionToActionResult != null)
                    {
                        forbiddenAsActionResult = exceptionToActionResult(ex);
                        return null;
                    }
                    else
                    {
                        throw new AdminkaException("User authetication and authorization service generates an error because of configuration or network connection problems", ex);
                    }
                }
            });

            if (user == default && forbiddenAsActionResult == default)
            {
                forbiddenAsActionResult = getForbiddenActionResult();
            }

            return (forbiddenAsActionResult, user, containerFactory);
        }

        //public static Guid SetupCorrelationToken(this HttpContext httpContext)
        //{
        //    var @value = default(Guid);
        //    var correlationToken = httpContext.Request.Headers["X-CorrelationToken"].FirstOrDefault();
        //    if (correlationToken == null)
        //    {
        //        @value = Guid.NewGuid();
        //        httpContext.Request.Headers.Add("X-CorrelationToken", @value.ToString());
        //    }
        //    else
        //    {
        //        @value = Guid.Parse(correlationToken);
        //    }
        //    return @value;
        //}

        // TODO: add user context to session,
        // for this there should be service that tracks user privileges changes 
        // in db and reset sessions if there are changes

        //public static UserContext GetUserContext(
        //    HttpContext httpContext,
        //    MemberTag   memberTag,
        //    IIdentity   identity,
        //    CultureInfo cultureInfo,
        //    string connectionString,
        //    AdminkaStorageConfiguration adminkaStorageConfiguration,
        //    AdminkaRoutineLogger routineLogger,
        //    Func<Guid, MemberTag, (IMemberLogger, IAuthenticationLogging)> loggingTransientsFactory,
        //    ContainerFactory configurationContainerFactory
        //    )
        //{
        //    // get userContextGuid from session
        //    // search in cash for userContextGuid

        //    UserContext userContext = null;
        //    var userJson = httpContext.Session.GetString("User");
        //    if (userJson != null)
        //    {
        //        var user = InjectedManager.DeserializeJson<AuthenticationDom.User>(userJson);
        //        userContext = new UserContext(user);
        //    }
        //    else
        //    {
        //        //var authenticationSerivce = new UserContextFactory(
        //        //    loggingTransientsFactory,
        //        //    storageRoutineTransitions,
        //        //    adminkaStorageConfiguration,
        //        //    configurationContainerFactory);
        //        //userContext = authenticationSerivce.Create(routineTag, identity, cultureInfo);

        //        userContext = InjectedManager.GetUserContext(
        //            routineLogger,
        //            loggingTransientsFactory,
        //            adminkaStorageConfiguration,
        //            configurationContainerFactory,
        //            memberTag, 
        //            identity, CultureInfo.CurrentCulture
        //            );

        //        userJson = InjectedManager.SerializeToJson(userContext.User, 1, false);
        //        httpContext.Session.SetString("User", userJson);
        //    }
        //    return userContext;
        //}
    }
}