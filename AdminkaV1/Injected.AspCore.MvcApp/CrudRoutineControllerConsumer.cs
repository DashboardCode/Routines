using System;
using System.Threading.Tasks; 
using Microsoft.AspNetCore.Mvc; 

using DashboardCode.Routines.Storage;
using DashboardCode.Routines.AspNetCore;
using DashboardCode.Routines;
using Microsoft.AspNetCore.Http;

namespace DashboardCode.AdminkaV1.Injected.AspCore.MvcApp
{
    class CrudRoutineControllerConsumer<TEntity, TKey> where TEntity: class, new()
    {
        readonly ControllerMeta<TEntity, TKey> meta;
        readonly RoutineController controller;
        readonly Func<string, UserContext, bool> authorize;
        public CrudRoutineControllerConsumer(
            RoutineController controller,
            ControllerMeta<TEntity, TKey> meta,
            Func<string, UserContext, bool> authorize
            ) 
        {
            this.meta = meta;
            this.controller = controller;
            this.authorize = authorize;
        }

        #region Compose - Configurability
        public static Func<RoutineController, Task<IActionResult>> Compose(
            string action, Func<MvcRoutine, Controller, Task<IActionResult>> func)
        {
            return controller => func(new MvcRoutine(controller, action), controller);
        }

        public static Func<RoutineController, Task<IActionResult>> Compose(
            string action,
            Func<MvcRoutine, Func<Func<string, object, IActionResult>, Task<IActionResult>>> func)
        {
            return Compose(action, (routine, controller) => func(routine)((view, model) => controller.View(view, model)));
        }

        public static Func<RoutineController, Task<IActionResult>> ComposeAsync(
            string action,
            Func<IRepository<TEntity>, Func<Func<string, object, IActionResult>, IActionResult>> func)
        {
            return Compose(action,
                async (routine, controller) =>
                     await routine.HandleStorageAsync<IActionResult, TEntity>(
                        repository => func(repository)((view, model) => controller.View(view, model))
                    )
            );
        }


        public static Func<RoutineController, Task<IActionResult>> ComposeAsync(
            string action,
            Func<HttpRequest, Func<IRepository<TEntity>, Func<Func<string, object, IActionResult>, IActionResult>>> func)
        {
            return Compose(action,
                async (routine, controller) =>
                     await routine.HandleStorageAsync<IActionResult, TEntity>(
                        repository => func(controller.Request)(repository)((view, model) => controller.View(view, model))
                    )
            );
        }

        public static Func<Task<IActionResult>> ComposeIndex(RoutineController controller, Include<TEntity> indexIncludes)
        {
            return () => ComposeAsync("Index", request => repository => view => {
                var entities = repository.List(indexIncludes);
                return view("Index", entities);
            })(controller);
        }
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
                var path = controller.HttpContext.Request.Path.Value;

                return controller.MakeActionResultOnEntityRequest(
                    nameof(Details),
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
                meta.ReferencesMeta.SetViewDataMultiSelectLists(controller, repository);
                return controller.View(nameof(Create));
            });
        }

        public async Task<IActionResult> CreateFormData()
        {
            var routine = new MvcRoutine(controller);
            return await routine.HandleStorageAsync<IActionResult, TEntity>((repository, storage, state) =>
            {
                if (! authorize(nameof(Create), state.UserContext))
                    return controller.Unauthorized();

                TEntity entity = controller.Bind(meta.Constructor, meta.editableBinders);
                meta.ReferencesMeta.ParseRequests(controller, entity, repository,
                    out Action<IBatch<TEntity>> modifyRelated, out Action setViewDataMultiSelectLists);

                return controller.MakeActionResultOnEntitySave(
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
                    () => controller.RedirectToAction(nameof(Index))
                );
            });
        }

        public async Task<IActionResult> Edit()
        {
            var routine = new MvcRoutine(controller);
            return await routine.HandleStorageAsync<IActionResult, TEntity>(repository =>
            {
                var (id, valid) = controller.HttpContext.Request.BindId(meta.KeyConverter);

                meta.ReferencesMeta.PrepareOptions(controller, repository, out Action<TEntity> setViewDataMultiSelectLists);
                return controller.MakeActionResultOnEntityRequest(
                    nameof(Edit),
                    valid,
                    () => repository.Find(meta.FindPredicate(id.Value), meta.EditIncludes),
                    entity =>
                        setViewDataMultiSelectLists(entity)
                );
            });
        }

        public async Task<IActionResult> EditFormData()
        {
            var routine = new MvcRoutine(controller);
            return await routine.HandleStorageAsync<IActionResult, TEntity>(
                (repository, storage, state) =>
                {
                    if (!authorize(nameof(Edit), state.UserContext))
                        return controller.Unauthorized();

                    var entity = controller.Bind(meta.Constructor, meta.editableBinders, meta.notEditableBinders);

                    meta.ReferencesMeta.ParseRequests(controller, entity, repository,
                       out var modifyRelated, out var setViewDataMultiSelectLists);

                    return controller.MakeActionResultOnEntitySave(
                        controller.ModelState.IsValid,
                        () => storage.Handle(
                            batch =>
                            {
                                batch.Modify(entity, meta.DisabledProperties);
                                modifyRelated(batch);
                            }),
                        () =>
                        {
                            setViewDataMultiSelectLists();
                            return controller.View(nameof(Edit), entity);
                        },
                        () => controller.RedirectToAction(nameof(Index))
                    );
                });
        }

        public async Task<IActionResult> Delete()
        {
            var routine = new MvcRoutine(controller);
            return await routine.HandleStorageAsync<IActionResult, TEntity>(repository =>
            {
                var (id, valid) = controller.HttpContext.Request.BindId(meta.KeyConverter);
                return controller.MakeActionResultOnEntityRequest(
                        nameof(Delete),
                        valid,
                        () => repository.Find(meta.FindPredicate(id.Value), meta.DeleteIncludes)
                    );
            });
        }

        public async Task<IActionResult> DeleteFormData()
        {
            var routine = new MvcRoutine(controller);
            return await routine.HandleStorageAsync<IActionResult, TEntity>((repository, storage, state) =>
            {
                if (!authorize(nameof(Delete), state.UserContext))
                    return controller.Unauthorized();
                var (id, valid) = controller.HttpContext.Request.BindId(meta.KeyConverter);
                var entity = repository.Find(meta.FindPredicate(id.Value));

                return controller.MakeActionResultOnEntitySave(
                        true,
                        () => storage.Handle(batch => batch.Remove(entity)),
                        () => controller.View(nameof(Delete), entity),
                        () => controller.RedirectToAction(nameof(Index))
                    );
            });
        }
    }
}