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

        public static Func<Func<object, IActionResult>, Func<IActionResult>, Func<bool>, Action<string, object>,  Func<ConfigurableController, Task<IActionResult>>> ComposeAsync(
            string action, Func<Func<object, IActionResult>, Func<IActionResult>, Func<bool>, Action<string, object>, Func<IRepository<TEntity>, IActionResult>> func)
            => (view, notFound, isValid, addViewData) => ComposeAsync(action, func(view, notFound, isValid, addViewData));

        public static Func<Func<object, IActionResult>, Func<IActionResult>, Action<string, string>, Action<string, object>, Func<IActionResult>, Func<ConfigurableController, Task<IActionResult>>> ComposeAsync(
            string action, Func<Func<object, IActionResult>, Func<IActionResult>, Action<string, string>, Action<string, object>, Func<IActionResult>, Func<IRepository<TEntity>, IOrmStorage<TEntity>, Routine<UserContext>, IActionResult>> func)
            => (view, unauthorized, addModelError, addViewData, redirect) => ComposeAsync(action, func(view, unauthorized, addModelError, addViewData, redirect));

        public static Func<Func<object, IActionResult>, Func<IActionResult>, Action<string, string>, Func<bool>, Action<string, object>, Func<IActionResult>, Func<ConfigurableController, Task<IActionResult>>> ComposeAsync(
            string action, Func<Func<object, IActionResult>, Func<IActionResult>, Action<string, string>, Func<bool>, Action<string, object>, Func<IActionResult>, Func<IRepository<TEntity>, IOrmStorage<TEntity>, Routine<UserContext>, IActionResult>> func)
            => (view, unauthorized, addModelError, isValid, addViewData, redirect) => ComposeAsync(action, func(view, unauthorized, addModelError, isValid, addViewData, redirect));

        public static Func<HttpRequest, Func<Func<object, IActionResult>, Func<IActionResult>, Func<ConfigurableController, Task<IActionResult>>>> ComposeAsync(
            string action, Func<HttpRequest, Func<Func<object, IActionResult>, Func<IActionResult>, Func<IRepository<TEntity>, IActionResult>>> func)
            => httpRequest => ComposeAsync(action, func(httpRequest));

        public static Func<HttpRequest, Func<Func<object, IActionResult>, Func<IActionResult>, Func<bool>, Action<string, object>, Func<ConfigurableController, Task<IActionResult>>>> ComposeAsync(
            string action, Func<HttpRequest, Func<Func<object, IActionResult>, Func<IActionResult>, Func<bool>, Action<string, object>, Func<IRepository<TEntity>, IActionResult>>> func)
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
            Func<string, ConvertFuncResult<TKey>> keyConverter,
            Func<TKey, Expression<Func<TEntity, bool>>> findPredicate) =>
             () => ComposeAsync("Details", request => (view,notFound) => repository => {
                 var (id, valid) = request.BindId(keyConverter);
                 return MvcHandler.MakeActionResultOnEntityRequest(
                      view,
                      notFound,
                      valid,
                      () => repository.Find(findPredicate(id.Value), detailsIncludes)
                 );
             })(controller.HttpContext.Request)(o => controller.View("Details", o), ()=>controller.NotFound())(controller);

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
            Dictionary<string, Func<TEntity, Func<StringValues, VerboseResult>>> editableBinders,
            Dictionary<string, Func<TEntity, Action<StringValues>>> notEditableBinders,
            Func<Action<string, object>, HttpRequest, TEntity, IRepository<TEntity>, ValueTuple<Action<IBatch<TEntity>>, Action>> parseRelated) =>
             () => ComposeAsync("CreateConfirmed", request => (view, unauthorized, addModelError, isValid, addViewData, redirect) => (repository, storage, state) =>
             {
                 if (!authorize("Create", state.UserContext))
                     return unauthorized();

                 TEntity entity = ControllerExtensions.Bind(request, addModelError, constructor, editableBinders, notEditableBinders);

                 var (modifyRelated, setViewDataMultiSelectLists) = parseRelated(addViewData, request, entity, repository);

                 return MvcHandler.MakeActionResultOnEntitySave(
                     addViewData,
                     addModelError,
                     isValid(),
                     () => storage.Handle(
                         batch =>
                         {
                             batch.Add(entity);
                             modifyRelated(batch);
                         }),
                     () =>
                     {
                         setViewDataMultiSelectLists();
                         return view(entity);
                     },
                     redirect
                 );
             })(controller.HttpContext.Request)(o => controller.View("Create", o), () => controller.Unauthorized(),
                 (p, e) => controller.ModelState.AddModelError(p, e), ()=> controller.ModelState.IsValid,
                 (s, o) => controller.ViewData[s] = o, () => controller.RedirectToAction("Index"))(controller);

        public static Func<Task<IActionResult>> ComposeEdit(
            ConfigurableController controller,
            Func<Action<string, object>, IRepository<TEntity>, Action<TEntity>> prepareOptions,
            Func<string, ConvertFuncResult<TKey>> keyConverter,
            Func<TKey, Expression<Func<TEntity, bool>>> findPredicate,
            Include<TEntity> editIncludes
            ) =>
             () => ComposeAsync("Edit", request => (view, notFound, isValid, addViewData) => repository =>
             {
                 var (id, valid) = request.BindId(keyConverter);
                 var setViewDataMultiSelectLists = prepareOptions(addViewData, repository);

                 return MvcHandler.MakeActionResultOnEntityRequest(
                     view,
                     notFound,
                     isValid(),
                     () => repository.Find(findPredicate(id.Value), editIncludes),
                     entity => setViewDataMultiSelectLists(entity)
                 );
             })(controller.HttpContext.Request)(o => controller.View("Edit", o), () => controller.NotFound(), () => controller.ModelState.IsValid, (n, o) => controller.ViewData.Add(n, o))(controller);

        public static Func<Task<IActionResult>> ComposeEditConfirmed(
            ConfigurableController controller,
            Func<string, UserContext, bool> authorize,
            Func<Action<string, object>, HttpRequest, TEntity, IRepository<TEntity>, ValueTuple<Action<IBatch<TEntity>>, Action>> parseRequests,
            Func<TEntity> constructor,
            Dictionary<string, Func<TEntity, Func<StringValues, VerboseResult>>> editableBinders,
            Dictionary<string, Func<TEntity, Action<StringValues>>> notEditableBinders,
            Include<TEntity> disabledProperties) =>
             () => ComposeAsync("EditConfirmed", request => (view, unauthorized, addModelError, isValid, addViewData, redirect) => (repository, storage, state) =>
             {
                 if (!authorize("Edit", state.UserContext))
                     return unauthorized();

                 TEntity entity = ControllerExtensions.Bind(request, addModelError, constructor, editableBinders, notEditableBinders);
                 var (modifyRelated, setViewDataMultiSelectLists) = parseRequests(addViewData, request, entity, repository);

                 return MvcHandler.MakeActionResultOnEntitySave(
                        addViewData,
                        addModelError,
                        isValid(),
                        () => storage.Handle(
                            batch =>
                            {
                                batch.Modify(entity, disabledProperties);
                                modifyRelated(batch);
                            }),
                        () =>
                        {
                            setViewDataMultiSelectLists();
                            return view(entity);
                        },
                        redirect
                    );
             })(controller.HttpContext.Request)(o => controller.View("Create", o), () => controller.Unauthorized(),
                 (p, e) => controller.ModelState.AddModelError(p, e), () => controller.ModelState.IsValid,
                 (s, o) => controller.ViewData[s] = o, () => controller.RedirectToAction("Index"))(controller);

        public static Func<Task<IActionResult>> ComposeDelete(
            ConfigurableController controller,
            Include<TEntity> deleteIncludes,
            Func<string, ConvertFuncResult<TKey>> keyConverter,
            Func<TKey, Expression<Func<TEntity, bool>>> findPredicate) =>
            () => ComposeAsync("Delete", request => (view, notFound) => repository => {
                var (id, valid) = request.BindId(keyConverter);
                return MvcHandler.MakeActionResultOnEntityRequest(
                     view,
                     notFound,
                     valid,
                     () => repository.Find(findPredicate(id.Value), deleteIncludes)
                );
            })(controller.HttpContext.Request)(o => controller.View("Delete", o), () => controller.NotFound())(controller);

        public static Func<Task<IActionResult>> ComposeDeleteConfirmed(
            ConfigurableController controller,
            Func<string, UserContext, bool> authorize,
            Func<string, ConvertFuncResult<TKey>> keyConverter,
            Func<TKey, Expression<Func<TEntity, bool>>> findPredicate) =>
         () => ComposeAsync("DeleteConfirmed", request => (view, unauthorized, addModelError, addViewData, redirect) => (repository, storage, state) =>
         {
             if (!authorize("Delete", state.UserContext))
                 return unauthorized();

             var (id, valid) = request.BindId(keyConverter);
             var entity = repository.Find(findPredicate(id.Value));

             return MvcHandler.MakeActionResultOnEntitySave(
                    addViewData,
                    addModelError,
                    true,
                    () => storage.Handle(batch => batch.Remove(entity)),
                    () => view(entity),
                    redirect
                );
         })(controller.HttpContext.Request)(o => controller.View("Delete", o), () => controller.Unauthorized(),
             (p, e) => controller.ModelState.AddModelError(p, e),
             (s, o) => controller.ViewData[s] = o, () => controller.RedirectToAction("Index"))(controller);
        #endregion

        public async Task<IActionResult> Index()
        {
            var routine = new MvcRoutine(controller, null);
            Func<object, IActionResult> view = o => controller.View(nameof(Index), o);
            return await routine.HandleStorageAsync<IActionResult, TEntity>(
                repository => {
                    var entities = repository.List(meta.IndexIncludes);
                    return view(entities);
                });
        }

        public async Task<IActionResult> Details()
        {
            var routine = new MvcRoutine(controller);
            return await routine.HandleStorageAsync<IActionResult, TEntity>(repository =>
            {
                var (id, valid) = controller.HttpContext.Request.BindId(meta.KeyConverter);
                return MvcHandler.MakeActionResultOnEntityRequest(
                    o => controller.View(nameof(Details), o),
                    controller.NotFound,
                    valid,
                    () => repository.Find(meta.FindPredicate(id.Value), meta.DetailsIncludes)
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
            return await routine.HandleStorageAsync<IActionResult, TEntity>((repository, storage, state) =>
            {
                if (! authorize(nameof(Create), state.UserContext))
                    return controller.Unauthorized();

                TEntity entity = ControllerExtensions.Bind(controller.HttpContext.Request, (key, value) =>controller.ModelState.AddModelError(key, value), meta.Constructor, meta.FormFields, meta.HiddenFormFields);

                var (modifyRelated, setViewDataMultiSelectLists) = meta.ReferencesCollection.ParseRelated((s, o) => controller.ViewData[s] = o, repository, controller.HttpContext.Request, entity);

                return MvcHandler.MakeActionResultOnEntitySave(
                    controller.ModelState.IsValid,
                    () => storage.Handle(
                        batch =>
                        {
                            batch.Add(entity);
                            modifyRelated(batch);
                        }),
                    () =>
                    {
                        setViewDataMultiSelectLists();
                        return controller.View(nameof(Create), entity);
                    },
                    () => controller.RedirectToAction(nameof(Index)),
                    ex => controller.ViewData["Exception"] = ex,
                    (key, value) => controller.ModelState.AddModelError(key, value)
                );
            });
        }

        public async Task<IActionResult> Edit()
        {
            var routine = new MvcRoutine(controller);
            return await routine.HandleStorageAsync<IActionResult, TEntity>(repository =>
            {
                var (id, valid) = controller.HttpContext.Request.BindId(meta.KeyConverter);

                var setViewDataMultiSelectLists = meta.ReferencesCollection.PrepareOptions((s, o) => controller.ViewData[s] = o, repository);

                return MvcHandler.MakeActionResultOnEntityRequest(
                    o => controller.View(nameof(Details), o),
                    controller.NotFound,
                    valid,
                    () => repository.Find(meta.FindPredicate(id.Value), meta.EditIncludes),
                    entity =>
                        setViewDataMultiSelectLists(entity)
                );
            });
        }

        public async Task<IActionResult> EditConfirmed()
        {
            var routine = new MvcRoutine(controller);
            return await routine.HandleStorageAsync<IActionResult, TEntity>(
                (repository, storage, state) =>
                {
                    if (!authorize(nameof(Edit), state.UserContext))
                        return controller.Unauthorized();

                    TEntity entity = ControllerExtensions.Bind(controller.HttpContext.Request, (p, e) => controller.ModelState.AddModelError(p, e), meta.Constructor, 
                        meta.FormFields, meta.HiddenFormFields);

                    var (modifyRelated, setViewDataMultiSelectLists) = meta.ReferencesCollection.ParseRelated((s, o) => controller.ViewData[s] = o, repository, controller.HttpContext.Request, entity);
                    
                    return MvcHandler.MakeActionResultOnEntitySave(
                        controller.ModelState.IsValid,
                        () => storage.Handle(
                            batch =>
                            {
                                batch.Modify(entity, meta.DisabledFormFields);
                                modifyRelated(batch);
                            }),
                        () =>
                        {
                            setViewDataMultiSelectLists();
                            return controller.View(nameof(Edit), entity);
                        },
                        () => controller.RedirectToAction(nameof(Index)),
                        ex => controller.ViewBag.Exception = ex,
                        (key, value) => controller.ModelState.AddModelError(key, value)
                    );
                });
        }

        public async Task<IActionResult> Delete()
        {
            var routine = new MvcRoutine(controller);
            return await routine.HandleStorageAsync<IActionResult, TEntity>(repository =>
            {
                var (id, valid) = controller.HttpContext.Request.BindId(meta.KeyConverter);
                return MvcHandler.MakeActionResultOnEntityRequest(
                        o => controller.View(nameof(Details), o),
                        controller.NotFound,
                        valid,
                        () => repository.Find(meta.FindPredicate(id.Value), meta.DeleteIncludes)
                    );
            });
        }

        public async Task<IActionResult> DeleteConfirmed()
        {
            var routine = new MvcRoutine(controller);
            return await routine.HandleStorageAsync<IActionResult, TEntity>((repository, storage, state) =>
            {
                if (!authorize(nameof(Delete), state.UserContext))
                    return controller.Unauthorized();
                var (id, valid) = controller.HttpContext.Request.BindId(meta.KeyConverter);
                var entity = repository.Find(meta.FindPredicate(id.Value));

                return MvcHandler.MakeActionResultOnEntitySave(
                        true,
                        () => storage.Handle(batch => batch.Remove(entity)),
                        () => controller.View(nameof(Delete), entity),
                        () => controller.RedirectToAction(nameof(Index)),
                        ex => controller.ViewBag.Exception = ex,
                        (key, value) => controller.ModelState.AddModelError(key, value)
                    );
            });
        }
    }
}