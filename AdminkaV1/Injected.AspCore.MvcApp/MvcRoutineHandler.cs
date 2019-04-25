﻿//using System;
//using System.Linq;
//using System.Threading.Tasks;
//using Microsoft.AspNetCore.Http;
//using Microsoft.AspNetCore.Mvc;

//using DashboardCode.Routines;
//using DashboardCode.Routines.Storage;
//using DashboardCode.Routines.AspNetCore;
//using DashboardCode.Routines.Logging;
//using DashboardCode.Routines.Configuration.Standard;
//using DashboardCode.Routines.Configuration;

//using DashboardCode.AdminkaV1.Injected.Logging;
//using DashboardCode.AdminkaV1.Injected.Telemetry;
//using Microsoft.AspNetCore.Mvc.RazorPages;
//using System.Collections.Generic;
//using System.Net;

//namespace DashboardCode.AdminkaV1.Injected.AspCore.MvcApp
//{
//    public class MetaRoutineHandler : PageRoutineHandler
//    {
//        public readonly SessionState SessionState;
//        public readonly ConfigurableController Controller;
//        public MetaRoutineHandler(ConfigurableController controller) :
//            this(controller, controller.HttpContext.Request.ToLog())
//        {
//        }
//        public MetaRoutineHandler(ConfigurableController controller, object input) :
//            this(controller, MvcAppManager.SetupCorrelationToken(controller.HttpContext), input)
//        {
//        }
//        private MetaRoutineHandler(ConfigurableController controller, Guid correlationToken, object input) :
//            this(controller,
//                correlationToken,
//                new MemberTag(
//                    controller.ControllerContext.ActionDescriptor.ControllerTypeInfo.Namespace,
//                    controller.ControllerContext.ActionDescriptor.ControllerTypeInfo.Name,
//                    controller.ControllerContext.ActionDescriptor.ActionName),
//                input)
//        {
//        }
//        public MetaRoutineHandler(
//            ConfigurableController controller,
//            Guid correlationToken,
//            MemberTag memberTag, 
//            object input) :
//            this(controller,
//                 correlationToken,
//                 memberTag,
//                 // MVC is configured to reload configuration data (and share one instance between all processes)
//                 // that is why I do not use InjectedManager.ConfigurationManagerLoader
//                 new ConfigurationManagerLoader(controller.RoutineResolvables),
//                 input)
//        {
//        }

//        private MetaRoutineHandler(
//                ConfigurableController controller,
//                Guid correlationToken,
//                MemberTag memberTag,
//                ConfigurationManagerLoader configurationManagerLoader,
//                object input) :
//            this(
//                 controller.ApplicationSettings.AdminkaStorageConfiguration,
//                 controller.ApplicationSettings.PerformanceCounters,
//                 InjectedManager.ResetConfigurationContainerFactoryStandard(configurationManagerLoader),
//                 controller,
//                 correlationToken,
//                 memberTag,
//                 InjectedManager.ComposeNLogMemberLoggerFactory(controller.ApplicationSettings.AuthenticationLogging),
//                 input)
//        {
//            controller.HttpContext.Items["CorrelationToken"] = correlationToken;
//            var headers = controller.HttpContext.Response.Headers;
//            if (headers["X-CorrelationToken"].Count() == 0)
//            {
//                headers.Add("X-CorrelationToken",    correlationToken.ToString());
//                headers.Add("X-MemberTag-Namespace", memberTag.Namespace);
//                headers.Add("X-MemberTag-Type",      memberTag.Type);
//                headers.Add("X-MemberTag-Member",    memberTag.Member);
//            }
//        }
//        private MetaRoutineHandler(
//            AdminkaStorageConfiguration adminkaStorageConfiguration,
//            IPerformanceCounters performanceCounters,
//            IConfigurationContainerFactory configurationFactory,
//            ConfigurableController controller, 
//            Guid correlationToken,
//            MemberTag memberTag,
//            Func<Guid, MemberTag, (IMemberLogger, IAuthenticationLogging)> loggingTransientsFactory,
//            object input) :
//            base(
//                adminkaStorageConfiguration,
//                performanceCounters,
//                configurationFactory,
//                loggingTransientsFactory,
//                correlationToken,
//                memberTag,
//                controller.User.Identity,
//                input)
//        {
//            this.Controller = controller;
//            this.SessionState = new SessionState(controller.HttpContext.Session, UserContext);
//            controller.ViewBag.Session = this.SessionState;
//            controller.ViewBag.UserContext = UserContext;
//        }

//        #region MVC
//        public Task<IActionResult> HandleMvcRequestAsync<TKey, TEntity>(
//                 string viewName,
//                 Func<
//                    IRepository<TEntity>,
//                    Func<
//                        Func<
//                            Func<string, ValuableResult<TKey>>,
//                            Func<TKey, TEntity>,
//                            Action<TEntity, Action<string, object>>,
//                            IActionResult>,
//                        IActionResult>
//                    > action
//                ) where TEntity : class =>
//                    StorageRoutineHandler.HandleStorageAsync<IActionResult, TEntity>( repository =>
//                        Task.Run(() =>
//                            MvcHandler.MakeActionResultOnRequest(
//                                    repository,
//                                    (n,v) => Controller.ViewData[n]=v,
//                                    Controller.HttpContext.Request,
//                                    (m) => new StatusCodeResult((int)HttpStatusCode.BadRequest),
//                                    o => Controller.View(viewName, o),
//                                    Controller.NotFound,
//                                    action
//                                 )
//                            )
//                    );

//        public Task<IActionResult> HandleMvcRequestAsync<TKey, TEntity>(
//                 string viewName,
//                 Func<
//                    IRepository<TEntity>,
//                    Func<
//                        Func<
//                            Func<string, ValuableResult<TKey>>,
//                            Func<TKey, TEntity>,
//                            IActionResult>,
//                        IActionResult>
//                    > action
//                ) where TEntity : class =>
//                    StorageRoutineHandler.HandleStorageAsync<IActionResult, TEntity>(repository =>
//                    Task.Run(() =>
//                      MvcHandler.MakeActionResultOnRequest(
//                               repository,
//                               Controller.HttpContext.Request,
//                               o => Controller.View(viewName, o),
//                               (m) => new StatusCodeResult((int)HttpStatusCode.BadRequest),
//                               Controller.NotFound,
//                               action
//                            )
//                    )
//                );

//        public Task<IActionResult> HandleMvcCreateAsync<TEntity>(
//                 string viewName,
//                 Func<
//                    IRepository<TEntity>,
//                    Func<
//                        Func<
//                            Func<TEntity>,
//                            Action<TEntity, Action<string, object>>,
//                            IActionResult>,
//                        IActionResult>
//                    > action
//                ) where TEntity : class =>
//                        StorageRoutineHandler.HandleStorageAsync<IActionResult, TEntity>(repository =>
//                           Task.Run( () =>
//                           MvcHandler.MakeActionResultOnCreate(
//                                   repository,
//                                   (n, v) => Controller.ViewData[n] = v,
//                                   Controller.HttpContext.Request,
//                                   o => Controller.View(viewName, o),
//                                   action
//                                )
//                           )
//                        );

//        public Task<IActionResult> HandleMvcCreateAsync<TEntity>(
//                 string viewName,
//                 Func<
//                    IRepository<TEntity>,
//                    Func<
//                        Func<
//                            Func<TEntity>,
//                            //Action<TEntity, Action<string, object>>,
//                            IActionResult>,
//                        IActionResult>
//                    > action
//                ) where TEntity : class =>
//                        StorageRoutineHandler.HandleStorageAsync<IActionResult, TEntity>(repository =>
//                        Task.Run(
//                           () => MvcHandler.MakeActionResultOnCreate(
//                                   repository,
//                                   Controller.HttpContext.Request,
//                                   o => Controller.View(viewName, o),
//                                   action
//                                )
//                            )
//                        );

//        public Task<IActionResult> HandleMvcSaveAsync<TEntity>(
//                 string viewName,
//                 Func<
//                    IRepository<TEntity>,
//                    RoutineClosure<UserContext>,
//                    Func<
//                       Func<
//                            Func<bool>,
//                            Func<HttpRequest, IComplexBinderResult<TEntity>>,
//                            Func<HttpRequest, TEntity, Action<string, object>, IComplexBinderResult<ValueTuple<Action<IBatch<TEntity>>, Action>>>,
//                            Action<TEntity, IBatch<TEntity>>,
//                            IActionResult>,
//                       IActionResult
//                    >
//                 > action
//                ) where TEntity : class =>
//                    StorageRoutineHandler.HandleStorageAsync<IActionResult, TEntity>((repository, storage, state) =>
//                       Task.Run(
//                            () => MvcHandler.MakeActionResultOnSave(
//                                repository,
//                                storage,
//                                state,
//                                () => Controller.Unauthorized(),
//                                Controller.HttpContext.Request,
//                                (n, v) => Controller.ViewData[n] = v,
//                                (n, v) => Controller.ModelState.AddModelError(n, v),
//                                () => Controller.RedirectToAction("Index"),
//                                (m) => new StatusCodeResult((int)HttpStatusCode.BadRequest),
//                                (e) => Controller.View(viewName, e),
//                                action
//                            )
//                         )
//                    );

//        public Task<IActionResult> HandleMvcSaveAsync<TEntity>(
//                 string viewName,
//                 Func<
//                    IRepository<TEntity>,
//                    RoutineClosure<UserContext>,
//                    Func<
//                       Func<
//                            Func<bool>,
//                            Func<HttpRequest, IComplexBinderResult<TEntity>>,
//                            //Func<HttpRequest, TEntity, Action<string, object>, IComplexBinderResult<ValueTuple<Action<IBatch<TEntity>>, Action>>>,
//                            Action<TEntity, IBatch<TEntity>>,
//                            IActionResult>,
//                       IActionResult
//                    >
//                 > action
//                ) where TEntity : class =>
//                    StorageRoutineHandler.HandleStorageAsync<IActionResult, TEntity>((repository, storage, state) =>
//                                Task.Run(
//                                    () => MvcHandler.MakeActionResultOnSave(
//                                    repository,
//                                    storage,
//                                    state,
//                                    () => Controller.Unauthorized(),
//                                    Controller.HttpContext.Request,
//                                    (n, v) => Controller.ViewData[n] = v,
//                                    (n, v) => Controller.ModelState.AddModelError(n, v),
//                                    () => Controller.RedirectToAction("Index"),
//                                    (m) => new StatusCodeResult((int)HttpStatusCode.BadRequest),
//                                    (e) => Controller.View(viewName, e),
//                                    action
//                                )
//                           )
//                    );
//        #endregion
//    }
//}