using System;
using System.Linq;
using System.Threading.Tasks;
using System.Runtime.CompilerServices;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

using DashboardCode.Routines;
using DashboardCode.Routines.Storage;
using DashboardCode.Routines.AspNetCore;
using DashboardCode.AdminkaV1.Injected.Logging;
using DashboardCode.AdminkaV1.Injected.NETStandard;
using DashboardCode.Routines.Configuration.NETStandard;
using DashboardCode.Routines.Configuration;
using DashboardCode.AdminkaV1.DataAccessEfCore;

namespace DashboardCode.AdminkaV1.Injected.AspCore.MvcApp
{
    public class MvcRoutine : AdminkaRoutineHandler
    {
        public readonly SessionState SessionState;
        public readonly ConfigurableController Controller;
        public MvcRoutine(ConfigurableController controller, [CallerMemberName] string action = "") :
            this(controller, WebManager.SetupCorrelationToken(controller.HttpContext), controller.HttpContext.Request.ToLog(), action)
        {
        }
        public MvcRoutine(ConfigurableController controller, object input, [CallerMemberName] string action = "") :
            this(controller, WebManager.SetupCorrelationToken(controller.HttpContext), input, action)
        {
        }
        private MvcRoutine(ConfigurableController controller, Guid correlationToken, object input, string action) :
            this(controller,
                new RoutineGuid(correlationToken, controller.GetType().Namespace, controller.GetType().Name, action),
                input)
        {
        }

        public MvcRoutine(ConfigurableController controller, RoutineGuid routineGuid, object input) :
            this(controller,
                 routineGuid,
                 new ConfigurationManagerLoader(controller.ConfigurationRoot),
                 input)
        {
        }

        public MvcRoutine(
                ConfigurableController controller, 
                RoutineGuid routineGuid,
                ConfigurationManagerLoader configurationManagerLoader,
                object input) :
            this(
                 new SqlServerAdmikaConfigurationFacade(configurationManagerLoader).ResolveAdminkaStorageConfiguration(),
                 new ConfigurationFactory(configurationManagerLoader),
                 controller,
                 routineGuid,
                 InjectedManager.ComposeNLogTransients(InjectedManager.DefaultRoutineTagTransformException),
                 input)
        {
            controller.HttpContext.Items["routineGuid"] = routineGuid;
            var headers = controller.HttpContext.Response.Headers;
            if (headers["X-RoutineGuid-CorrelationToken"].Count() == 0)
            {
                headers.Add("X-RoutineGuid-CorrelationToken",    routineGuid.CorrelationToken.ToString());
                headers.Add("X-RoutineGuid-MemberTag-Namespace", routineGuid.MemberTag.Namespace);
                headers.Add("X-RoutineGuid-MemberTag-Type",      routineGuid.MemberTag.Type);
                headers.Add("X-RoutineGuid-MemberTag-Member",    routineGuid.MemberTag.Member);
            }
        }
        private MvcRoutine(
            AdminkaStorageConfiguration admikaConfigurationFacade,
            IConfigurationFactory configurationFactory,
            ConfigurableController controller, RoutineGuid routineGuid,
            Func<RoutineGuid, IContainer, RoutineLoggingTransients> loggingTransientsFactory,
            object input) :
            base(
                admikaConfigurationFacade,
                configurationFactory,
                loggingTransientsFactory,
                routineGuid,
                controller.User.Identity,
                input)
        {
            this.Controller = controller;
            this.SessionState = new SessionState(controller.HttpContext.Session, userContext);
            controller.ViewBag.Session = this.SessionState;
            controller.ViewBag.UserContext = userContext;
        }

        #region MVC
        public async Task<IActionResult> HandleMvcRequestAsync<TKey, TEntity>(
                 string viewName,
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
                    await HandleStorageAsync<IActionResult, TEntity>( repository =>
                        MvcHandler.MakeActionResultOnRequest(
                                repository,
                                (n,v) => Controller.ViewData[n]=v,
                                Controller.HttpContext.Request,
                                o => Controller.View(viewName, o),
                                Controller.NotFound,
                                action
                             )
                    );

        public async Task<IActionResult> HandleMvcRequestAsync<TKey, TEntity>(
                 string viewName,
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
                    await HandleStorageAsync<IActionResult, TEntity>(repository =>
                       MvcHandler.MakeActionResultOnRequest(
                               repository,
                               Controller.HttpContext.Request,
                               o => Controller.View(viewName, o),
                               Controller.NotFound,
                               action
                            )
                    );

        public async Task<IActionResult> HandleMvcCreateAsync<TEntity>(
                 string viewName,
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
                        await HandleStorageAsync<IActionResult, TEntity>(repository =>
                           MvcHandler.MakeActionResultOnCreate(
                                   repository,
                                   (n, v) => Controller.ViewData[n] = v,
                                   Controller.HttpContext.Request,
                                   o => Controller.View(viewName, o),
                                   action
                                )
                        );

        public async Task<IActionResult> HandleMvcCreateAsync<TEntity>(
                 string viewName,
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
                        await HandleStorageAsync<IActionResult, TEntity>(repository =>
                           MvcHandler.MakeActionResultOnCreate(
                                   repository,
                                   Controller.HttpContext.Request,
                                   o => Controller.View(viewName, o),
                                   action
                                )
                        );

        public async Task<IActionResult> HandleMvcSaveAsync<TEntity>(
                 string viewName,
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
                    await HandleStorageAsync<IActionResult, TEntity>((repository, storage, state) =>
                       MvcHandler.MakeActionResultOnSave(
                           repository,
                           storage,
                           state,
                           () => Controller.Unauthorized(),
                           Controller.HttpContext.Request,
                           (n, v) => Controller.ViewData[n] = v,
                           (n, v) => Controller.ModelState.AddModelError(n, v),
                           () => Controller.RedirectToAction("Index"),
                           (e) => Controller.View(viewName, e),
                           action
                       )
                    );

        public async Task<IActionResult> HandleMvcSaveAsync<TEntity>(
                 string viewName,
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
                    await HandleStorageAsync<IActionResult, TEntity>((repository, storage, state) =>
                       MvcHandler.MakeActionResultOnSave(
                           repository,
                           storage,
                           state,
                           () => Controller.Unauthorized(),
                           Controller.HttpContext.Request,
                           (n, v) => Controller.ViewData[n] = v,
                           (n, v) => Controller.ModelState.AddModelError(n, v),
                           () => Controller.RedirectToAction("Index"),
                           (e) => Controller.View(viewName, e),
                           action
                       )
                    );
        #endregion
    }
}