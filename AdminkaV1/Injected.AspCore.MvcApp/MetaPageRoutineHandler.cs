using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

using Microsoft.Extensions.Options;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

using DashboardCode.Routines;
using DashboardCode.Routines.Storage;
using DashboardCode.Routines.AspNetCore;
using DashboardCode.Routines.Configuration.Standard;

namespace DashboardCode.AdminkaV1.Injected.AspCore.MvcApp
{
    public class MetaPageRoutineHandler : PageRoutineHandler
    {
        // TODO: support SessionState
        // public readonly SessionState SessionState;

        public readonly PageModel PageModel;
        public MetaPageRoutineHandler(
            PageModel pageModel,
            Action<PageRoutineFeature> onPageRoutineFeature,
            string defaultBackwardUrl,
            [CallerMemberName] string member = null
            ) :
            base(
                pageModel,
                onPageRoutineFeature,
                defaultBackwardUrl,
                (ApplicationSettings)pageModel.HttpContext.RequestServices.GetService(typeof(ApplicationSettings)),
                (IMemoryCache)pageModel.HttpContext.RequestServices.GetService(typeof(IMemoryCache)),
                member
                )
        {
            // TODO reload configuration on changed
            var trackedConfigurationSnapshot = (IOptionsSnapshot<List<RoutineResolvable>>)pageModel.HttpContext.RequestServices.GetService(typeof(IOptionsSnapshot<List<RoutineResolvable>>));
            var trackedConfiguration = trackedConfigurationSnapshot.Value;
            this.PageModel = pageModel;
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
                    HandleAsync( async (container, closure) =>
                    await container.ResolveAdminkaDbContextHandler().HandleStorageAsync<IActionResult, TEntity>(repository =>
                       Task.Run(() =>
                           MvcHandler.MakeActionResultOnRequest(
                                   repository,
                                   (n, v) => PageModel.ViewData[n] = v,
                                   PageModel.HttpContext.Request,
                                   o => {
                                            setPageEntity(o);
                                            return PageModel.Page();
                                   },
                                   (m) => {
                                       return PageModel.BadRequest();
                                   },
                                   PageModel.NotFound,
                                   action
                                )
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
                    HandleAsync(async (container, closure) =>
                   await container.ResolveAdminkaDbContextHandler().HandleStorageAsync<IActionResult, TEntity>(repository =>
                   Task.Run(() =>
                      MvcHandler.MakeActionResultOnRequest(
                               repository,
                               PageModel.HttpContext.Request,
                               o => {
                                   setPageEntity(o);
                                   return PageModel.Page();
                               },
                               (m) => {
                                   return PageModel.BadRequest();
                               },
                               PageModel.NotFound,
                               action
                            )
                    ))
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
                    HandleAsync(async (container, closure) =>
                   await container.ResolveAdminkaDbContextHandler().HandleStorageAsync<IActionResult, TEntity>(repository =>
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
                           ))
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
                    HandleAsync(async (container, closure) =>
                   await container.ResolveAdminkaDbContextHandler().HandleStorageAsync<IActionResult, TEntity>(repository =>
                        Task.Run(
                           () => MvcHandler.MakeActionResultOnCreate(
                                   repository,
                                   PageModel.HttpContext.Request,
                                   o => {
                                       setPageEntity(o);
                                       return PageModel.Page();
                                   },
                                   action
                                ))
                            )
                        );

        public Task<IActionResult> HandlePageSaveAsync<TEntity>(
                 Action<TEntity> setPageEntity,
                 //string indexPage,
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
                    HandleAsync(async (container, closure) =>
                    await container.ResolveAdminkaDbContextHandler().HandleStorageAsync<IActionResult, TEntity>( (repository,storage,state) =>
                       Task.Run(
                            () => MvcHandler.MakeActionResultOnSave(
                                repository,
                                storage,
                                state,
                                () => PageModel.Unauthorized(),
                                PageModel.HttpContext.Request,
                                (n, v) => PageModel.ViewData[n] = v,
                                (n, v) => PageModel.ModelState.AddModelError(n, v),
                                () => PageModel.RedirectToPage(this.PageRoutineFeature.BackwardUrl),
                                (m) => {
                                    return PageModel.BadRequest();
                                },
                                o => {
                                    setPageEntity(o);
                                    return PageModel.Page();
                                },
                                action
                            )
                         ))
                    );

        public Task<IActionResult> HandlePageSaveAsync<TEntity>(
                 Action<TEntity> setPageEntity,
                 //string indexPage,
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
                    HandleAsync(async (container, closure) =>
                    await container.ResolveAdminkaDbContextHandler().HandleStorageAsync<IActionResult, TEntity>((repository, storage, state) =>
                               Task.Run(
                                    () => MvcHandler.MakeActionResultOnSave(
                                    repository,
                                    storage,
                                    state,
                                    () => PageModel.Unauthorized(),
                                    PageModel.HttpContext.Request,
                                    (n, v) => PageModel.ViewData[n] = v,
                                    (n, v) => PageModel.ModelState.AddModelError(n, v),
                                    () => PageModel.RedirectToPage(this.PageRoutineFeature.BackwardUrl),
                                    (m) => {
                                        return PageModel.BadRequest();
                                    },
                                    o => {
                                        setPageEntity(o);
                                        return PageModel.Page();
                                    },
                                    action
                                )
                           ))
                    );
        #endregion
    }
}