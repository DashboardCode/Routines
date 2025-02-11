using System;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Newtonsoft.Json;

using DashboardCode.Routines;
using DashboardCode.Routines.Storage;
using DashboardCode.Routines.AspNetCore;
using DashboardCode.Routines.Configuration;

using DashboardCode.AdminkaV1.AuthenticationDom;
using DashboardCode.AdminkaV1.Injected.Logging;
using DashboardCode.AdminkaV1.AuthenticationDom.DataAccessEfCore;
using System.Threading.Tasks;

namespace DashboardCode.AdminkaV1.Injected.AspCore.WebApp
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

        public static ComplexRoutineHandlerAsync<StorageRoutineHandlerAsync<TUserContext>, TUserContext> GetContainerStorageHandlerAsync<TUserContext>(
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
  
            IHandlerAsync<RoutineClosure<TUserContext>> loggingHandler = adminkaRoutineHandlerFactory.CreateLoggingHandler(
                            memberTag,
                            containerFactory.CreateContainer(memberTag, getAuditStamp(userContext)),
                            userContext,
                            hasVerboseLoggingPrivilege: false,
                            getInput());

            return new ComplexRoutineHandlerAsync<StorageRoutineHandlerAsync<TUserContext>, TUserContext>/*AdminkaRoutineHandlerBase<TUserContext>*/(
                    closure => new AuthenticationDomStorageRoutineHandlerAsync<TUserContext>(
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
        /*
        public static ComplexRoutineHandlerAsync2<StorageRoutineHandlerAsync<TUserContext>, TUserContext> GetContainerStorageHandlerAsync2<TUserContext>(
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

            IHandlerAsync<RoutineClosure<TUserContext>> loggingHandler = adminkaRoutineHandlerFactory.CreateLoggingHandler(
                            memberTag,
                            containerFactory.CreateContainer(memberTag, getAuditStamp(userContext)),
                            userContext,
                            hasVerboseLoggingPrivilege: false,
                            getInput());

            return new ComplexRoutineHandlerAsync2<StorageRoutineHandlerAsync<TUserContext>, TUserContext>(

                    closure => new Task<StorageRoutineHandlerAsync<TUserContext>>(()=>new AuthenticationDomStorageRoutineHandlerAsync<TUserContext>(
                        applicationSettings.AdminkaStorageConfiguration,
                        userContext,
                        null,
                        new Handler<RoutineClosure<TUserContext>, RoutineClosure<TUserContext>>(
                              () => closure,
                              closure
                        ),
                        getAudit: uc => getAuditStamp(uc)
                    )),
                    loggingHandler
                );
        }
    */
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