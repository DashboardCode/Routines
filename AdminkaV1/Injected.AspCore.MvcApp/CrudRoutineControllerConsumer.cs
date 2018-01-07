using System;
using System.Threading.Tasks;
using System.Linq.Expressions;
using System.Collections.Generic;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Primitives;
using Microsoft.AspNetCore.Mvc;
using System.Net;

using DashboardCode.Routines;
using DashboardCode.Routines.Storage;
using DashboardCode.Routines.AspNetCore;

namespace DashboardCode.AdminkaV1.Injected.AspCore.MvcApp
{
    //public class ControllerAdapter
    //{
    //    readonly RoutineController controller;
    //    public ControllerAdapter(RoutineController controller)
    //    {
    //        this.controller = controller;
    //    }

    //    public bool IsVal
    //}

    class CrudRoutineControllerConsumer<TEntity, TKey> where TEntity: class, new()
    {
        readonly ControllerMeta<TEntity, TKey> meta;
        readonly ConfigurableController controller;
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
        public static Func<ConfigurableController, Task<IActionResult>> Compose(string action, Func<MvcRoutine, Task<IActionResult>> func) 
            => controller => func(new MvcRoutine(controller, action));

        public static Func<ConfigurableController, Task<IActionResult>> ComposeAsync(string action, Func<IRepository<TEntity>, IActionResult> func)
            => Compose(action,
                async routine =>
                    await routine.HandleStorageAsync<IActionResult, TEntity>(
                        repository => func(repository)
                    )
            );

        public static Func<ConfigurableController, Task<IActionResult>> ComposeAsync(string action, Func<IRepository<TEntity>, IOrmStorage<TEntity>, Routine<UserContext>, IActionResult> func)
            => Compose(action,
                async routine =>
                    await routine.HandleStorageAsync<IActionResult, TEntity>(
                        (repository, storage, state) => func(repository, storage, state)
                    )
            );

        public static Func<Func<object, IActionResult>, Func<ConfigurableController, Task<IActionResult>>> ComposeAsync(
            string action, Func<Func<object, IActionResult>,  Func<IRepository<TEntity>, IActionResult>> func)
            => view => ComposeAsync(action, func(view));

        public static Func<Func<object, IActionResult>, Action<string,object>, Func<ConfigurableController, Task<IActionResult>>> ComposeAsync(
            string action, Func<Func<object, IActionResult>, Action<string, object>, Func<IRepository<TEntity>, IActionResult>> func)
            => (view, addViewData) => ComposeAsync(action, func(view, addViewData));

        public static Func<Func<object, IActionResult>, Func<IActionResult>, Func<ConfigurableController, Task<IActionResult>>> ComposeAsync(
            string action, Func<Func<object, IActionResult>, Func<IActionResult>, Func<IRepository<TEntity>, IActionResult>> func)
            => (view, notFound) => ComposeAsync(action, func(view, notFound));

        public static Func<Func<object, IActionResult>, Func<IActionResult>, Action<string, object>,  Func<ConfigurableController, Task<IActionResult>>> ComposeAsync(
            string action, Func<Func<object, IActionResult>, Func<IActionResult>, Action<string, object>, Func<IRepository<TEntity>, IActionResult>> func)
            => (view, notFound, addViewData) => ComposeAsync(action, func(view, notFound, addViewData));

        public static Func<Func<object, IActionResult>, Func<IActionResult>, Action<string, string>, Action<string, object>, Func<IActionResult>, Func<ConfigurableController, Task<IActionResult>>> ComposeAsync(
            string action, Func<Func<object, IActionResult>, Func<IActionResult>, Action<string, string>, Action<string, object>, Func<IActionResult>, Func<IRepository<TEntity>, IOrmStorage<TEntity>, Routine<UserContext>, IActionResult>> func)
            => (view, unauthorized, addModelError, addViewData, redirect) => ComposeAsync(action, func(view, unauthorized, addModelError, addViewData, redirect));

        public static Func<Func<object, IActionResult>, Func<IActionResult>, Action<string, string>, Func<bool>, Action<string, object>, Func<IActionResult>, Func<ConfigurableController, Task<IActionResult>>> ComposeAsync(
            string action, Func<Func<object, IActionResult>, Func<IActionResult>, Action<string, string>, Func<bool>, Action<string, object>, Func<IActionResult>, Func<IRepository<TEntity>, IOrmStorage<TEntity>, Routine<UserContext>, IActionResult>> func)
            => (view, unauthorized, addModelError, isValid, addViewData, redirect) => ComposeAsync(action, func(view, unauthorized, addModelError, isValid, addViewData, redirect));

        public static Func<HttpRequest, Func<Func<object, IActionResult>, Func<IActionResult>, Func<ConfigurableController, Task<IActionResult>>>> ComposeAsync(
            string action, Func<HttpRequest, Func<Func<object, IActionResult>, Func<IActionResult>, Func<IRepository<TEntity>, IActionResult>>> func)
            => httpRequest => ComposeAsync(action, func(httpRequest));

        public static Func<HttpRequest, Func<Func<object, IActionResult>, Func<IActionResult>,  Action<string, object>, Func<ConfigurableController, Task<IActionResult>>>> ComposeAsync(
            string action, Func<HttpRequest, Func<Func<object, IActionResult>, Func<IActionResult>, Action<string, object>, Func<IRepository<TEntity>, IActionResult>>> func)
            => httpRequest => ComposeAsync(action, func(httpRequest));

        public static Func<HttpRequest, Func<Func<object, IActionResult>, Func<IActionResult>, Action<string, string>, Func<bool>, Action<string, object>, Func<IActionResult>, Func<ConfigurableController, Task<IActionResult>>>> ComposeAsync(
            string action, Func<HttpRequest, Func<Func<object, IActionResult>, Func<IActionResult>, Action<string, string>, Func<bool>, Action<string, object>, Func<IActionResult>, Func<IRepository<TEntity>, IOrmStorage<TEntity>, Routine<UserContext>, IActionResult>>> func)
            => httpRequest => ComposeAsync(action, func(httpRequest));

        public static Func<HttpRequest, Func<Func<object, IActionResult>, Func<IActionResult>, Action<string, string>, Action<string, object>, Func<IActionResult>, Func<ConfigurableController, Task<IActionResult>>>> ComposeAsync(
            string action, Func<HttpRequest, Func<Func<object, IActionResult>, Func<IActionResult>, Action<string, string>, Action<string, object>, Func<IActionResult>, Func<IRepository<TEntity>, IOrmStorage<TEntity>, Routine<UserContext>, IActionResult>>> func)
            => httpRequest => ComposeAsync(action, func(httpRequest));

        public static Func<Task<IActionResult>> ComposeIndex(ConfigurableController controller, Include<TEntity> indexIncludes) =>
             () => ComposeAsync("Index",  view => repository => {
                var entities = repository.List(indexIncludes);
                return view(entities);
             })(o=>controller.View("Index",o))(controller);

        public static Func<Task<IActionResult>> ComposeDetails(
            ConfigurableController controller, 
            Include<TEntity> detailsIncludes,
            Func<string, ValuableResult<TKey>> keyConverter,
            Func<TKey, Expression<Func<TEntity, bool>>> findPredicate) =>
             () => ComposeAsync("Details", request => (view,notFound) => repository => MvcHandler.MakeActionResultOnEntityRequest(
                      keyConverter,
                      request,
                      view,
                      notFound,
                      key => repository.Find(findPredicate(key), detailsIncludes)
                 )
             )(controller.HttpContext.Request)(o => controller.View("Details", o), ()=>controller.NotFound())(controller);

        public static Func<Task<IActionResult>> ComposeCreate(
            ConfigurableController controller,
            Action<Action<string, object>, IRepository<TEntity>> prepareEmptyOptions
            ) =>
            () => ComposeAsync("Create", (view,addViewBag) => repository =>
            {
                prepareEmptyOptions(addViewBag, repository);
                return view(default(TEntity));
            })(o => controller.View("Create", o), (s, o) => controller.ViewData[s] = o)(controller);

        public static Func<Task<IActionResult>> ComposeCreateConfirmed(
            ConfigurableController controller,
            Func<string, UserContext, bool> authorize,
            Func<TEntity> constructor,
            Dictionary<string, Func<TEntity, Func<StringValues, IVerboseResult<List<string>>>>> formFields,
            Dictionary<string, Func<TEntity, Action<StringValues>>> hiddenFormFields,
            Func<Action<string, object>, HttpRequest, IRepository<TEntity>, TEntity, IComplexBinderResult<ValueTuple<Action<IBatch<TEntity>>, Action>>> parseRelated) =>
             () => ComposeAsync("CreateConfirmed", request2 => (view, unauthorized, addModelError, isValid, addViewData2, redirect) => (repository2, storage2, state2) =>
             {
                 return MvcHandler.MakeActionResultOnEntitySave3(
                     repository2, storage2, state2,
                     unauthorized,
                     request2,
                     addViewData2,
                     addModelError,
                     redirect,
                     view,
                     (repository, state) => steps =>
                         steps(
                             () => authorize(nameof(Create), state.UserContext),
                             request => MvcHandler.Bind(request, constructor, formFields, hiddenFormFields),
                             (request, entity, addViewData) => parseRelated(addViewData, request, repository, entity),
                             (entity, batch) => batch.Add(entity)
                     )
                 );
             })(controller.HttpContext.Request)(o => controller.View("Create", o), () => controller.Unauthorized(),
                 (p, e) => controller.ModelState.AddModelError(p, e), ()=> controller.ModelState.IsValid,
                 (s, o) => controller.ViewData[s] = o, () => controller.RedirectToAction("Index"))(controller);

        public static Func<Task<IActionResult>> ComposeEdit(
            ConfigurableController controller,
            Func<Action<string, object>, IRepository<TEntity>, Action<TEntity>> prepareOptions,
            Func<string, ValuableResult<TKey>> keyConverter,
            Func<TKey, Expression<Func<TEntity, bool>>> findPredicate,
            Include<TEntity> editIncludes
            ) =>
             () => ComposeAsync("Edit", request => (view, notFound, addViewData) => repository =>
                MvcHandler.MakeActionResultOnEntityRequest(
                    keyConverter,
                    request,
                    view,
                    notFound,
                    key => repository.Find(findPredicate(key), editIncludes)
                )
             )(controller.HttpContext.Request)(o => controller.View("Edit", o), () => controller.NotFound(), (n, o) => controller.ViewData.Add(n, o))(controller);

        public static Func<Task<IActionResult>> ComposeEditConfirmed(
            ConfigurableController controller,
            Func<string, UserContext, bool> authorize,
            Func<TEntity> constructor,
            Dictionary<string, Func<TEntity, Func<StringValues, IVerboseResult<List<string>>>>> formFields,
            Dictionary<string, Func<TEntity, Action<StringValues>>> hiddenFormFields,
            Include<TEntity> disabledFormFields,
            Func<Action<string, object>, HttpRequest, IRepository<TEntity>, TEntity, IComplexBinderResult<ValueTuple<Action<IBatch<TEntity>>, Action>>> parseRelated) =>
             () => ComposeAsync("EditConfirmed", request2 => (view, unauthorized, addModelError, isValid, addViewData2, redirect) => (repository2, storage2, state2) =>
             {
                 return MvcHandler.MakeActionResultOnEntitySave3(
                        repository2, storage2, state2,
                        unauthorized,
                        request2,
                        addViewData2,
                        addModelError,
                        redirect,
                        view,
                        (repository, state) => steps => 
                            steps(
                                () => authorize(nameof(Edit), state.UserContext),
                                request => MvcHandler.Bind(request, constructor, formFields, hiddenFormFields),
                                (request, entity, addViewData) => parseRelated(addViewData, request, repository, entity),
                                (entity, batch) => batch.Modify(entity, disabledFormFields)
                        )
                    );
             })(controller.HttpContext.Request)(o => controller.View("Edit", o), () => controller.Unauthorized(),
                 (p, e) => controller.ModelState.AddModelError(p, e), () => controller.ModelState.IsValid,
                 (s, o) => controller.ViewData[s] = o, () => controller.RedirectToAction("Index"))(controller);

        public static Func<Task<IActionResult>> ComposeDelete(
            ConfigurableController controller,
            Include<TEntity> deleteIncludes,
            Func<string, ValuableResult<TKey>> keyConverter,
            Func<TKey, Expression<Func<TEntity, bool>>> findPredicate) =>
            () => ComposeAsync("Delete", request => (view, notFound) => repository => 
                MvcHandler.MakeActionResultOnEntityRequest(
                    keyConverter,
                    request,
                    view,
                    notFound,
                    key => repository.Find(findPredicate(key), deleteIncludes)
                )
            )(controller.HttpContext.Request)(o => controller.View("Delete", o), () => controller.NotFound())(controller);

        public static Func<Task<IActionResult>> ComposeDeleteConfirmed(
            ConfigurableController controller,
            Func<string, UserContext, bool> authorize,
            Func<string, ValuableResult<TKey>> keyConverter,
            Func<TKey, Expression<Func<TEntity, bool>>> findPredicate) =>
         () => ComposeAsync("DeleteConfirmed", request2 => (view, unauthorized, addModelError, addViewData2, redirect) => (repository2, storage2, state2) =>
         {
             return MvcHandler.MakeActionResultOnEntitySave3(
                        repository2, storage2, state2,
                        unauthorized,
                        controller.HttpContext.Request,
                        addViewData2,
                        addModelError,
                        redirect,
                        (e) => controller.View(nameof(Edit), e),
                        (repository, state) => steps =>
                            steps(
                                () => authorize(nameof(Delete), state.UserContext),
                                r =>
                                {
                                    var (id, valid) = r.BindId(keyConverter);
                                    var entity = repository.Find(findPredicate(id.Value));
                                    return new ComplexBinderResult<TEntity>(entity, null);
                                },
                                null,
                                (entity, batch) => batch.Remove(entity)
                        )
                    );
         })(controller.HttpContext.Request)(o => controller.View("Delete", o), () => controller.Unauthorized(),
             (p, e) => controller.ModelState.AddModelError(p, e),
             (s, o) => controller.ViewData[s] = o, () => controller.RedirectToAction("Index"))(controller);
        #endregion

        public async Task<IActionResult> Index()
        {
            var routine = new MvcRoutine(controller, null);
            return await routine.HandleStorageAsync<IActionResult, TEntity>(
                repository => {
                    var entities = repository.List(meta.IndexIncludes);
                    return controller.View(nameof(Index), entities);
                });
        }

        public async Task<IActionResult> Details()
        {
            var routine = new MvcRoutine(controller);
            return await routine.HandleStorageAsync<IActionResult, TEntity>(repository =>
            {
                var (id, valid) = controller.HttpContext.Request.BindId(meta.KeyConverter);
                return MvcHandler.MakeActionResultOnEntityRequest(
                    meta.KeyConverter,
                    controller.HttpContext.Request,
                    o => controller.View(nameof(Details), o),
                    controller.NotFound,
                    key => repository.Find(meta.FindPredicate(key), meta.DetailsIncludes)
                );
            });
        }

        public async Task<IActionResult> Create()
        {
            var routine = new MvcRoutine(controller, null);
            return await routine.HandleStorageAsync<IActionResult, TEntity>(repository =>
            {
                meta.ReferencesCollection.PrepareEmptyOptions((s, o) => controller.ViewData[s] = o, repository);
                return controller.View(nameof(Create));
            });
        }

        public async Task<IActionResult> CreateConfirmed()
        {
            var routine = new MvcRoutine(controller);
            return await routine.HandleStorageAsync<IActionResult, TEntity>((repository2, storage2, state2) =>
            {
            return MvcHandler.MakeActionResultOnEntitySave3(
                    repository2, storage2, state2,
                    () => controller.Unauthorized(),
                    controller.HttpContext.Request,
                    (n, v) => controller.ViewData[n] = v,
                    (n ,v) => controller.ModelState.AddModelError(n,v),
                    () => controller.RedirectToAction(nameof(Index)),
                    e => controller.View(nameof(Create), e),
                    (repository, state) => steps =>
                        steps(
                                () => authorize(nameof(Create), state.UserContext),
                                request => MvcHandler.Bind(request, meta.Constructor, meta.FormFields, meta.HiddenFormFields),
                                (request, entity, addViewData) => meta.ReferencesCollection.ParseRelated(addViewData, request, repository, entity),
                                (entity, batch) => batch.Add(entity)
                            )
                        );
            });
        }

        public async Task<IActionResult> Edit()
        {
            var routine = new MvcRoutine(controller);
            return await routine.HandleStorageAsync<IActionResult, TEntity>(repository =>
            {
                var (id, valid) = controller.HttpContext.Request.BindId(meta.KeyConverter);

                return MvcHandler.MakeActionResultOnEntityRequest(
                    meta.KeyConverter,
                    controller.HttpContext.Request,
                    o => controller.View(nameof(Edit), o),
                    controller.NotFound,
                    key => repository.Find(meta.FindPredicate(key), meta.EditIncludes),
                    entity => meta.ReferencesCollection.PrepareOptions((s, o) => controller.ViewData[s] = o, repository)
                );
            });
        }

        public async Task<IActionResult> EditConfirmed()
        {
            var routine = new MvcRoutine(controller);
            return await routine.HandleStorageAsync<IActionResult, TEntity>(
                (repository2, storage2, state2) =>
                {
                    return MvcHandler.MakeActionResultOnEntitySave3(
                        repository2, storage2, state2,
                        () => controller.Unauthorized(),
                        controller.HttpContext.Request,
                        (n, v) => controller.ViewData[n] = v,
                        (n,v)=> controller.ModelState.AddModelError(n, v),
                        () => controller.RedirectToAction(nameof(Index)),
                        (e) => controller.View(nameof(Edit), e),
                        (repository, state) => steps =>
                            steps(
                                () => authorize(nameof(Edit), state.UserContext),
                                request => MvcHandler.Bind(request, meta.Constructor, meta.FormFields, meta.HiddenFormFields),
                                (request, entity, addViewData) => meta.ReferencesCollection.ParseRelated(addViewData, request, repository, entity),
                                (entity, batch) => batch.Modify(entity, meta.DisabledFormFields)
                        )
                    );
                });
        }

        public async Task<IActionResult> Delete()
        {
            var routine = new MvcRoutine(controller);
            return await routine.HandleStorageAsync<IActionResult, TEntity>(repository =>
            {
                return MvcHandler.MakeActionResultOnEntityRequest(
                    meta.KeyConverter,
                    controller.HttpContext.Request,
                    o => controller.View(nameof(Delete), o),
                    controller.NotFound,
                    key => repository.Find(meta.FindPredicate(key), meta.DeleteIncludes)
                 );
            });
        }

        public async Task<IActionResult> DeleteConfirmed()
        {
            var routine = new MvcRoutine(controller);
            return await routine.HandleStorageAsync<IActionResult, TEntity>((repository2, storage2, state2) =>
            {
                return MvcHandler.MakeActionResultOnEntitySave3(
                    repository2,
                    storage2,
                    state2,
                    () => controller.Unauthorized(),
                    controller.HttpContext.Request,
                    (n, v) => controller.ViewData[n] = v,
                    (n, v) => controller.ModelState.AddModelError(n, v),
                    () => controller.RedirectToAction(nameof(Index)),
                    (e) => controller.View(nameof(Delete), e),
                    (repository, state) => steps =>
                        steps(
                            () => authorize(nameof(Delete), state.UserContext),
                            request =>
                            {
                                var (id, valid) = request.BindId(meta.KeyConverter);
                                var entity = repository.Find(meta.FindPredicate(id.Value));
                                return new ComplexBinderResult<TEntity>(entity, null);
                            },
                            null,
                            (entity, batch) => batch.Remove(entity)
                       )
                );
            });
        }

        
    }
}