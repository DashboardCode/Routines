using System;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

using DashboardCode.Routines.Storage;

namespace DashboardCode.Routines.AspNetCore
{
    public class MetaPageRoutineHandler<TUserContext, TUser> 
    {
        // TODO: support SessionState
        // public readonly SessionState SessionState;
        public readonly PageModel PageModel;
        public readonly PageRoutineHandler<StorageRoutineHandler<TUserContext>, TUserContext, TUser> PageRoutineHandler;
        public MetaPageRoutineHandler(
            PageModel pageModel,
            PageRoutineHandler<StorageRoutineHandler<TUserContext>,TUserContext, TUser> pageRoutineHandler
            )
        {
            this.PageModel = pageModel;
            this.PageRoutineHandler = pageRoutineHandler;
        }

        #region MVC
        public IActionResult HandlePageRequest<TKey, TEntity>(
                 Action<TEntity> setPageEntity,
                 Func<
                    IRepository<TEntity>,
                    RoutineClosure<TUserContext>,
                    Func<
                        Func<
                            Func<bool>,
                            Func<string, ValuableResult<TKey>>,
                            Func<TKey, TEntity>,
                            Action<TEntity, Action<string, object>>,
                            IActionResult>,
                        IActionResult>
                    > action
                ) where TEntity : class =>
                    PageRoutineHandler.Handle((container, closure) =>
                       container.HandleRepository<IActionResult, TEntity>(repository =>
                               MvcHandler.MakeActionResultOnRequest(
                                  repository,

                                  (n, v) => PageModel.ViewData[n] = v,
                                  () => PageModel.Unauthorized(),
                                  PageModel.HttpContext.Request,
                                  o => {
                                      setPageEntity(o);
                                      return PageModel.Page();
                                  },
                                  (m) => {
                                      return PageModel.BadRequest();
                                  },
                                  PageModel.NotFound,
                                  (r) => action(r, closure)
                               )
                          
                   )
        );

        public IActionResult HandlePageRequest<TKey, TEntity>(
                 Action<TEntity> setPageEntity,
                 Func<
                    IRepository<TEntity>,
                    RoutineClosure<TUserContext>,
                    Func<
                        Func<
                            Func<bool>,
                            Func<string, ValuableResult<TKey>>,
                            Func<TKey, TEntity>,
                            IActionResult>,
                        IActionResult>
                    > action
                ) where TEntity : class =>
                   PageRoutineHandler.Handle((container, closure) =>
                        container.HandleRepository<IActionResult, TEntity>(repository =>
                           MvcHandler.MakeActionResultOnRequest(
                               repository,
                               () => PageModel.Unauthorized(),
                               PageModel.HttpContext.Request,
                               o => {
                                   setPageEntity(o);
                                   return PageModel.Page();
                               },
                               (m) => {
                                   return PageModel.BadRequest();
                               },
                               PageModel.NotFound,
                               (r) => action(r, closure)
                            )
                    )
                );

        public IActionResult HandlePageCreate<TEntity>(
                 Action<TEntity> setPageEntity,
                 Func<
                    IRepository<TEntity>,
                    RoutineClosure<TUserContext>,
                    Func<
                        Func<
                            Func<bool>,
                            Func<TEntity>,
                            Action<TEntity, Action<string, object>>,
                            IActionResult>,
                        IActionResult>
                    > action
                ) where TEntity : class =>
                   PageRoutineHandler.Handle( (container, closure) =>
                        container.HandleRepository<IActionResult, TEntity>(repository =>
                                MvcHandler.MakeActionResultOnCreate(
                                  repository,
                                  (n, v) => PageModel.ViewData[n] = v,
                                  () => PageModel.Unauthorized(),
                                  PageModel.HttpContext.Request,
                                  o => {
                                      setPageEntity(o);
                                      return PageModel.Page();
                                  },
                                  (r) => action(r, closure)
                               )
                           )
                        );

        public IActionResult HandlePageCreate<TEntity>(
                 Action<object> setPageEntity,
                 Func<
                    IRepository<TEntity>,
                    RoutineClosure<TUserContext>,
                    Func<
                        Func<
                            Func<bool>,
                            Func<TEntity>,
                            //Action<TEntity, Action<string, object>>,
                            IActionResult>,
                        IActionResult>
                    > action
                ) where TEntity : class =>
                   PageRoutineHandler.Handle((container, closure) =>
                        container.HandleRepository<IActionResult, TEntity>(repository =>
                        MvcHandler.MakeActionResultOnCreate(
                                   repository,
                                   () => PageModel.Unauthorized(),
                                   PageModel.HttpContext.Request,
                                   o => {
                                       setPageEntity(o);
                                       return PageModel.Page();
                                   },
                                   (r) => action(r, closure)
                                ))
                        );

        public IActionResult HandlePageSave<TEntity>(
                 Action<TEntity> setPageEntity,
                 PageRoutineFeature pageRoutineFeature,
                 Func<
                    IRepository<TEntity>,
                    RoutineClosure<TUserContext>,
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
                    PageRoutineHandler.Handle((container, closure) =>
                    container.HandleStorage<IActionResult, TEntity>((repository, storage, state) =>
                      MvcHandler.MakeActionResultOnSave(
                               repository,
                               storage,
                               state,
                               () => PageModel.Unauthorized(),
                               PageModel.HttpContext.Request,
                               (n, v) => PageModel.ViewData[n] = v,
                               (n, v) => PageModel.ModelState.AddModelError(n, v),
                                //Redirect(BackwardString);
                               () => new RedirectResult(pageRoutineFeature.Referrer), 
                               (m) => {
                                   return PageModel.BadRequest();
                               },
                               o => {
                                   setPageEntity(o);
                                   return PageModel.Page();
                               },
                               action
                           )
                        )
                    );

        public IActionResult HandlePageSave<TEntity>(
                 Action<TEntity> setPageEntity,
                 PageRoutineFeature pageRoutineFeature,
                 Func<
                    IRepository<TEntity>,
                    RoutineClosure<TUserContext>,
                    Func<
                       Func<
                            Func<bool>,
                            Func<HttpRequest, IComplexBinderResult<TEntity>>,
                            Action<TEntity, IBatch<TEntity>>,
                            IActionResult>,
                       IActionResult
                    >
                 > action
                ) where TEntity : class =>
                    PageRoutineHandler.Handle((container, closure) =>
                        container.HandleStorage<IActionResult, TEntity>((repository, storage, state) =>
                                 MvcHandler.MakeActionResultOnSave(
                                        repository,
                                        storage,
                                        state,
                                        () => PageModel.Unauthorized(),
                                        PageModel.HttpContext.Request,
                                        (n, v) => PageModel.ViewData[n] = v,
                                        (n, v) => PageModel.ModelState.AddModelError(n, v),

                                        () => new RedirectResult(pageRoutineFeature.Referrer),
                                        (m) => {
                                            return PageModel.BadRequest();
                                        },
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
    /*
    public class MetaPageRoutineHandlerAsync2<TUserContext, TUser>
    {
        // TODO: support SessionState
        // public readonly SessionState SessionState;
        public readonly PageModel PageModel;
        public readonly PageRoutineHandlerAsync2<StorageRoutineHandlerAsync<TUserContext>, TUserContext, TUser> PageRoutineHandler;
        public MetaPageRoutineHandlerAsync2(
            PageModel pageModel,
            PageRoutineHandlerAsync2<StorageRoutineHandlerAsync<TUserContext>, TUserContext, TUser> pageRoutineHandler
            )
        {
            this.PageModel = pageModel;
            this.PageRoutineHandler = pageRoutineHandler;
        }

        #region MVC
        public Task<IActionResult> HandlePageRequestAsync<TKey, TEntity>(
                 Action<TEntity> setPageEntity,
                 Func<
                    IRepository<TEntity>,
                    RoutineClosure<TUserContext>,
                    Func<
                        Func<
                            Func<bool>,
                            Func<string, ValuableResult<TKey>>,
                            Func<TKey, TEntity>,
                            Action<TEntity, Action<string, object>>,
                            IActionResult>,
                        IActionResult>
                    > action
                ) where TEntity : class =>

                    PageRoutineHandler.HandleAsync(async (container, closure) =>
                       await container.HandleRepositoryAsync<IActionResult, TEntity>(repository =>
                           Task.Run(() =>
                               MvcHandler.MakeActionResultOnRequest(
                                  repository,

                                  (n, v) => PageModel.ViewData[n] = v,
                                  () => PageModel.Unauthorized(),
                                  PageModel.HttpContext.Request,
                                  o => {
                                      setPageEntity(o);
                                      return PageModel.Page();
                                  },
                                  (m) => {
                                      return PageModel.BadRequest();
                                  },
                                  PageModel.NotFound,
                                  (r) => action(r, closure)
                               )
                          )
                   )
        );

        public Task<IActionResult> HandlePageRequestAsync<TKey, TEntity>(
                 Action<TEntity> setPageEntity,
                 Func<
                    IRepository<TEntity>,
                    RoutineClosure<TUserContext>,
                    Func<
                        Func<
                            Func<bool>,
                            Func<string, ValuableResult<TKey>>,
                            Func<TKey, TEntity>,
                            IActionResult>,
                        IActionResult>
                    > action
                ) where TEntity : class =>
                   PageRoutineHandler.HandleAsync(async (container, closure) =>
                        await container.HandleRepositoryAsync<IActionResult, TEntity>(repository =>
                         Task.Run(() =>
                           MvcHandler.MakeActionResultOnRequest(
                               repository,
                               () => PageModel.Unauthorized(),
                               PageModel.HttpContext.Request,
                               o => {
                                   setPageEntity(o);
                                   return PageModel.Page();
                               },
                               (m) => {
                                   return PageModel.BadRequest();
                               },
                               PageModel.NotFound,
                               (r) => action(r, closure)
                            )
                    ))
                );

        public Task<IActionResult> HandlePageCreateAsync<TEntity>(
                 Action<TEntity> setPageEntity,
                 Func<
                    IRepository<TEntity>,
                    RoutineClosure<TUserContext>,
                    Func<
                        Func<
                            Func<bool>,
                            Func<TEntity>,
                            Action<TEntity, Action<string, object>>,
                            IActionResult>,
                        IActionResult>
                    > action
                ) where TEntity : class =>
                   PageRoutineHandler.HandleAsync(async (container, closure) =>
                        await container.HandleRepositoryAsync<IActionResult, TEntity>(repository =>
                           Task.Run(() =>
                                MvcHandler.MakeActionResultOnCreate(
                                  repository,
                                  (n, v) => PageModel.ViewData[n] = v,
                                  () => PageModel.Unauthorized(),
                                  PageModel.HttpContext.Request,
                                  o => {
                                      setPageEntity(o);
                                      return PageModel.Page();
                                  },
                                  (r) => action(r, closure)
                               )
                           )
                        )
                        );

        public Task<IActionResult> HandlePageCreateAsync<TEntity>(
                 Action<object> setPageEntity,
                 Func<
                    IRepository<TEntity>,
                    RoutineClosure<TUserContext>,
                    Func<
                        Func<
                            Func<bool>,
                            Func<TEntity>,
                            //Action<TEntity, Action<string, object>>,
                            IActionResult>,
                        IActionResult>
                    > action
                ) where TEntity : class =>
                   PageRoutineHandler.HandleAsync(async (container, closure) =>
                   await container.HandleRepositoryAsync<IActionResult, TEntity>(repository =>
                        Task.Run(
                           () => MvcHandler.MakeActionResultOnCreate(
                                   repository,
                                   () => PageModel.Unauthorized(),
                                   PageModel.HttpContext.Request,
                                   o => {
                                       setPageEntity(o);
                                       return PageModel.Page();
                                   },
                                   (r) => action(r, closure)
                                ))
                            )
                        );

        public Task<IActionResult> HandlePageSaveAsync<TEntity>(
                 Action<TEntity> setPageEntity,
                 PageRoutineFeature pageRoutineFeature,
                 Func<
                    IRepository<TEntity>,
                    RoutineClosure<TUserContext>,
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
                    PageRoutineHandler.HandleAsync(async (container, closure) =>
                    await container.HandleStorageAsync<IActionResult, TEntity>((repository, storage, state) =>
                      Task.Run(
                           () => MvcHandler.MakeActionResultOnSave(
                               repository,
                               storage,
                               state,
                               () => PageModel.Unauthorized(),
                               PageModel.HttpContext.Request,
                               (n, v) => PageModel.ViewData[n] = v,
                               (n, v) => PageModel.ModelState.AddModelError(n, v),
                               //Redirect(BackwardString);
                               () => new RedirectResult(pageRoutineFeature.Referrer),
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
                 PageRoutineFeature pageRoutineFeature,
                 Func<
                    IRepository<TEntity>,
                    RoutineClosure<TUserContext>,
                    Func<
                       Func<
                            Func<bool>,
                            Func<HttpRequest, IComplexBinderResult<TEntity>>,
                            Action<TEntity, IBatch<TEntity>>,
                            IActionResult>,
                       IActionResult
                    >
                 > action
                ) where TEntity : class =>
                    PageRoutineHandler.HandleAsync(async (container, closure) =>
                     await container.HandleStorageAsync<IActionResult, TEntity>((repository, storage, state) =>
                                 Task.Run(
                                    () => MvcHandler.MakeActionResultOnSave(
                                        repository,
                                        storage,
                                        state,
                                        () => PageModel.Unauthorized(),
                                        PageModel.HttpContext.Request,
                                        (n, v) => PageModel.ViewData[n] = v,
                                        (n, v) => PageModel.ModelState.AddModelError(n, v),

                                        () => new RedirectResult(pageRoutineFeature.Referrer),
                                        (m) => {
                                            return PageModel.BadRequest();
                                        },
                                        o => {
                                            setPageEntity(o);
                                            return PageModel.Page();
                                        },
                                        action
                                    )
                           )
                        )
                    );
        #endregion
    }
    */
    public class MetaPageRoutineHandlerAsync<TUserContext, TUser>
    {
        // TODO: support SessionState
        // public readonly SessionState SessionState;
        public readonly PageModel PageModel;
        public readonly PageRoutineHandlerAsync<StorageRoutineHandlerAsync<TUserContext>, TUserContext, TUser> PageRoutineHandler;
        public MetaPageRoutineHandlerAsync(
            PageModel pageModel,
            PageRoutineHandlerAsync<StorageRoutineHandlerAsync<TUserContext>, TUserContext, TUser> pageRoutineHandler
            )
        {
            this.PageModel = pageModel;
            this.PageRoutineHandler = pageRoutineHandler;
        }

        #region MVC
        public Task<IActionResult> HandlePageRequestAsync<TKey, TEntity>(
                 Action<TEntity> setPageEntity,
                 Func<
                    IRepository<TEntity>,
                    RoutineClosure<TUserContext>,
                    Func<
                        Func<
                            Func<bool>,
                            Func<string, ValuableResult<TKey>>,
                            Func<TKey, TEntity>,
                            Action<TEntity, Action<string, object>>,
                            IActionResult>,
                        IActionResult>
                    > action
                ) where TEntity : class =>

                    PageRoutineHandler.HandleAsync(async (container, closure) =>
                       await container.HandleRepositoryAsync<IActionResult, TEntity>(repository =>
                           Task.Run(() =>
                               MvcHandler.MakeActionResultOnRequest(
                                  repository,

                                  (n, v) => PageModel.ViewData[n] = v,
                                  () => PageModel.Unauthorized(),
                                  PageModel.HttpContext.Request,
                                  o => {
                                      setPageEntity(o);
                                      return PageModel.Page();
                                  },
                                  (m) => {
                                      return PageModel.BadRequest();
                                  },
                                  PageModel.NotFound,
                                  (r) => action(r, closure)
                               )
                          )
                   )
        );

        public Task<IActionResult> HandlePageRequestAsync<TKey, TEntity>(
                 Action<TEntity> setPageEntity,
                 Func<
                    IRepository<TEntity>,
                    RoutineClosure<TUserContext>,
                    Func<
                        Func<
                            Func<bool>,
                            Func<string, ValuableResult<TKey>>,
                            Func<TKey, TEntity>,
                            IActionResult>,
                        IActionResult>
                    > action
                ) where TEntity : class =>
                   PageRoutineHandler.HandleAsync(async (container, closure) =>
                        await container.HandleRepositoryAsync<IActionResult, TEntity>(repository =>
                         Task.Run(() =>
                           MvcHandler.MakeActionResultOnRequest(
                               repository,
                               () => PageModel.Unauthorized(),
                               PageModel.HttpContext.Request,
                               o => {
                                   setPageEntity(o);
                                   return PageModel.Page();
                               },
                               (m) => {
                                   return PageModel.BadRequest();
                               },
                               PageModel.NotFound,
                               (r) => action(r, closure)
                            )
                    ))
                );

        public Task<IActionResult> HandlePageCreateAsync<TEntity>(
                 Action<TEntity> setPageEntity,
                 Func<
                    IRepository<TEntity>,
                    RoutineClosure<TUserContext>,
                    Func<
                        Func<
                            Func<bool>,
                            Func<TEntity>,
                            Action<TEntity, Action<string, object>>,
                            IActionResult>,
                        IActionResult>
                    > action
                ) where TEntity : class =>
                   PageRoutineHandler.HandleAsync(async (container, closure) =>
                        await container.HandleRepositoryAsync<IActionResult, TEntity>(repository =>
                           Task.Run(() =>
                                MvcHandler.MakeActionResultOnCreate(
                                  repository,
                                  (n, v) => PageModel.ViewData[n] = v,
                                  () => PageModel.Unauthorized(),
                                  PageModel.HttpContext.Request,
                                  o => {
                                      setPageEntity(o);
                                      return PageModel.Page();
                                  },
                                  (r) => action(r, closure)
                               )
                           )
                        )
                        );

        public Task<IActionResult> HandlePageCreateAsync<TEntity>(
                 Action<object> setPageEntity,
                 Func<
                    IRepository<TEntity>,
                    RoutineClosure<TUserContext>,
                    Func<
                        Func<
                            Func<bool>,
                            Func<TEntity>,
                            //Action<TEntity, Action<string, object>>,
                            IActionResult>,
                        IActionResult>
                    > action
                ) where TEntity : class =>
                   PageRoutineHandler.HandleAsync(async (container, closure) =>
                   await container.HandleRepositoryAsync<IActionResult, TEntity>(repository =>
                        Task.Run(
                           () => MvcHandler.MakeActionResultOnCreate(
                                   repository,
                                   () => PageModel.Unauthorized(),
                                   PageModel.HttpContext.Request,
                                   o => {
                                       setPageEntity(o);
                                       return PageModel.Page();
                                   },
                                   (r) => action(r, closure)
                                ))
                            )
                        );

        public Task<IActionResult> HandlePageSaveAsync<TEntity>(
                 Action<TEntity> setPageEntity,
                 PageRoutineFeature pageRoutineFeature,
                 Func<
                    IRepository<TEntity>,
                    RoutineClosure<TUserContext>,
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
                    PageRoutineHandler.HandleAsync(async (container, closure) =>
                    await container.HandleStorageAsync<IActionResult, TEntity>((repository, storage, state) =>
                      Task.Run(
                           () => MvcHandler.MakeActionResultOnSave(
                               repository,
                               storage,
                               state,
                               () => PageModel.Unauthorized(),
                               PageModel.HttpContext.Request,
                               (n, v) => PageModel.ViewData[n] = v,
                               (n, v) => PageModel.ModelState.AddModelError(n, v),
                               //Redirect(BackwardString);
                               () => new RedirectResult(pageRoutineFeature.Referrer),
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
                 PageRoutineFeature pageRoutineFeature,
                 Func<
                    IRepository<TEntity>,
                    RoutineClosure<TUserContext>,
                    Func<
                       Func<
                            Func<bool>,
                            Func<HttpRequest, IComplexBinderResult<TEntity>>,
                            Action<TEntity, IBatch<TEntity>>,
                            IActionResult>,
                       IActionResult
                    >
                 > action
                ) where TEntity : class =>
                    PageRoutineHandler.HandleAsync(async (container, closure) =>
                     await container.HandleStorageAsync<IActionResult, TEntity>((repository, storage, state) =>
                                 Task.Run(
                                    () => MvcHandler.MakeActionResultOnSave(
                                        repository,
                                        storage,
                                        state,
                                        () => PageModel.Unauthorized(),
                                        PageModel.HttpContext.Request,
                                        (n, v) => PageModel.ViewData[n] = v,
                                        (n, v) => PageModel.ModelState.AddModelError(n, v),

                                        () => new RedirectResult(pageRoutineFeature.Referrer),
                                        (m) => {
                                            return PageModel.BadRequest();
                                        },
                                        o => {
                                            setPageEntity(o);
                                            return PageModel.Page();
                                        },
                                        action
                                    )
                           )
                        )
                    );
        #endregion
    }
}