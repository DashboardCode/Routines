using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

using DashboardCode.Routines;
using DashboardCode.Routines.Storage;
using DashboardCode.Routines.AspNetCore;
using DashboardCode.Routines.Logging;
using DashboardCode.Routines.Configuration.Standard;
using DashboardCode.Routines.Configuration;

using DashboardCode.AdminkaV1.Injected.Logging;
using DashboardCode.AdminkaV1.Injected.Telemetry;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Collections.Generic;

namespace DashboardCode.AdminkaV1.Injected.AspCore.MvcApp
{
    public class PageRoutineHandler : AdminkaRoutineHandler
    {
        public readonly SessionState SessionState;
        public readonly PageModel PageModel;
        public PageRoutineHandler(PageModel pageModel, ApplicationSettings applicationSettings, List<RoutineResolvable> routineResolvables) :
            this(pageModel, applicationSettings, routineResolvables, pageModel.HttpContext.Request.ToLog())
        {
        }
        public PageRoutineHandler(PageModel pageModel, ApplicationSettings applicationSettings, List<RoutineResolvable> routineResolvables, object input) :
            this(pageModel, applicationSettings, routineResolvables, WebManager.SetupCorrelationToken(pageModel.HttpContext), input)
        {
        }
        private PageRoutineHandler(PageModel pageModel, ApplicationSettings applicationSettings, List<RoutineResolvable> routineResolvables, Guid correlationToken, object input) :
            this(pageModel,
                applicationSettings, routineResolvables,
                correlationToken,
                new MemberTag(
                    pageModel.PageContext.ActionDescriptor.PageTypeInfo.Namespace,
                    pageModel.PageContext.ActionDescriptor.PageTypeInfo.Name,
                    pageModel.PageContext.ActionDescriptor.DisplayName /*ActionName*/), // TODO TEST - should be verb
                input)
        {
        }
        public PageRoutineHandler(
            PageModel pageModel, ApplicationSettings applicationSettings, List<RoutineResolvable> routineResolvables,
            Guid correlationToken,
            MemberTag memberTag,
            object input) :
            this(pageModel,
                applicationSettings,
                 correlationToken,
                 memberTag,
                 // MVC is configured to reload configuration data (and share one instance between all processes)
                 // that is why I do not use InjectedManager.ConfigurationManagerLoader
                 new ConfigurationManagerLoader(routineResolvables),
                 input)
        {
        }

        private PageRoutineHandler(
                PageModel pageModel, ApplicationSettings applicationSettings,
                Guid correlationToken,
                MemberTag memberTag,
                ConfigurationManagerLoader configurationManagerLoader,
                object input) :
            this(
                 applicationSettings.AdminkaStorageConfiguration,
                 applicationSettings.PerformanceCounters,
                 InjectedManager.ResetConfigurationContainerFactoryStandard(configurationManagerLoader),
                 pageModel,
                 correlationToken,
                 memberTag,
                 InjectedManager.ComposeNLogMemberLoggerFactory(applicationSettings.AuthenticationLogging),
                 input)
        {
            pageModel.HttpContext.Items["CorrelationToken"] = correlationToken;
            var headers = pageModel.HttpContext.Response.Headers;
            if (headers["X-CorrelationToken"].Count() == 0)
            {
                headers.Add("X-CorrelationToken", correlationToken.ToString());
                headers.Add("X-MemberTag-Namespace", memberTag.Namespace);
                headers.Add("X-MemberTag-Type", memberTag.Type);
                headers.Add("X-MemberTag-Member", memberTag.Member);
            }
        }
        private PageRoutineHandler(
            AdminkaStorageConfiguration adminkaStorageConfiguration,
            IPerformanceCounters performanceCounters,
            IConfigurationContainerFactory configurationFactory,
            PageModel pageModel,
            Guid correlationToken,
            MemberTag memberTag,
            Func<Guid, MemberTag, (IMemberLogger, IAuthenticationLogging)> loggingTransientsFactory,
            object input) :
            base(
                adminkaStorageConfiguration,
                performanceCounters,
                configurationFactory,
                loggingTransientsFactory,
                correlationToken,
                memberTag,
                pageModel.User.Identity,
                input)
        {
            this.PageModel = pageModel;
            this.SessionState = new SessionState(pageModel.HttpContext.Session, UserContext);
            pageModel.ViewData["Session"] = this.SessionState;
            pageModel.ViewData["UserContext"] = UserContext;
        }

        #region MVC
        public Task<IActionResult> HandlePageRequestAsync<TKey, TEntity>(
                 Action<TEntity> setPageEntity,
                 Func<
                    IRepository<TEntity>,
                    Func<
                        Func<
                            Func<string, ValuableResult<TKey>>,
                            Func<TKey, TEntity>,
                            Action<TEntity, Action<string, object>>,
                            IActionResult>,
                        IActionResult>
                    > action
                ) where TEntity : class =>
                    StorageRoutineHandler.HandleStorageAsync<IActionResult, TEntity>(repository =>
                       Task.Run(() =>
                           MvcHandler.MakeActionResultOnRequest(
                                   repository,
                                   (n, v) => PageModel.ViewData[n] = v,
                                   PageModel.HttpContext.Request,
                                   o => {
                                            setPageEntity(o);
                                            return PageModel.Page();
                                       },
                                   PageModel.NotFound,
                                   action
                                )
                           )
                    );

        public Task<IActionResult> HandlePageRequestAsync<TKey, TEntity>(
                 Action<TEntity> setPageEntity,
                 Func<
                    IRepository<TEntity>,
                    Func<
                        Func<
                            Func<string, ValuableResult<TKey>>,
                            Func<TKey, TEntity>,
                            IActionResult>,
                        IActionResult>
                    > action
                ) where TEntity : class =>
                    StorageRoutineHandler.HandleStorageAsync<IActionResult, TEntity>(repository =>
                    Task.Run(() =>
                      MvcHandler.MakeActionResultOnRequest(
                               repository,
                               PageModel.HttpContext.Request,
                               o => {
                                   setPageEntity(o);
                                   return PageModel.Page();
                               },
                               PageModel.NotFound,
                               action
                            )
                    )
                );

        public Task<IActionResult> HandlePageCreateAsync<TEntity>(
                 Action<TEntity> setPageEntity,
                 Func<
                    IRepository<TEntity>,
                    Func<
                        Func<
                            Func<TEntity>,
                            Action<TEntity, Action<string, object>>,
                            IActionResult>,
                        IActionResult>
                    > action
                ) where TEntity : class =>
                        StorageRoutineHandler.HandleStorageAsync<IActionResult, TEntity>(repository =>
                           Task.Run(() =>
                          MvcHandler.MakeActionResultOnCreate(
                                  repository,
                                  (n, v) => PageModel.ViewData[n] = v,
                                  PageModel.HttpContext.Request,
                                  o => {
                                      setPageEntity(o);
                                      return PageModel.Page();
                                  },
                                  action
                               )
                           )
                        );

        public Task<IActionResult> HandlePageCreateAsync<TEntity>(
                 Action<object> setPageEntity,
                 Func<
                    IRepository<TEntity>,
                    Func<
                        Func<
                            Func<TEntity>,
                            //Action<TEntity, Action<string, object>>,
                            IActionResult>,
                        IActionResult>
                    > action
                ) where TEntity : class =>
                        StorageRoutineHandler.HandleStorageAsync<IActionResult, TEntity>(repository =>
                        Task.Run(
                           () => MvcHandler.MakeActionResultOnCreate(
                                   repository,
                                   PageModel.HttpContext.Request,
                                   o => {
                                       setPageEntity(o);
                                       return PageModel.Page();
                                   },
                                   action
                                )
                            )
                        );

        public Task<IActionResult> HandlePageSaveAsync<TEntity>(
                 Action<TEntity> setPageEntity,
                 string indexPage,
                 Func<
                    IRepository<TEntity>,
                    RoutineClosure<UserContext>,
                    Func<
                       Func<
                            Func<bool>,
                            Func<HttpRequest, IComplexBinderResult<TEntity>>,
                            Func<HttpRequest, TEntity, Action<string, object>, IComplexBinderResult<ValueTuple<Action<IBatch<TEntity>>, Action>>>,
                            Action<TEntity, IBatch<TEntity>>,
                            IActionResult>,
                       IActionResult
                    >
                 > action
                ) where TEntity : class =>
                    StorageRoutineHandler.HandleStorageAsync<IActionResult, TEntity>((repository, storage, state) =>
                       Task.Run(
                            () => MvcHandler.MakeActionResultOnSave(
                                repository,
                                storage,
                                state,
                                () => PageModel.Unauthorized(),
                                PageModel.HttpContext.Request,
                                (n, v) => PageModel.ViewData[n] = v,
                                (n, v) => PageModel.ModelState.AddModelError(n, v),
                                () => PageModel.RedirectToPage(indexPage),
                                o => {
                                    setPageEntity(o);
                                    return PageModel.Page();
                                },
                                action
                            )
                         )
                    );

        public Task<IActionResult> HandlePageSaveAsync<TEntity>(
                 Action<TEntity> setPageEntity,
                 string indexPage,
                 Func<
                    IRepository<TEntity>,
                    RoutineClosure<UserContext>,
                    Func<
                       Func<
                            Func<bool>,
                            Func<HttpRequest, IComplexBinderResult<TEntity>>,
                            //Func<HttpRequest, TEntity, Action<string, object>, IComplexBinderResult<ValueTuple<Action<IBatch<TEntity>>, Action>>>,
                            Action<TEntity, IBatch<TEntity>>,
                            IActionResult>,
                       IActionResult
                    >
                 > action
                ) where TEntity : class =>
                    StorageRoutineHandler.HandleStorageAsync<IActionResult, TEntity>((repository, storage, state) =>
                                Task.Run(
                                    () => MvcHandler.MakeActionResultOnSave(
                                    repository,
                                    storage,
                                    state,
                                    () => PageModel.Unauthorized(),
                                    PageModel.HttpContext.Request,
                                    (n, v) => PageModel.ViewData[n] = v,
                                    (n, v) => PageModel.ModelState.AddModelError(n, v),
                                    () => PageModel.RedirectToPage(indexPage),
                                    o => {
                                        setPageEntity(o);
                                        return PageModel.Page();
                                    },
                                    action
                                )
                           )
                    );
        #endregion
    }
}