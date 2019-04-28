using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq.Expressions;

using Microsoft.Extensions.Primitives;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

using DashboardCode.Routines.Storage;

namespace DashboardCode.Routines.AspNetCore
{
    public class CrudRoutinePageConsumer<TUserContext, TUser, TEntity, TKey> where TEntity : class, new()
    {
        #region Compose
        public static Func<PageRoutineHandler<ComplexRoutineHandler<StorageRoutineHandler<TUserContext>, TUserContext>, TUserContext, TUser>, Task<IActionResult>> Compose(
            Func<MetaPageRoutineHandler<TUserContext, TUser>, Task<IActionResult>> func
        )
        {
            Func<PageRoutineHandler<ComplexRoutineHandler<StorageRoutineHandler<TUserContext>, TUserContext>, TUserContext, TUser>, Task<IActionResult>> x = (pageRoutineHandler) =>
            {
                return func(
                  new MetaPageRoutineHandler<TUserContext, TUser>(pageRoutineHandler));
            };
            return x;
        }

        public static Func<PageRoutineHandler<ComplexRoutineHandler<StorageRoutineHandler<TUserContext>, TUserContext>, TUserContext, TUser>, Task<IActionResult>> ComposeAsync(
            Func<IRepository<TEntity>, RoutineClosure<TUserContext>, Task<IActionResult>> func
        )
        {
            return Compose(
                 metaRoutineHandler =>
                     metaRoutineHandler.PageRoutineHandler.HandleAsync(async (container, closure) => await container.Handle(s=>s.HandleStorageAsync<IActionResult, TEntity>(
                       repository => func(repository, closure)
                   )))
            );
        }

        public static Func<Func<IEnumerable<TEntity>, IActionResult>, Func<PageRoutineHandler<ComplexRoutineHandler<StorageRoutineHandler<TUserContext>, TUserContext>, TUserContext, TUser>, Task<IActionResult>>>
            ComposeAsync(
            Func<Func<IEnumerable<TEntity>, IActionResult>, Func<IRepository<TEntity>, RoutineClosure<TUserContext>, Task<IActionResult>>> func)
            => view => ComposeAsync(func(view));

        public static Func<PageRoutineHandler<ComplexRoutineHandler<StorageRoutineHandler<TUserContext>, TUserContext>, TUserContext, TUser>, Task<IActionResult>> ComposePageSaveAsync(
            Action<TEntity> setPageEntity,
            //string indexPage,
            Func<
                IRepository<TEntity>,
                RoutineClosure<TUserContext>,
                Func<
                    Func<
                         PageRoutineFeature,
                         Func<bool>,
                         Func<HttpRequest, IComplexBinderResult<TEntity>>,
                         Func<HttpRequest, TEntity, Action<string, object>, IComplexBinderResult<ValueTuple<Action<IBatch<TEntity>>, Action>>>,
                         Action<TEntity, IBatch<TEntity>>,
                         IActionResult>,
                    IActionResult
                    >
                > action)
            => Compose(
                async handler =>
                    await handler.HandlePageSaveAsync(
                        setPageEntity, /*indexPage,*/
                        action
                    )
            );

        public static Func<PageRoutineHandler<ComplexRoutineHandler<StorageRoutineHandler<TUserContext>, TUserContext>, TUserContext, TUser>, Task<IActionResult>> ComposePageSaveAsync(
            Action<TEntity> setPageEntity,
            //string indexPage,
            Func<
                IRepository<TEntity>,
                RoutineClosure<TUserContext>,
                Func<
                    Func<
                         PageRoutineFeature,
                         Func<bool>,
                         Func<HttpRequest, IComplexBinderResult<TEntity>>,
                         Action<TEntity, IBatch<TEntity>>,
                         IActionResult>,
                    IActionResult
                    >
                > action)
            => Compose(
                async handler =>
                    await handler.HandlePageSaveAsync(
                        setPageEntity, /*indexPage,*/
                        action
                    )
            );

        public static Func<PageRoutineHandler<ComplexRoutineHandler<StorageRoutineHandler<TUserContext>, TUserContext>, TUserContext, TUser>, Task<IActionResult>> ComposePageRequestAsync(
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
                IActionResult
                >
            > action)
            => Compose(
                async handler =>
                    await handler.HandlePageRequestAsync(
                        setPageEntity,
                        action
                    )
            );

        public static Func<PageRoutineHandler<ComplexRoutineHandler<StorageRoutineHandler<TUserContext>, TUserContext>, TUserContext, TUser>, Task<IActionResult>> ComposePageRequestAsync(
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

        public static Func<PageRoutineHandler<ComplexRoutineHandler<StorageRoutineHandler<TUserContext>, TUserContext>, TUserContext, TUser>, Task<IActionResult>> ComposePageCreateAsync(
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
                    > action)
            => Compose(
                async routine =>
                    await routine.HandlePageCreateAsync(
                        setPageEntity,
                        action
                    )
            );

        public static Func<PageRoutineHandler<ComplexRoutineHandler<StorageRoutineHandler<TUserContext>, TUserContext>, TUserContext, TUser>, Task<IActionResult>> ComposePageCreateAsync(
                 Action<object> setPageEntity,
                 Func<
                    IRepository<TEntity>,
                    RoutineClosure<TUserContext>,
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

        #region Compose MVC Controlelr methods

        public static Func<Task<IActionResult>> ComposeIndex(
            PageModel pageModel,
            Action<IEnumerable<TEntity>> setPageEntity,
            Action<PageRoutineFeature> onPageRoutineFeature,
            string defaultBackwardUrl,
            Func<string, TUserContext, bool> authorize,
            Include<TEntity> indexIncludes,
            Func<PageModel, Action<PageRoutineFeature>, string, string, PageRoutineHandler<ComplexRoutineHandler<StorageRoutineHandler<TUserContext>, TUserContext>, TUserContext, TUser>> createPageRoutineHandler
            ) =>
             () => ComposeAsync(view => async (repository, state) => {
                 if (!(authorize?.Invoke("Index", state.UserContext) ?? true))
                     return pageModel.Unauthorized();
                 var entities = await repository.ListAsync(indexIncludes);
                 return view(entities);
             })(o => { setPageEntity(o); return pageModel.Page(); })(createPageRoutineHandler(pageModel, onPageRoutineFeature, defaultBackwardUrl, "Index"));

        public static Func<Task<IActionResult>> ComposeDetails(
            PageModel pageModel,
            Action<TEntity> setPageEntity,
            Action<PageRoutineFeature> onPageRoutineFeature,
            string defaultBackwardUrl,
            Func<string, TUserContext, bool> authorize,
            Include<TEntity> detailsIncludes,
            Func<string, ValuableResult<TKey>> keyConverter,
            Func<TKey, Expression<Func<TEntity, bool>>> findPredicate,
            Func<PageModel, Action<PageRoutineFeature>, string, string, PageRoutineHandler<ComplexRoutineHandler<StorageRoutineHandler<TUserContext>, TUserContext>, TUserContext, TUser>> createPageRoutineHandler
            ) =>
               () => ComposePageRequestAsync(
                    setPageEntity,
                    (repository, state) => steps =>
                        steps(
                            () => authorize?.Invoke("Details", state.UserContext) ?? true,
                            keyConverter,
                            key => repository.Find(findPredicate(key), detailsIncludes)
                            )
                    )(createPageRoutineHandler(pageModel, onPageRoutineFeature, defaultBackwardUrl, "Details"));

        public static Func<Task<IActionResult>> ComposeCreate(
            PageModel pageModel,
            Action<TEntity> setPageEntity,
            Action<PageRoutineFeature> onPageRoutineFeature,
            string defaultBackwardUrl,
            Func<string, TUserContext, bool> authorize,
            Action<Action<string, object>, IRepository<TEntity>> prepareEmptyOptions,
            Func<PageModel, Action<PageRoutineFeature>, string, string, PageRoutineHandler<ComplexRoutineHandler<StorageRoutineHandler<TUserContext>, TUserContext>, TUserContext, TUser>> createPageRoutineHandler
            ) =>
                () => ComposePageCreateAsync(
                    setPageEntity,
                    (repository, state) => steps =>
                        steps(
                            () => authorize?.Invoke("Create", state.UserContext) ?? true,
                            () => default,
                            (entity, addViewData) => prepareEmptyOptions(addViewData, repository)
                            )
                    )(createPageRoutineHandler(pageModel, onPageRoutineFeature, defaultBackwardUrl, "Create"));

        public static Func<Task<IActionResult>> ComposeCreateConfirmed(
            PageModel pageModel,
            Action<TEntity> setPageEntity,
            Action<PageRoutineFeature> onPageRoutineFeature,
            string defaultBackwardUrl,
            Func<string, TUserContext, bool> authorize,
            Func<TEntity> constructor,
            Dictionary<string, Func<TEntity, Func<StringValues, IVerboseResult<List<string>>>>> formFields,
            Dictionary<string, Func<TEntity, Action<StringValues>>> hiddenFormFields,
            Func<Action<string, object>, HttpRequest, IRepository<TEntity>, TEntity, IComplexBinderResult<ValueTuple<Action<IBatch<TEntity>>, Action>>> parseRelated,
            Func<PageModel, Action<PageRoutineFeature>, string, string, PageRoutineHandler<ComplexRoutineHandler<StorageRoutineHandler<TUserContext>, TUserContext>, TUserContext, TUser>> createPageRoutineHandler
            ) =>
                () => ComposePageSaveAsync(setPageEntity, /* defaultBackwardUrl,*/
                (repository, state) => steps =>
                    steps(
                         null, // !! TODO
                         () => authorize?.Invoke("Create", state.UserContext) ?? true,
                         request => MvcHandler.Bind(request, constructor, formFields, hiddenFormFields),
                         (request, entity, addViewData) => parseRelated(addViewData, request, repository, entity),
                         (entity, batch) => batch.Add(entity)
                    )
                )(createPageRoutineHandler(pageModel, onPageRoutineFeature, defaultBackwardUrl, "CreateConfirmed"));

        public static Func<Task<IActionResult>> ComposeEdit(
            PageModel pageModel,
            Action<TEntity> setPageEntity,
            Action<PageRoutineFeature> onPageRoutineFeature,
            string defaultBackwardUrl,
            Func<string, TUserContext, bool> authorize,
            Include<TEntity> editIncludes,
            Func<string, ValuableResult<TKey>> keyConverter,
            Func<TKey, Expression<Func<TEntity, bool>>> findPredicate,
            Func<Action<string, object>, IRepository<TEntity>, Action<TEntity>> parseRelated,
            Func<PageModel, Action<PageRoutineFeature>, string, string, PageRoutineHandler<ComplexRoutineHandler<StorageRoutineHandler<TUserContext>, TUserContext>, TUserContext, TUser>> createPageRoutineHandler
            ) =>
                () => ComposePageRequestAsync(setPageEntity,
                    (repository, state) => steps =>
                        steps(
                            () => authorize?.Invoke("Edit", state.UserContext) ?? true,
                            keyConverter,
                            key => repository.Find(findPredicate(key), editIncludes),
                            (entity, addViewData) => parseRelated(addViewData, repository)(entity)
                        )
                )(createPageRoutineHandler(pageModel, onPageRoutineFeature, defaultBackwardUrl, "Edit"));

        public static Func<Task<IActionResult>> ComposeEditConfirmed(
            PageModel pageModel,
            Action<TEntity> setPageEntity,
            Action<PageRoutineFeature> onPageRoutineFeature,
            string defaultBackwardUrl,
            Func<string, TUserContext, bool> authorize,
            Func<TEntity> constructor,
            Dictionary<string, Func<TEntity, Func<StringValues, IVerboseResult<List<string>>>>> formFields,
            Dictionary<string, Func<TEntity, Action<StringValues>>> hiddenFormFields,
            Include<TEntity> disabledFormFields,
            Func<Action<string, object>, HttpRequest, IRepository<TEntity>, TEntity, IComplexBinderResult<ValueTuple<Action<IBatch<TEntity>>, Action>>> parseRelated,
            Func<PageModel, Action<PageRoutineFeature>, string, string, PageRoutineHandler<ComplexRoutineHandler<StorageRoutineHandler<TUserContext>, TUserContext>, TUserContext, TUser>> createPageRoutineHandler
            ) =>
                () => ComposePageSaveAsync(setPageEntity, /* defaultBackwardUrl,*/
                    (repository, state) => steps =>
                        steps(
                            null, // !! TODO
                            () => authorize?.Invoke("Edit", state.UserContext) ?? true,
                            request => MvcHandler.Bind(request, constructor, formFields, hiddenFormFields),
                            (request, entity, addViewData) => parseRelated(addViewData, request, repository, entity),
                            (entity, batch) => batch.Modify(entity, disabledFormFields)
                         )
                )(createPageRoutineHandler(pageModel, onPageRoutineFeature, defaultBackwardUrl, "EditConfirmed"));

        public static Func<Task<IActionResult>> ComposeDelete(
            PageModel pageModel,
            Action<TEntity> setPageEntity,
            Action<PageRoutineFeature> onPageRoutineFeature,
            string defaultBackwardUrl,
            Func<string, TUserContext, bool> authorize,
            Include<TEntity> deleteIncludes,
            Func<string, ValuableResult<TKey>> keyConverter,
            Func<TKey, Expression<Func<TEntity, bool>>> findPredicate,
            Func<PageModel, Action<PageRoutineFeature>, string, string, PageRoutineHandler<ComplexRoutineHandler<StorageRoutineHandler<TUserContext>, TUserContext>, TUserContext, TUser>> createPageRoutineHandler
            ) =>
                () => ComposePageRequestAsync(setPageEntity,
                    (repository, state) => steps =>
                        steps(
                            () => authorize?.Invoke("Delete", state.UserContext) ?? true,
                            keyConverter,
                            key => repository.Find(findPredicate(key), deleteIncludes)
                        )
                )(createPageRoutineHandler(pageModel, onPageRoutineFeature, defaultBackwardUrl, "Delete"));

        public static Func<Task<IActionResult>> ComposeDeleteConfirmed(
            PageModel pageModel,
            Action<TEntity> setPageEntity,
            Action<PageRoutineFeature> onPageRoutineFeature,
            string defaultBackwardUrl,
            Func<string, TUserContext, bool> authorize,
            Func<TEntity> constructor,
            Dictionary<string, Func<TEntity, Action<StringValues>>> hiddenFormFields,
            Func<PageModel, Action<PageRoutineFeature>, string, string, PageRoutineHandler<ComplexRoutineHandler<StorageRoutineHandler<TUserContext>, TUserContext>, TUserContext, TUser>> createPageRoutineHandler
            ) =>
                () => ComposePageSaveAsync(setPageEntity,/* defaultBackwardUrl,*/
                    (repository, state) => steps =>
                        steps(
                            null, // !! TODO
                            () => authorize?.Invoke("Delete", state.UserContext) ?? true,
                            request => MvcHandler.Bind(request, constructor, null, hiddenFormFields),
                            (entity, batch) => batch.Remove(entity)
                    )
                )(createPageRoutineHandler(pageModel, onPageRoutineFeature, defaultBackwardUrl, "DeleteConfirmed"));
        #endregion

    }
}