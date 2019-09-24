using System;
using System.Linq;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Http.Extensions;
using Newtonsoft.Json;

using DashboardCode.Routines;
using DashboardCode.Routines.Storage;
using DashboardCode.Routines.AspNetCore;
using DashboardCode.Routines.Configuration;

using DashboardCode.AdminkaV1.AuthenticationDom;
using DashboardCode.AdminkaV1.Injected.ActiveDirectory;
using DashboardCode.AdminkaV1.Injected.Logging;
using Microsoft.AspNetCore.WebUtilities;
using DashboardCode.AdminkaV1.AuthenticationDom.DataAccessEfCore.Services;
using DashboardCode.AdminkaV1.AuthenticationDom.DataAccessEfCore;

namespace DashboardCode.AdminkaV1.Injected.AspCore.MvcApp
{
    public static class MvcAppManager
    {
        public static string GetErrorActionJson(Exception ex, string aspRequestId, bool isAdminPrivilege)
        {
            string content;
            if (isAdminPrivilege)
            {
                var markdownMessage = InjectedManager.Markdown(ex);
                var htmlMessage = InjectedManager.ToHtmlException(markdownMessage);
                var source = new { isAdminPrivilege = true, aspRequestId, htmlMessage };
                content = JsonConvert.SerializeObject(source);
            }
            else
            {
                var source = new { isAdminPrivilege = false, message = "There was been a problem with the website. We are working on resolving it." };
                content = JsonConvert.SerializeObject(source);
            }
            return content;
        }

        public static ComplexRoutineHandler<StorageRoutineHandler<TUserContext>, TUserContext> GetContainerStorageHandler<TUserContext>(
                    ContainerFactory containerFactory,
                    MemberTag memberTag,
                    AspRoutineFeature aspRoutineFeature,
                    Func<object> getInput,
                    TUserContext userContext,
                    ApplicationSettings applicationSettings,
                    //Func<TUserContext, string> getConfigurationFor,
                    Func<TUserContext, string> getAuditStamp
            )
        {
            var adminkaRoutineHandlerFactory = new AdminkaRoutineHandlerFactory<TUserContext>(
                        correlationToken: Guid.NewGuid(),
                        InjectedManager.DefaultRoutineTagTransformException,
                        InjectedManager.ComposeNLogMemberLoggerFactory(null),
                        applicationSettings.PerformanceCounters);
  
            IHandler<RoutineClosure<TUserContext>> loggingHandler = adminkaRoutineHandlerFactory.CreateLoggingHandler(
                            memberTag,
                            containerFactory.CreateContainer(memberTag, getAuditStamp(userContext)),
                            userContext,
                            hasVerboseLoggingPrivilege: false,
                            getInput());

            return new ComplexRoutineHandler<StorageRoutineHandler<TUserContext>, TUserContext>/*AdminkaRoutineHandlerBase<TUserContext>*/(
                    closure => new AuthenticationDomStorageRoutineHandler<TUserContext>(
                        applicationSettings.AdminkaStorageConfiguration,
                        userContext,
                        null,
                        new Handler<RoutineClosure<TUserContext>, RoutineClosure<TUserContext>>(
                              () => closure,
                              closure
                        ),
                        getAudit: uc => getAuditStamp(uc)
                    ),
                    loggingHandler
                );
        }

        public static ComplexRoutineHandler<PerCallContainer<TUserContext>, TUserContext> GetContainerHandler<TUserContext>(
                    //ContainerFactory containerFactory,
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
                    documentBuilder: aspRoutineFeature.TraceDocument.Builder,
                    hasVerboseLoggingPrivilege: false,
                    configurationFor: getConfigurationFor(userContext),
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
                IMemoryCache memoryCache,
                PageRoutineFeature pageRoutineFeature
            )
        {
            return GetUserAndFailedActionResultInitialisedAsync(applicationSettings, memberTag,
                    getName: () => pageModel.User.Identity.Name,
                    systemIsInRole: g => pageModel.User.IsInRole(g),
                    aspRoutineFeature,
                    memoryCache,
                    getForbiddenActionResult: () => pageModel.RedirectToPage(
                        "AccessDenied",
                        new { pageRoutineFeature.Referrer }),
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

                        return await container.ResolveAuthenticationDomDbContextHandler().HandleDbContextAsync(
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

                        return await container.ResolveAuthenticationDomDbContextHandler().HandleDbContextAsync(
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

        public static UserContext SetAndGetUserContext(PageModel pageModel, User user)
        {
            var userContext = new UserContext(user);
            pageModel.ViewData["UserContext"] = userContext;
            return userContext;
        }

        public static PageRoutineFeature SetAndGetPageRoutineFeature(PageModel pageModel, Referrer referrer)
        {
            var pageRoutineFeature = new PageRoutineFeature() { Referrer = referrer.Href };
            pageModel.HttpContext.Features.Set(pageRoutineFeature);
            return pageRoutineFeature;
        }
    }
}