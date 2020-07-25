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
using DashboardCode.AdminkaV1.AuthenticationDom.DataAccessEfCore.Services;
using Newtonsoft.Json;
using DashboardCode.AdminkaV1.Injected.Logging;
using System.Collections.Generic;
using Microsoft.AspNetCore.Http;

namespace DashboardCode.AdminkaV1.Injected.AspNetCore
{
    public static class AspNetCoreManager
    {
        public static PageRoutineFeature SetAndGetPageRoutineFeature(PageModel pageModel, Referrer referrer)
        {
            var pageRoutineFeature = new PageRoutineFeature() { Referrer = referrer.Href };
            pageModel.HttpContext.Features.Set(pageRoutineFeature);
            return pageRoutineFeature;
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

        public static Task<(IActionResult forbiddenActionResult, User user, ContainerFactory containerFactory)>
            GetUserAndFailedActionResultInitialisedAsync(
                ApplicationSettings applicationSettings,
                MemberTag memberTag,
                ControllerBase controllerBase,
                AspRoutineFeature aspRoutineFeature,
                IMemoryCache memoryCache,
                string aspRequestId
            )
        {
            return GetUserAndFailedActionResultInitialisedAsync(applicationSettings, memberTag,
                    getName: () => controllerBase.User.Identity.Name,
                    systemIsInRole: g => controllerBase.User.IsInRole(g),
                    aspRoutineFeature,
                    memoryCache,
                    getForbiddenActionResult: () => controllerBase.Forbid(),
                    exceptionToActionResult: (ex) => {
                        return AspNetCoreManager.GetErrorActionResult(ex, aspRequestId, applicationSettings.ForceDetailsOnCustomErrorPage, null);
                    }
                );
        }

        public static
            ComplexRoutineHandler<PerCallContainer<TUserContext>, TUserContext>
                GetContainerHandler<TUserContext, TUser>(
                    AspRoutineFeature aspRoutineFeature,
                    Func<object> getInput,
                    TUser user,
                    TUserContext userContext,
                    ContainerFactory containerFactory,
                    MemberTag memberTag,
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
                    hasVerboseLoggingPrivilege: false, // TODO: userContext ...
                    configurationFor: getAuditStamp(userContext),
                    memberTag: memberTag
                );
        }

        public static
            ComplexRoutineHandlerAsync<PerCallContainer<TUserContext>, TUserContext>
                GetContainerHandlerAsync<TUserContext, TUser>(
                    AspRoutineFeature aspRoutineFeature,
                    Func<object> getInput,
                    TUser user,
                    TUserContext userContext,
                    ContainerFactory containerFactory,
                    MemberTag memberTag,
                    ApplicationSettings applicationSettings,
                    Func<TUserContext, string> getConfigurationFor,
                    Func<TUserContext, string> getAuditStamp
            )
        {
            return new AdminkaRoutineHandlerBaseAsync<TUserContext>(
                    applicationSettings,
                    userContext,
                    getAuditStamp: getAuditStamp,
                    input: getInput(),
                    correlationToken: aspRoutineFeature.CorrelationToken,
                    documentBuilder: aspRoutineFeature.TraceDocument.Builder,
                    hasVerboseLoggingPrivilege: false, // TODO: userContext ...
                    configurationFor: getAuditStamp(userContext),
                    memberTag: memberTag
                );
        }

        /*
        public static
            ComplexRoutineHandlerAsync2<PerCallContainer<TUserContext>, TUserContext>
                GetContainerHandlerAsync2<TUserContext, TUser>(
                    AspRoutineFeature aspRoutineFeature,
                    Func<object> getInput,
                    TUser user,
                    TUserContext userContext,
                    ContainerFactory containerFactory,
                    MemberTag memberTag,
                    ApplicationSettings applicationSettings,
                    Func<TUserContext, string> getConfigurationFor,
                    Func<TUserContext, string> getAuditStamp
            )
        {
            return new AdminkaRoutineHandlerBaseAsync2<TUserContext>(
                    applicationSettings,
                    userContext,
                    getAuditStamp: getAuditStamp,
                    input: getInput(),
                    correlationToken: aspRoutineFeature.CorrelationToken,
                    documentBuilder: aspRoutineFeature.TraceDocument.Builder,
                    hasVerboseLoggingPrivilege: false, // TODO: userContext ...
                    configurationFor: getAuditStamp(userContext),
                    memberTag: memberTag
                );
        }
        */
        public static IRoutineHandler<TUser, TUserContext> GetUserHandler<TUser, TUserContext>(
            AspRoutineFeature aspRoutineFeature,
            Func<object> getInput,
            TUser user,
            TUserContext userContext,
            ContainerFactory containerFactory,
            MemberTag memberTag,
            ApplicationSettings applicationSettings,
            Func<TUserContext, string> configurationFor
            )
        {
            var composeLoggers = InjectedManager.ComposeNLogMemberLoggerFactory(aspRoutineFeature.TraceDocument.Builder);
            var logger = new AdminkaRoutineHandlerFactory<TUserContext>(aspRoutineFeature.CorrelationToken,
                InjectedManager.DefaultRoutineTagTransformException, composeLoggers,
                applicationSettings.PerformanceCounters);
            var @for = configurationFor(userContext);
            var container = containerFactory.CreateContainer(memberTag, @for);
            bool hasVerboseLoggingPrivilege = true;
            var input = getInput();
            var routineHandler = logger.CreateLoggingHandler(memberTag, container, userContext, hasVerboseLoggingPrivilege, input);

            return new ComplexRoutineHandler<TUser, TUserContext>(
                    closure => user,
                    routineHandler
                );
        }


        public static IRoutineHandlerAsync<TUser, TUserContext> GetUserHandlerAsync<TUser, TUserContext>(
            AspRoutineFeature aspRoutineFeature,
            Func<object> getInput,
            TUser user,
            TUserContext userContext,
            ContainerFactory containerFactory,
            MemberTag memberTag,
            ApplicationSettings applicationSettings,
            Func<TUserContext, string> configurationFor
            )
        {
            var composeLoggers = InjectedManager.ComposeNLogMemberLoggerFactory(aspRoutineFeature.TraceDocument.Builder);
            var logger = new AdminkaRoutineHandlerFactory<TUserContext>(aspRoutineFeature.CorrelationToken,
                InjectedManager.DefaultRoutineTagTransformException, composeLoggers,
                applicationSettings.PerformanceCounters);
            var @for = configurationFor(userContext);
            var container = containerFactory.CreateContainer(memberTag, @for);
            bool hasVerboseLoggingPrivilege = true;
            var input = getInput();
            var routineHandler = logger.CreateLoggingHandler(memberTag, container, userContext, hasVerboseLoggingPrivilege, input);

            return new ComplexRoutineHandlerAsync<TUser, TUserContext>(
                    closure => user,
                    routineHandler
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

            var routine = new AdminkaAnonymousRoutineHandlerAsync(
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

                        return await container.ResolveAuthenticationDomDbContextHandlerAsync().HandleDbContextAsync(
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

                        return await container.ResolveAuthenticationDomDbContextHandlerAsync().HandleDbContextAsync(
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

        public static ComplexRoutineHandler<PerCallContainer<TUserContext>, TUserContext> GetContainerHandler<TUserContext>(
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

        public static ComplexRoutineHandlerAsync<PerCallContainer<TUserContext>, TUserContext> GetContainerHandlerAsync<TUserContext>(
                    MemberTag memberTag,
                    AspRoutineFeature aspRoutineFeature,
                    Func<object> getInput,
                    TUserContext userContext,
                    ApplicationSettings applicationSettings,
                    Func<TUserContext, string> getConfigurationFor,
                    Func<TUserContext, string> getAuditStamp
            )
        {
            return new AdminkaRoutineHandlerBaseAsync<TUserContext>(
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
        /*
        public static ComplexRoutineHandlerAsync2<PerCallContainer<TUserContext>, TUserContext> GetContainerHandlerAsync2<TUserContext>(
                    MemberTag memberTag,
                    AspRoutineFeature aspRoutineFeature,
                    Func<object> getInput,
                    TUserContext userContext,
                    ApplicationSettings applicationSettings,
                    Func<TUserContext, string> getConfigurationFor,
                    Func<TUserContext, string> getAuditStamp
            )
        {
            return new AdminkaRoutineHandlerBaseAsync2<TUserContext>(
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
        */
        // ----------------

        public static IActionResult GetErrorActionResult(Exception ex, string aspRequestId, bool forceDetailsOnCustomErrorPage, User user)
        {
            var json = GetErrorActionJson(ex, aspRequestId, forceDetailsOnCustomErrorPage || (user != null
                && user.HasPrivilege(Privilege.ConfigureSystem))); // TODO: specifc privilege for "ADMIN's error dialog"
            return new ContentResult { Content = json, StatusCode = 500, ContentType = "application/json" };
        }

        public static string GetErrorActionJson(Exception ex, string aspRequestId, bool isAdminPrivilege)
        {
            var content = default(string);
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

        #region JQuery DataTable support
        public static (int startPageAtIndex, int pageLength, string searchValue, ICollection<(int, bool)> columnsOrders, ICollection<(int, string)> columnsSearches) GetJQueryDataTableRequest(ControllerBase baseController) =>
             GetJQueryDataTableRequest(baseController.HttpContext.Request.Form);

        public static (int startPageAtIndex, int pageLength, string searchValue, ICollection<(int, bool)> columnsOrders, ICollection<(int, string)> columnsSearches) GetJQueryDataTableRequest(IFormCollection formCollection)
        {
            var startPageAtIndex = int.Parse(formCollection["start"]);
            var pageLength = int.Parse(formCollection["length"]);
            var searchValue = formCollection["search[value]"].ToString();
            var columnsOrders = new List<(int, bool)>();
            var columnsSearches = new List<(int, string)>();
            
            var i = 0;
            while (
                formCollection.TryGetValue($"order[{i}][column]", out var columnNumber)
                && formCollection.TryGetValue($"order[{i}][dir]", out var direction)
                )
            {
                columnsOrders.Add((int.Parse(columnNumber), direction == "asc"));
                i++;
            }
            i = 0;
            while (formCollection.TryGetValue($"columns[{i}][search][value]", out var colSearchValue))
            {
                columnsSearches.Add((i, colSearchValue));
                i++;
            }
            return (startPageAtIndex, pageLength, searchValue, columnsOrders, columnsSearches);
        }
        #endregion
    }
}

