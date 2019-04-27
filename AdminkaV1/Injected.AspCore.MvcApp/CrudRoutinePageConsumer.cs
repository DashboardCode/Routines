using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq.Expressions;

using Microsoft.Extensions.Primitives;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

using DashboardCode.Routines;
using DashboardCode.Routines.Storage;
using DashboardCode.Routines.AspNetCore;

namespace DashboardCode.AdminkaV1.Injected.AspCore.MvcApp
{
    class CrudRoutinePageConsumer<TEntity, TKey> where TEntity : class, new()
    {
        #region Compose
        public static Func<PageModel, Action<PageRoutineFeature>, string, string,  Task<IActionResult>> Compose(Func<MetaPageRoutineHandler, Task<IActionResult>> func)
            => (p, onPageRoutineFeature, defaultBackwardUrl, member) => func(
                new MetaPageRoutineHandler(p, onPageRoutineFeature, defaultBackwardUrl, member));

        public static Func<PageModel, Action<PageRoutineFeature>, string, string, Task<IActionResult>> ComposeAsync(Func<IRepository<TEntity>, RoutineClosure<UserContext>, Task<IActionResult>> func)
            => Compose(
                 routineHandler =>
                     routineHandler.HandleAsync( async (container, closure) => await container.ResolveAdminkaDbContextHandler().HandleStorageAsync<IActionResult, TEntity>(
                        repository => func(repository, closure)
                    ))
            );
        public static Func<Func<IEnumerable<TEntity>, IActionResult>, Func<PageModel, Action<PageRoutineFeature>, string, string, Task<IActionResult>>>
            ComposeAsync(
            Func<Func<IEnumerable<TEntity>, IActionResult>, Func<IRepository<TEntity>, RoutineClosure<UserContext>, Task<IActionResult>>> func)
            => view => ComposeAsync(func(view));

        

        public static Func<PageModel, Action<PageRoutineFeature>, string, string, Task<IActionResult>> ComposePageSaveAsync(
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
                > action)
            => Compose(
                async routine =>
                    await routine.HandlePageSaveAsync(
                        setPageEntity, /*indexPage,*/
                        action
                    )
            );

        public static Func<PageModel, Action<PageRoutineFeature>, string, string, Task<IActionResult>> ComposePageSaveAsync(
            Action<TEntity> setPageEntity,
            //string indexPage,
            Func<
                IRepository<TEntity>,
                RoutineClosure<UserContext>,
                Func<
                    Func<
                         Func<bool>,
                         Func<HttpRequest, IComplexBinderResult<TEntity>>,
                         Action<TEntity, IBatch<TEntity>>,
                         IActionResult>,
                    IActionResult
                    >
                > action)
            => Compose(
                async routine =>
                    await routine.HandlePageSaveAsync(
                        setPageEntity, /*indexPage,*/
                        action
                    )
            );

        public static Func<PageModel, Action<PageRoutineFeature>, string, string, Task<IActionResult>> ComposePageRequestAsync(
            Action<TEntity> setPageEntity,
            Func<
                IRepository<TEntity>,
                RoutineClosure<UserContext>,
                Func<
                Func<
                    Func<bool>,
                    Func<string, ValuableResult<TKey>>,
                    Func<TKey, TEntity>,
                    Action<TEntity, Action<string, object>>,
                    IActionResult>,
                IActionResult
                >
            > action)
            => Compose(
                async routine =>
                    await routine.HandlePageRequestAsync(
                        setPageEntity,
                        action
                    )
            );

        public static Func<PageModel, Action<PageRoutineFeature>, string, string, Task<IActionResult>> ComposePageRequestAsync(
            Action<TEntity> setPageEntity,
            Func<
                IRepository<TEntity>,
                RoutineClosure<UserContext>,
                Func<
                Func<
                    Func<bool>,
                    Func<string, ValuableResult<TKey>>,
                    Func<TKey, TEntity>,
                    IActionResult>,
                IActionResult
                >
            > action)
            => Compose(
                async routine =>
                    await routine.HandlePageRequestAsync(
                        setPageEntity,
                        action
                    )
            );

        public static Func<PageModel, Action<PageRoutineFeature>, string, string, Task<IActionResult>> ComposePageCreateAsync(
                 Action<TEntity> setPageEntity,
                 Func<
                    IRepository<TEntity>,
                    RoutineClosure<UserContext>,
                    Func<
                        Func<
                            Func<bool>,
                            Func<TEntity>,
                            Action<TEntity, Action<string, object>>,
                            IActionResult>,
                        IActionResult>
                    > action)
            => Compose(
                async routine =>
                    await routine.HandlePageCreateAsync(
                        setPageEntity,
                        action
                    )
            );

        public static Func<PageModel, Action<PageRoutineFeature>, string, string, Task<IActionResult>> ComposePageCreateAsync(
                 Action<object> setPageEntity,
                 Func<
                    IRepository<TEntity>,
                    RoutineClosure<UserContext>,
                    Func<
                        Func<
                            Func<bool>,
                            Func<TEntity>,
                            IActionResult>,
                        IActionResult>
                    > action)
            => Compose(
                async routine =>
                    await routine.HandlePageCreateAsync(
                        setPageEntity,
                        action
                    )
            );



        #endregion

        #region Compose Commented
        //public static Func<ConfigurableController, Task<IActionResult>> ComposeAsync(string actionName, Func<IRepository<TEntity>, IOrmStorage<TEntity>, Routine<UserContext>, IActionResult> func)
        //    => Compose(actionName,
        //        async routine =>
        //            await routine.HandleStorageAsync<IActionResult, TEntity>(
        //                (repository, storage, state) => func(repository, storage, state)
        //            )
        //    );

        //public static Func<Func<object, IActionResult>, Action<string,object>, Func<ConfigurableController, Task<IActionResult>>> ComposeAsync(
        //    string action, Func<Func<object, IActionResult>, Action<string, object>, Func<IRepository<TEntity>, IActionResult>> func)
        //    => (view, addViewData) => ComposeAsync(action, func(view, addViewData));

        //public static Func<Func<object, IActionResult>, Func<IActionResult>, Func<ConfigurableController, Task<IActionResult>>> ComposeAsync(
        //    string action, Func<Func<object, IActionResult>, Func<IActionResult>, Func<IRepository<TEntity>, IActionResult>> func)
        //    => (view, notFound) => ComposeAsync(action, func(view, notFound));

        //public static Func<Func<object, IActionResult>, Func<IActionResult>, Action<string, object>,  Func<ConfigurableController, Task<IActionResult>>> ComposeAsync(
        //    string action, Func<Func<object, IActionResult>, Func<IActionResult>, Action<string, object>, Func<IRepository<TEntity>, IActionResult>> func)
        //    => (view, notFound, addViewData) => ComposeAsync(action, func(view, notFound, addViewData));

        //public static Func<Func<object, IActionResult>, Func<IActionResult>, Action<string, string>, Action<string, object>, Func<IActionResult>, Func<ConfigurableController, Task<IActionResult>>> ComposeAsync(
        //    string action, Func<Func<object, IActionResult>, Func<IActionResult>, Action<string, string>, Action<string, object>, Func<IActionResult>, Func<IRepository<TEntity>, IOrmStorage<TEntity>, Routine<UserContext>, IActionResult>> func)
        //    => (view, unauthorized, addModelError, addViewData, redirect) => ComposeAsync(action, func(view, unauthorized, addModelError, addViewData, redirect));

        //public static Func<Func<object, IActionResult>, Func<IActionResult>, Action<string, string>, Func<bool>, Action<string, object>, Func<IActionResult>, Func<ConfigurableController, Task<IActionResult>>> ComposeAsync(
        //    string action, Func<Func<object, IActionResult>, Func<IActionResult>, Action<string, string>, Func<bool>, Action<string, object>, Func<IActionResult>, Func<IRepository<TEntity>, IOrmStorage<TEntity>, Routine<UserContext>, IActionResult>> func)
        //    => (view, unauthorized, addModelError, isValid, addViewData, redirect) => ComposeAsync(action, func(view, unauthorized, addModelError, isValid, addViewData, redirect));

        //public static Func<HttpRequest, Func<Func<object, IActionResult>, Func<IActionResult>, Func<ConfigurableController, Task<IActionResult>>>> ComposeAsync(
        //    string action, Func<HttpRequest, Func<Func<object, IActionResult>, Func<IActionResult>, Func<IRepository<TEntity>, IActionResult>>> func)
        //    => httpRequest => ComposeAsync(action, func(httpRequest));

        //public static Func<HttpRequest, Func<Func<object, IActionResult>, Func<IActionResult>,  Action<string, object>, Func<ConfigurableController, Task<IActionResult>>>> ComposeAsync(
        //    string action, Func<HttpRequest, Func<Func<object, IActionResult>, Func<IActionResult>, Action<string, object>, Func<IRepository<TEntity>, IActionResult>>> func)
        //    => httpRequest => ComposeAsync(action, func(httpRequest));

        //public static Func<HttpRequest, Func<Func<object, IActionResult>, Func<IActionResult>, Action<string, string>, Func<bool>, Action<string, object>, Func<IActionResult>, Func<ConfigurableController, Task<IActionResult>>>> ComposeAsync(
        //    string action, Func<HttpRequest, Func<Func<object, IActionResult>, Func<IActionResult>, Action<string, string>, Func<bool>, Action<string, object>, Func<IActionResult>, Func<IRepository<TEntity>, IOrmStorage<TEntity>, Routine<UserContext>, IActionResult>>> func)
        //    => httpRequest => ComposeAsync(action, func(httpRequest));

        //public static Func<HttpRequest, Func<Func<object, IActionResult>, Func<IActionResult>, Action<string, string>, Action<string, object>, Func<IActionResult>, Func<ConfigurableController, Task<IActionResult>>>> ComposeAsync(
        //    string action, Func<HttpRequest, Func<Func<object, IActionResult>, Func<IActionResult>, Action<string, string>, Action<string, object>, Func<IActionResult>, Func<IRepository<TEntity>, IOrmStorage<TEntity>, Routine<UserContext>, IActionResult>>> func)
        //    => httpRequest => ComposeAsync(action, func(httpRequest));
        #endregion

        #region Compose MVC Controlelr methods

        public static Func<Task<IActionResult>> ComposeIndex(
            PageModel pageModel,
            Action<IEnumerable<TEntity>> setPageEntity,
            Action<PageRoutineFeature> onPageRoutineFeature,
            string defaultBackwardUrl,
            Func<string, UserContext, bool> authorize,
            Include<TEntity> indexIncludes) =>
             () => ComposeAsync(view => async (repository, state) => {
                 if (! (authorize?.Invoke("Index", state.UserContext)??true))
                     return pageModel.Unauthorized();
                 var entities = await repository.ListAsync(indexIncludes);
                 return view(entities);
             })(o => { setPageEntity(o); return pageModel.Page(); })(pageModel, onPageRoutineFeature, defaultBackwardUrl, "Index");

        public static Func<Task<IActionResult>> ComposeDetails(
            PageModel pageModel,
            Action<TEntity> setPageEntity,
            Action<PageRoutineFeature> onPageRoutineFeature,
            string defaultBackwardUrl,
            Func<string, UserContext, bool> authorize,
            Include<TEntity> detailsIncludes,
            Func<string, ValuableResult<TKey>> keyConverter,
            Func<TKey, Expression<Func<TEntity, bool>>> findPredicate
            ) =>
               () => ComposePageRequestAsync(
                    setPageEntity,
                    (repository, state) => steps =>
                        steps(
                            () => authorize?.Invoke("Details", state.UserContext) ?? true,
                            keyConverter,
                            key => repository.Find(findPredicate(key), detailsIncludes)
                            )
                    )(pageModel, onPageRoutineFeature, defaultBackwardUrl, "Details");

        public static Func<Task<IActionResult>> ComposeCreate(
            PageModel pageModel,
            Action<TEntity> setPageEntity,
            Action<PageRoutineFeature> onPageRoutineFeature,
            string defaultBackwardUrl,
            Func<string, UserContext, bool> authorize,
            Action<Action<string, object>, IRepository<TEntity>> prepareEmptyOptions
            ) =>
                () => ComposePageCreateAsync(
                    setPageEntity,
                    (repository, state) => steps =>
                        steps(
                            () => authorize?.Invoke("Create", state.UserContext) ?? true,
                            () => default,
                            (entity, addViewData) => prepareEmptyOptions(addViewData, repository)
                            )
                    )(pageModel, onPageRoutineFeature, defaultBackwardUrl, "Create");

        public static Func<Task<IActionResult>> ComposeCreateConfirmed(
            PageModel pageModel,
            Action<TEntity> setPageEntity,
            Action<PageRoutineFeature> onPageRoutineFeature,
            string defaultBackwardUrl,
            Func<string, UserContext, bool> authorize,
            Func<TEntity> constructor,
            Dictionary<string, Func<TEntity, Func<StringValues, IVerboseResult<List<string>>>>> formFields,
            Dictionary<string, Func<TEntity, Action<StringValues>>> hiddenFormFields,
            Func<Action<string, object>, HttpRequest, IRepository<TEntity>, TEntity, IComplexBinderResult<ValueTuple<Action<IBatch<TEntity>>, Action>>> parseRelated
            ) =>
                () => ComposePageSaveAsync(setPageEntity, /* defaultBackwardUrl,*/
                (repository, state) => steps =>
                    steps(
                         () => authorize?.Invoke("Create", state.UserContext) ?? true,
                         request => MvcHandler.Bind(request, constructor, formFields, hiddenFormFields),
                         (request, entity, addViewData) => parseRelated(addViewData, request, repository, entity),
                         (entity, batch) => batch.Add(entity)
                    )
                )(pageModel, onPageRoutineFeature, defaultBackwardUrl, "CreateConfirmed");

        public static Func<Task<IActionResult>> ComposeEdit(
            PageModel pageModel,
            Action<TEntity> setPageEntity,
            Action<PageRoutineFeature> onPageRoutineFeature,
            string defaultBackwardUrl,
            Func<string, UserContext, bool> authorize,
            Include<TEntity> editIncludes,
            Func<string, ValuableResult<TKey>> keyConverter,
            Func<TKey, Expression<Func<TEntity, bool>>> findPredicate,
            Func<Action<string, object>, IRepository<TEntity>, Action<TEntity>> parseRelated
            ) =>
                () => ComposePageRequestAsync(setPageEntity,
                    (repository, state) => steps =>
                        steps(
                            () => authorize?.Invoke("Edit", state.UserContext) ?? true,
                            keyConverter,
                            key => repository.Find(findPredicate(key), editIncludes),
                            (entity, addViewData) => parseRelated(addViewData, repository)(entity)
                        )
                )(pageModel, onPageRoutineFeature, defaultBackwardUrl, "Edit");

        public static Func<Task<IActionResult>> ComposeEditConfirmed(
            PageModel pageModel,
            Action<TEntity> setPageEntity,
            Action<PageRoutineFeature> onPageRoutineFeature,
            string defaultBackwardUrl,
            Func<string, UserContext, bool> authorize,
            Func<TEntity> constructor,
            Dictionary<string, Func<TEntity, Func<StringValues, IVerboseResult<List<string>>>>> formFields,
            Dictionary<string, Func<TEntity, Action<StringValues>>> hiddenFormFields,
            Include<TEntity> disabledFormFields,
            Func<Action<string, object>, HttpRequest, IRepository<TEntity>, TEntity, IComplexBinderResult<ValueTuple<Action<IBatch<TEntity>>, Action>>> parseRelated
            ) =>
                () => ComposePageSaveAsync(setPageEntity, /* defaultBackwardUrl,*/
                    (repository, state) => steps =>
                        steps(
                            () => authorize?.Invoke("Edit", state.UserContext)??true,
                            request => MvcHandler.Bind(request, constructor, formFields, hiddenFormFields),
                            (request, entity, addViewData) => parseRelated(addViewData, request, repository, entity),
                            (entity, batch) => batch.Modify(entity, disabledFormFields)
                         )
                )(pageModel, onPageRoutineFeature, defaultBackwardUrl, "EditConfirmed");

        public static Func<Task<IActionResult>> ComposeDelete(
            PageModel pageModel,
            Action<TEntity> setPageEntity,
            Action<PageRoutineFeature> onPageRoutineFeature,
            string defaultBackwardUrl,
            Func<string, UserContext, bool> authorize,
            Include<TEntity> deleteIncludes,
            Func<string, ValuableResult<TKey>> keyConverter,
            Func<TKey, Expression<Func<TEntity, bool>>> findPredicate
            ) =>
                () => ComposePageRequestAsync(setPageEntity,
                    (repository, state) => steps =>
                        steps(
                            () => authorize?.Invoke("Delete", state.UserContext) ?? true,
                            keyConverter,
                            key => repository.Find(findPredicate(key), deleteIncludes)
                        )
                )(pageModel, onPageRoutineFeature, defaultBackwardUrl, "Delete");

        public static Func<Task<IActionResult>> ComposeDeleteConfirmed(
            PageModel pageModel,
            Action<TEntity> setPageEntity,
            Action<PageRoutineFeature> onPageRoutineFeature,
            string defaultBackwardUrl,
            Func<string, UserContext, bool> authorize,
            Func<TEntity> constructor,
            Dictionary<string, Func<TEntity, Action<StringValues>>> hiddenFormFields
            ) =>
                () => ComposePageSaveAsync(setPageEntity,/* defaultBackwardUrl,*/
                    (repository, state) => steps =>
                        steps(
                            () => authorize?.Invoke("Delete", state.UserContext)??true,
                            request => MvcHandler.Bind(request, constructor, null, hiddenFormFields),
                            (entity, batch) => batch.Remove(entity)
                    )
                )(pageModel, onPageRoutineFeature, defaultBackwardUrl, "DeleteConfirmed");
        #endregion

    }
}