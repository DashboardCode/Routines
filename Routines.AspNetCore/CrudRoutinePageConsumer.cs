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
        readonly PageModel pageModel;
        readonly PageRoutineHandler<StorageRoutineHandler<TUserContext>, TUserContext, TUser> pageRoutineHandler;
        readonly PageRoutineFeature pageRoutineFeature;
        public CrudRoutinePageConsumer(
            PageModel pageModel,
            PageRoutineFeature pageRoutineFeature,
            PageRoutineHandler<StorageRoutineHandler<TUserContext>, TUserContext, TUser> pageRoutineHandler
            )
        {
            this.pageModel = pageModel;
            this.pageRoutineHandler = pageRoutineHandler;
            this.pageRoutineFeature = pageRoutineFeature;
        }

        #region Compose
        public Task<IActionResult> Compose(
            Func<MetaPageRoutineHandler<TUserContext, TUser>, Task<IActionResult>> func
        )
        {
            return func(new MetaPageRoutineHandler<TUserContext, TUser>(pageModel, pageRoutineHandler));
        }

        public Task<IActionResult> ComposeAsync(Func<IRepository<TEntity>, RoutineClosure<TUserContext>, Task<IActionResult>> func
        )
        {
            return pageRoutineHandler.HandleAsync(async (container, closure) => await container.HandleStorageAsync<IActionResult, TEntity>(
                       repository => func(repository, closure)
                   ));
        }

        public  Func<Func<IEnumerable<TEntity>, IActionResult>, Task<IActionResult>>
            ComposeAsync(
            Func<Func<IEnumerable<TEntity>, IActionResult>, Func<IRepository<TEntity>, RoutineClosure<TUserContext>, Task<IActionResult>>> func)
            => view => ComposeAsync(func(view));

        public Task<IActionResult> ComposePageSaveAsync(
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

        public Task<IActionResult> ComposePageSaveAsync(
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

        public  Task<IActionResult> ComposePageRequestAsync(
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

        public  Task<IActionResult> ComposePageRequestAsync(
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

        public Task<IActionResult> ComposePageCreateAsync(
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

        //public Task<IActionResult> ComposePageCreateAsync(
        //         Action<object> setPageEntity,
        //         Func<
        //            IRepository<TEntity>,
        //            RoutineClosure<TUserContext>,
        //            Func<
        //                Func<
        //                    Func<bool>,
        //                    Func<TEntity>,
        //                    IActionResult>,
        //                IActionResult>
        //            > action)
        //    => Compose(
        //        async routine =>
        //            await routine.HandlePageCreateAsync(
        //                setPageEntity,
        //                action
        //            )
        //    );
        #endregion

        #region Compose MVC Controlelr methods

        public Task<IActionResult> ComposeIndex(
            Action<IEnumerable<TEntity>> setPageEntity,
            Func<TUserContext, bool> authorize,
            Include<TEntity> indexIncludes
            ) =>
             ComposeAsync(view => async (repository, state) => {
                 if (!(authorize?.Invoke(state.UserContext) ?? true))
                     return pageModel.Unauthorized();
                 var entities = await repository.ListAsync(indexIncludes);
                 return view(entities);
             })(o => { setPageEntity(o); return pageModel.Page(); });

        public Task<IActionResult> ComposeDetails(
            Action<TEntity> setPageEntity,
            Func<TUserContext, bool> authorize,
            Include<TEntity> detailsIncludes,
            Func<string, ValuableResult<TKey>> keyConverter,
            Func<TKey, Expression<Func<TEntity, bool>>> findPredicate
            ) =>
               ComposePageRequestAsync(
                    setPageEntity,
                    (repository, state) => steps =>
                        steps(
                            () => authorize?.Invoke(state.UserContext) ?? true,
                            keyConverter,
                            key => repository.Find(findPredicate(key), detailsIncludes)
                            )
                    );

        public Task<IActionResult> ComposeCreate(
            Action<TEntity> setPageEntity,
            Func<TUserContext, bool> authorize,
            Action<Action<string, object>, IRepository<TEntity>> prepareEmptyOptions
            ) =>
                ComposePageCreateAsync(
                    setPageEntity,
                    (repository, state) => steps =>
                        steps(
                            () => authorize?.Invoke(state.UserContext) ?? true,
                            () => default,
                            (entity, addViewData) => prepareEmptyOptions(addViewData, repository)
                            )
                    );

        public Task<IActionResult> ComposeCreateConfirmed(
            Action<TEntity> setPageEntity,
            Func<TUserContext, bool> authorize,
            Func<TEntity> constructor,
            Dictionary<string, Func<TEntity, Func<StringValues, IVerboseResult<List<string>>>>> formFields,
            Dictionary<string, Func<TEntity, Action<StringValues>>> hiddenFormFields,
            Func<Action<string, object>, HttpRequest, IRepository<TEntity>, TEntity, IComplexBinderResult<ValueTuple<Action<IBatch<TEntity>>, Action>>> parseRelated
            ) =>
                ComposePageSaveAsync(setPageEntity, /* defaultBackwardUrl,*/
                (repository, closure) => steps =>
                    steps(
                         pageRoutineFeature,
                         () => authorize?.Invoke(closure.UserContext) ?? true,
                         request => MvcHandler.Bind(request, constructor, formFields, hiddenFormFields),
                         (request, entity, addViewData) => parseRelated(addViewData, request, repository, entity),
                         (entity, batch) => batch.Add(entity)
                    )
                );

        public Task<IActionResult> ComposeEdit(
            Action<TEntity> setPageEntity,
            Func< TUserContext, bool> authorize,
            Include<TEntity> editIncludes,
            Func<string, ValuableResult<TKey>> keyConverter,
            Func<TKey, Expression<Func<TEntity, bool>>> findPredicate,
            Func<Action<string, object>, IRepository<TEntity>, Action<TEntity>> parseRelated
            ) =>
                ComposePageRequestAsync(setPageEntity,
                    (repository, state) => steps =>
                        steps(
                            () => authorize?.Invoke(state.UserContext) ?? true,
                            keyConverter,
                            key => repository.Find(findPredicate(key), editIncludes),
                            (entity, addViewData) => parseRelated(addViewData, repository)(entity)
                        )
                );

        public Task<IActionResult> ComposeEditConfirmed(
            Action<TEntity> setPageEntity,
            Func<TUserContext, bool> authorize,
            Func<TEntity> constructor,
            Dictionary<string, Func<TEntity, Func<StringValues, IVerboseResult<List<string>>>>> formFields,
            Dictionary<string, Func<TEntity, Action<StringValues>>> hiddenFormFields,
            Include<TEntity> disabledFormFields,
            Func<Action<string, object>, HttpRequest, IRepository<TEntity>, TEntity, IComplexBinderResult<ValueTuple<Action<IBatch<TEntity>>, Action>>> parseRelated
            ) =>
                ComposePageSaveAsync(setPageEntity, /* defaultBackwardUrl,*/
                    (repository, closure) => steps =>
                        steps(
                            pageRoutineFeature, 
                            () => authorize?.Invoke(closure.UserContext) ?? true,
                            request => MvcHandler.Bind(request, constructor, formFields, hiddenFormFields),
                            (request, entity, addViewData) => parseRelated(addViewData, request, repository, entity),
                            (entity, batch) => batch.Modify(entity, disabledFormFields)
                         )
                );

        public Task<IActionResult> ComposeDelete(
            Action<TEntity> setPageEntity,
            Func< TUserContext, bool> authorize,
            Include<TEntity> deleteIncludes,
            Func<string, ValuableResult<TKey>> keyConverter,
            Func<TKey, Expression<Func<TEntity, bool>>> findPredicate
            ) =>
                ComposePageRequestAsync(setPageEntity,
                    (repository, state) => steps =>
                        steps(
                            () => authorize?.Invoke(state.UserContext) ?? true,
                            keyConverter,
                            key => repository.Find(findPredicate(key), deleteIncludes)
                        )
                );

        public Task<IActionResult> ComposeDeleteConfirmed(
            Action<TEntity> setPageEntity,
            Func< TUserContext, bool> authorize,
            Func<TEntity> constructor,
            Dictionary<string, Func<TEntity, Action<StringValues>>> hiddenFormFields
            ) =>
                ComposePageSaveAsync(setPageEntity,/* defaultBackwardUrl,*/
                    (repository, state) => steps =>
                        steps(
                            pageRoutineFeature, 
                            () => authorize?.Invoke(state.UserContext) ?? true,
                            request => MvcHandler.Bind(request, constructor, null, hiddenFormFields),
                            (entity, batch) => batch.Remove(entity)
                    )
                );
        #endregion

    }
}