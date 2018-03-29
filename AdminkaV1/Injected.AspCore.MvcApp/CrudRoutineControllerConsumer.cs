using System;
using System.Threading.Tasks;
using System.Linq.Expressions;
using System.Collections.Generic;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Primitives;

using DashboardCode.Routines;
using DashboardCode.Routines.Storage;
using DashboardCode.Routines.AspNetCore;

namespace DashboardCode.AdminkaV1.Injected.AspCore.MvcApp
{
    class CrudRoutineControllerConsumer<TEntity, TKey> where TEntity: class, new()
    {
        readonly ConfigurableController controller;
        readonly ControllerMeta<TEntity, TKey> meta;
        readonly Func<string, UserContext, bool> authorize;
        public CrudRoutineControllerConsumer(
            ConfigurableController controller,
            ControllerMeta<TEntity, TKey> meta,
            Func<string, UserContext, bool> authorize
            ) 
        {
            this.controller = controller;
            this.meta = meta;
            this.authorize  = authorize;
        }

        #region Compose
        public static Func<ConfigurableController, Task<IActionResult>> Compose(Func<MvcRoutineHandler, Task<IActionResult>> func)
            => controller => func(new MvcRoutineHandler(controller));

        public static Func<ConfigurableController, Task<IActionResult>> ComposeAsync(Func<IRepository<TEntity>, Task<IActionResult>> func)
            => Compose(
                 routine =>
                     routine.HandleStorageAsync<IActionResult, TEntity>(
                        repository => func(repository)
                    )
            );

        public static Func<ConfigurableController, Task<IActionResult>> ComposeMvcSaveAsync(
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
                > action)
            => Compose(
                async routine =>
                    await routine.HandleMvcSaveAsync(
                        viewName,
                        action
                    )
            );

        public static Func<ConfigurableController, Task<IActionResult>> ComposeMvcSaveAsync(
            string viewName,
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
                    await routine.HandleMvcSaveAsync(
                        viewName,
                        action
                    )
            );

        public static Func<ConfigurableController, Task<IActionResult>> ComposeMvcRequestAsync(
            string viewName,
            Func<
                IRepository<TEntity>,
                Func<
                Func<
                    Func<string, ValuableResult<TKey>>,
                    Func<TKey, TEntity>,
                    Action<TEntity, Action<string, object>>,
                    IActionResult>,
                IActionResult
                >
            > action)
            => Compose(
                async routine =>
                    await routine.HandleMvcRequestAsync(
                        viewName,
                        action
                    )
            );

        public static Func<ConfigurableController, Task<IActionResult>> ComposeMvcRequestAsync(
            string viewName,
            Func<
                IRepository<TEntity>,
                Func<
                Func<
                    Func<string, ValuableResult<TKey>>,
                    Func<TKey, TEntity>,
                    IActionResult>,
                IActionResult
                >
            > action)
            => Compose(
                async routine =>
                    await routine.HandleMvcRequestAsync(
                        viewName,
                        action
                    )
            );

        public static Func<ConfigurableController, Task<IActionResult>> ComposeMvcCreateAsync(string viewName,
                 Func<
                    IRepository<TEntity>,
                    Func<
                        Func<
                            Func<TEntity>,
                            Action<TEntity, Action<string, object>>,
                            IActionResult>,
                        IActionResult>
                    > action)
            => Compose(
                async routine =>
                    await routine.HandleMvcCreateAsync(
                        viewName,
                        action
                    )
            );

        public static Func<ConfigurableController, Task<IActionResult>> ComposeMvcCreateAsync(string viewName,
                 Func<
                    IRepository<TEntity>,
                    Func<
                        Func<
                            Func<TEntity>,
                            IActionResult>,
                        IActionResult>
                    > action)
            => Compose(
                async routine =>
                    await routine.HandleMvcCreateAsync(
                        viewName,
                        action
                    )
            );

        public static Func<Func<object, IActionResult>, Func<ConfigurableController, Task<IActionResult>>> ComposeAsync(
            Func<Func<object, IActionResult>,  Func<IRepository<TEntity>, Task<IActionResult>>> func)
            => view => ComposeAsync(func(view));

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
        public static Func<Task<IActionResult>> ComposeIndex(ConfigurableController controller, Include<TEntity> indexIncludes) =>
             () => ComposeAsync( view => async repository => {
                var entities = await repository.ListAsync(indexIncludes);
                return view(entities);
             })(o=>controller.View("Index",o))(controller);

        public static Func<Task<IActionResult>> ComposeDetails(
            ConfigurableController controller, 
            Include<TEntity> detailsIncludes,
            Func<string, ValuableResult<TKey>> keyConverter,
            Func<TKey, Expression<Func<TEntity, bool>>> findPredicate
            ) =>
               () => ComposeMvcRequestAsync(
                   "Details",
                    repository => steps =>
                        steps(
                            keyConverter,
                            key => repository.Find(findPredicate(key), detailsIncludes)
                            )
                    )(controller);

        public static Func<Task<IActionResult>> ComposeCreate(
            ConfigurableController controller,
            Action<Action<string, object>, IRepository<TEntity>> prepareEmptyOptions
            ) =>
                () => ComposeMvcCreateAsync(
                    "Create",
                    repository => steps =>
                        steps(
                            ()=>default,
                            (entity, addViewData) => prepareEmptyOptions(addViewData, repository)
                            )
                    )(controller);

        public static Func<Task<IActionResult>> ComposeCreateConfirmed(
            ConfigurableController controller,
            Func<string, UserContext, bool> authorize,
            Func<TEntity> constructor,
            Dictionary<string, Func<TEntity, Func<StringValues, IVerboseResult<List<string>>>>> formFields,
            Dictionary<string, Func<TEntity, Action<StringValues>>> hiddenFormFields,
            Func<Action<string, object>, HttpRequest, IRepository<TEntity>, TEntity, IComplexBinderResult<ValueTuple<Action<IBatch<TEntity>>, Action>>> parseRelated
            ) =>
                () => ComposeMvcSaveAsync("Create",
                (repository, state) => steps =>
                    steps(
                         () => authorize(nameof(Create), state.UserContext),
                         request => MvcHandler.Bind(request, constructor, formFields, hiddenFormFields),
                         (request, entity, addViewData) => parseRelated(addViewData, request, repository, entity),
                         (entity, batch) => batch.Add(entity)
                    )
                )(controller);

        public static Func<Task<IActionResult>> ComposeEdit(
            ConfigurableController controller,
            Include<TEntity> editIncludes,
            Func<string, ValuableResult<TKey>> keyConverter,
            Func<TKey, Expression<Func<TEntity, bool>>> findPredicate,
            Func<Action<string, object>, IRepository<TEntity>, Action<TEntity>> parseRelated
            ) =>
                () => ComposeMvcRequestAsync("Edit",
                    repository => steps =>
                        steps(
                            keyConverter,
                            key => repository.Find(findPredicate(key), editIncludes),
                            (entity, addViewData) => parseRelated(addViewData, repository)(entity)
                        )
                )(controller);

        public static Func<Task<IActionResult>> ComposeEditConfirmed(
            ConfigurableController controller,
            Func<string, UserContext, bool> authorize,
            Func<TEntity> constructor,
            Dictionary<string, Func<TEntity, Func<StringValues, IVerboseResult<List<string>>>>> formFields,
            Dictionary<string, Func<TEntity, Action<StringValues>>> hiddenFormFields,
            Include<TEntity> disabledFormFields,
            Func<Action<string, object>, HttpRequest, IRepository<TEntity>, TEntity, IComplexBinderResult<ValueTuple<Action<IBatch<TEntity>>, Action>>> parseRelated
            ) =>
                () => ComposeMvcSaveAsync( "Edit",
                    (repository, state) => steps =>
                        steps(
                            () => authorize(nameof(Edit), state.UserContext),
                            request => MvcHandler.Bind(request, constructor, formFields, hiddenFormFields),
                            (request, entity, addViewData) => parseRelated(addViewData, request, repository, entity),
                            (entity, batch) => batch.Modify(entity, disabledFormFields)
                         )
                )(controller);

        public static Func<Task<IActionResult>> ComposeDelete(
            ConfigurableController controller,
            Include<TEntity> deleteIncludes,
            Func<string, ValuableResult<TKey>> keyConverter,
            Func<TKey, Expression<Func<TEntity, bool>>> findPredicate
            ) =>
                () => ComposeMvcRequestAsync("Delete",
                    repository => steps =>
                        steps(
                            keyConverter,
                            key => repository.Find(findPredicate(key), deleteIncludes)
                        )
                )(controller);

        public static Func<Task<IActionResult>> ComposeDeleteConfirmed(
            ConfigurableController controller,
            Func<string, UserContext, bool> authorize,
            Func<TEntity> constructor,
            Dictionary<string, Func<TEntity, Action<StringValues>>> hiddenFormFields
            ) =>
                () => ComposeMvcSaveAsync("Delete",
                    (repository, state) => steps =>
                        steps(
                            () => authorize(nameof(Delete), state.UserContext),
                            request => MvcHandler.Bind(request, constructor, null, hiddenFormFields),
                            (entity, batch) => batch.Remove(entity)
                    )
                )(controller);
        #endregion

        public  Task<IActionResult> Index()
        {
            var routine = new MvcRoutineHandler(controller);
            return routine.HandleStorageAsync<IActionResult, TEntity>(
                async repository => {
                    var entities = await repository.ListAsync(meta.IndexIncludes);
                    return controller.View(nameof(Index), entities);
                });
        }

        public async Task<IActionResult> Details()
        {
            var routine = new MvcRoutineHandler(controller);
            return await routine.HandleMvcRequestAsync<TKey, TEntity>(
                "Details",
                repository => steps =>
                    steps(
                        meta.KeyConverter, 
                        key => repository.Find(meta.FindPredicate(key), meta.DetailsIncludes))
            );
        }

        public async Task<IActionResult> Create()
        {
            var routine = new MvcRoutineHandler(controller);
            return await routine.HandleMvcCreateAsync<TEntity>(
                "Create",
                 repository => steps =>
                    steps(
                        () => meta.Constructor(),
                        (entity, addViewData) => meta.ReferencesCollection.PrepareEmptyOptions(addViewData, repository)
                    )
            );
        }

        public async Task<IActionResult> CreateConfirmed()
        {
            var routine = new MvcRoutineHandler(controller);
            return await routine.HandleMvcSaveAsync<TEntity>(
                "Create",
                (repository, state) => steps =>
                    steps(
                            () => authorize(nameof(Create), state.UserContext),
                            request => MvcHandler.Bind(request, meta.Constructor, meta.FormFields, meta.HiddenFormFields),
                            (request, entity, addViewData) => meta.ReferencesCollection.ParseRelated(addViewData, request, repository, entity),
                            (entity, batch) => batch.Add(entity)
                        )
            );
        }

        public async Task<IActionResult> Edit()
        {
            var routine = new MvcRoutineHandler(controller);
            return await routine.HandleMvcRequestAsync<TKey, TEntity>(
                "Edit",
                 repository => steps =>
                      steps(
                meta.KeyConverter,
                key => repository.Find(meta.FindPredicate(key), meta.EditIncludes),
                (entity, addViewData) => meta.ReferencesCollection.PrepareOptions(addViewData, repository)(entity))
            );
        }

        public async Task<IActionResult> EditConfirmed()
        {
            var routine = new MvcRoutineHandler(controller);
            return await routine.HandleMvcSaveAsync<TEntity>(
                "Edit",
                (repository, state) => steps =>
                    steps(
                        () => authorize(nameof(Edit), state.UserContext),
                        request => MvcHandler.Bind(request, meta.Constructor, meta.FormFields, meta.HiddenFormFields),
                        (request, entity, addViewData) => meta.ReferencesCollection.ParseRelated(addViewData, request, repository, entity),
                        (entity, batch) => batch.Modify(entity, meta.DisabledFormFields)
                    )
            );
        }

        public async Task<IActionResult> Delete()
        {
            var routine = new MvcRoutineHandler(controller);
            return await routine.HandleMvcRequestAsync<TKey, TEntity>(
                "Delete",
                repository => steps =>
                      steps(
                          meta.KeyConverter,
                          key => repository.Find(meta.FindPredicate(key), meta.DeleteIncludes)
                          )
            );
        }

        public async Task<IActionResult> DeleteConfirmed()
        {
            var routine = new MvcRoutineHandler(controller);
            return await routine.HandleMvcSaveAsync<TEntity>(
                "Delete",
                (repository, state) => steps =>
                steps(
                    () => authorize(nameof(Delete), state.UserContext),
                    request => MvcHandler.Bind(request, meta.Constructor, meta.FormFields, meta.HiddenFormFields),
                    (entity, batch) => batch.Remove(entity)
                )
            );
        }
    }
}