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
        readonly PageRoutineFeature pageRoutineFeature;
        readonly PageRoutineHandler<StorageRoutineHandler<TUserContext>, TUserContext, TUser> pageRoutineHandler;
        
        public CrudRoutinePageConsumer(
            PageModel pageModel,
            PageRoutineFeature pageRoutineFeature,
            PageRoutineHandler<StorageRoutineHandler<TUserContext>, TUserContext, TUser> pageRoutineHandler
            )
        {
            this.pageModel = pageModel;
            this.pageRoutineFeature = pageRoutineFeature;
            this.pageRoutineHandler = pageRoutineHandler;
        }

        #region Compose
        public Task<IActionResult> Handle(
            Func<MetaPageRoutineHandler<TUserContext, TUser>, Task<IActionResult>> func
        )
        {
            return func(new MetaPageRoutineHandler<TUserContext, TUser>(pageModel, pageRoutineHandler));
        }

        public Task<IActionResult> HandleAsync(Func<IRepository<TEntity>, RoutineClosure<TUserContext>, Task<IActionResult>> func
        )
        {
            return pageRoutineHandler.HandleAsync(async (container, closure) => await container.HandleStorageAsync<IActionResult, TEntity>(
                       repository => func(repository, closure)
                   ));
        }

        public  Func<Func<IEnumerable<TEntity>, IActionResult>, Task<IActionResult>>
            HandleAsync(
            Func<Func<IEnumerable<TEntity>, IActionResult>, Func<IRepository<TEntity>, RoutineClosure<TUserContext>, Task<IActionResult>>> func)
            => view => HandleAsync(func(view));

        public Task<IActionResult> HandlePageSaveAsync(
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
                > action)
            => Handle(
                async handler =>
                    await handler.HandlePageSaveAsync(
                        setPageEntity, pageRoutineFeature,
                        action
                    )
            );

        public Task<IActionResult> HandlePageSaveAsync(
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
                > action)
            => Handle(
                async handler =>
                    await handler.HandlePageSaveAsync(
                        setPageEntity, pageRoutineFeature,
                        action
                    )
            );

        public  Task<IActionResult> HandlePageRequestAsync(
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
            => Handle(
                async handler =>
                    await handler.HandlePageRequestAsync(
                        setPageEntity,
                        action
                    )
            );

        public  Task<IActionResult> HandlePageRequestAsync(
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
            => Handle(
                async routine =>
                    await routine.HandlePageRequestAsync(
                        setPageEntity,
                        action
                    )
            );

        public Task<IActionResult> HandlePageCreateAsync(
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
            => Handle(
                async routine =>
                    await routine.HandlePageCreateAsync(
                        setPageEntity,
                        action
                    )
            );
        #endregion

        #region Compose MVC Controlelr methods

        public Task<IActionResult> HandleIndexAsync(
            Action<IEnumerable<TEntity>> setPageEntity,
            Func<TUserContext, bool> authorize,
            Include<TEntity> indexIncludes
            ) =>
             HandleAsync(view => async (repository, closure) => {
                 if (!(authorize?.Invoke(closure.UserContext) ?? true))
                     return pageModel.Unauthorized();
                 var entities = await repository.ListAsync(indexIncludes);
                 return view(entities);
             })(o => { setPageEntity(o); return pageModel.Page(); });

        public Task<IActionResult> HandleDetailsAsync(
            Action<TEntity> setPageEntity,
            Func<TUserContext, bool> authorize,
            Include<TEntity> detailsIncludes,
            Func<string, ValuableResult<TKey>> keyConverter,
            Func<TKey, Expression<Func<TEntity, bool>>> findPredicate
            ) =>
               HandlePageRequestAsync(
                    setPageEntity,
                    (repository, closure) => steps =>
                        steps(
                            () => authorize?.Invoke(closure.UserContext) ?? true,
                            keyConverter,
                            key => repository.Find(findPredicate(key), detailsIncludes)
                            )
                    );

        public Task<IActionResult> HandleCreateAsync(
            Action<TEntity> setPageEntity,
            Func<TUserContext, bool> authorize,
            Action<Action<string, object>, IRepository<TEntity>> prepareEmptyOptions
            ) =>
                HandlePageCreateAsync(
                    setPageEntity,
                    (repository, closure) => steps =>
                        steps(
                            () => authorize?.Invoke(closure.UserContext) ?? true,
                            () => default,
                            (entity, addViewData) => prepareEmptyOptions(addViewData, repository)
                            )
                    );

        public Task<IActionResult> HandleCreateConfirmedAsync(
            Action<TEntity> setPageEntity,
            Func<TUserContext, bool> authorize,
            string formEntityPrefix,
            Func<TEntity> constructor,
            Dictionary<string, Func<TEntity, Func<StringValues, IVerboseResult<List<string>>>>> formFields,
            Dictionary<string, Func<TEntity, Action<StringValues>>> hiddenFormFields,
            Func<Action<string, object>, HttpRequest, IRepository<TEntity>, TEntity, IComplexBinderResult<ValueTuple<Action<IBatch<TEntity>>, Action>>> parseRelated
            ) =>
                HandlePageSaveAsync(setPageEntity, pageRoutineFeature,
                (repository, closure) => steps =>
                    steps(
                         () => authorize?.Invoke(closure.UserContext) ?? true,
                         request => MvcHandler.Bind(request, formEntityPrefix, constructor, formFields, hiddenFormFields),
                         (request, entity, addViewData) => parseRelated(addViewData, request, repository, entity),
                         (entity, batch) => batch.Add(entity)
                    )
                );

        public Task<IActionResult> HandleEditAsync(
            Action<TEntity> setPageEntity,
            Func< TUserContext, bool> authorize,
            Include<TEntity> editIncludes,
            Func<string, ValuableResult<TKey>> keyConverter,
            Func<TKey, Expression<Func<TEntity, bool>>> findPredicate,
            Func<Action<string, object>, IRepository<TEntity>, Action<TEntity>> parseRelated
            ) =>
                HandlePageRequestAsync(setPageEntity,
                    (repository, closure) => steps =>
                        steps(
                            () => authorize?.Invoke(closure.UserContext) ?? true,
                            keyConverter,
                            key => repository.Find(findPredicate(key), editIncludes),
                            (entity, addViewData) => parseRelated(addViewData, repository)(entity)
                        )
                );

        public Task<IActionResult> HandleEditConfirmedAsync(
            Action<TEntity> setPageEntity,
            Func<TUserContext, bool> authorize,
            string formEntityPrefix,
            Func<TEntity> constructor,
            Dictionary<string, Func<TEntity, Func<StringValues, IVerboseResult<List<string>>>>> formFields,
            Dictionary<string, Func<TEntity, Action<StringValues>>> hiddenFormFields,
            Include<TEntity> disabledFormFields,
            Func<Action<string, object>, HttpRequest, IRepository<TEntity>, TEntity, IComplexBinderResult<ValueTuple<Action<IBatch<TEntity>>, Action>>> parseRelated
            ) =>
                HandlePageSaveAsync(setPageEntity, pageRoutineFeature,
                    (repository, closure) => steps =>
                        steps(
                            () => authorize?.Invoke(closure.UserContext) ?? true,
                            request => MvcHandler.Bind(request, formEntityPrefix, constructor, formFields, hiddenFormFields),
                            (request, entity, addViewData) => parseRelated(addViewData, request, repository, entity),
                            (entity, batch) => batch.Modify(entity, disabledFormFields)
                         )
                );

        public Task<IActionResult> HandleDeleteAsync(
            Action<TEntity> setPageEntity,
            Func< TUserContext, bool> authorize,
            Include<TEntity> deleteIncludes,
            Func<string, ValuableResult<TKey>> keyConverter,
            Func<TKey, Expression<Func<TEntity, bool>>> findPredicate
            ) =>
                HandlePageRequestAsync(setPageEntity,
                    (repository, closure) => steps =>
                        steps(
                            () => authorize?.Invoke(closure.UserContext) ?? true,
                            keyConverter,
                            key => repository.Find(findPredicate(key), deleteIncludes)
                        )
                );

        public Task<IActionResult> HandleDeleteConfirmedAsync(
            Action<TEntity> setPageEntity,
            Func< TUserContext, bool> authorize,
            string formEntityPrefix,
            Func<TEntity> constructor,
            Dictionary<string, Func<TEntity, Action<StringValues>>> hiddenFormFields
            ) =>
                HandlePageSaveAsync(setPageEntity, pageRoutineFeature,
                    (repository, closure) => steps =>
                        steps(
                            () => authorize?.Invoke(closure.UserContext) ?? true,
                            request => MvcHandler.Bind(request, formEntityPrefix, constructor, null, hiddenFormFields),
                            (entity, batch) => batch.Remove(entity)
                    )
                );
        #endregion

    }
}