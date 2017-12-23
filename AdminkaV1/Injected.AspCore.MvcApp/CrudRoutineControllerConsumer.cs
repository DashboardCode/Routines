using System;
using System.Threading.Tasks; 
using Microsoft.AspNetCore.Mvc; 

using DashboardCode.Routines.Storage;
using DashboardCode.Routines.AspNetCore;
using Microsoft.Extensions.Primitives;

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

        public async Task<IActionResult> Index()
        {
            var routine = new MvcRoutine(controller, null);
            return await routine.HandleStorageAsync<IActionResult, TEntity>(
                (repository) => {
                    var groups = repository.List(meta.IndexIncludes);
                    return controller.View(groups);
                });
        }

        public async Task<IActionResult> Details()
        {
            var routine = new MvcRoutine(controller);
            return await routine.HandleStorageAsync<IActionResult, TEntity>(repository =>
            {
                var (id, valid) = controller.BindId(meta.KeyConverter);
                var path = controller.HttpContext.Request.Path.Value;

                return controller.MakeActionResultOnRequest(
                    () => valid,
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
                return controller.View();
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
                    out Action<IBatch<TEntity>> modifyRelateds, out Action setViewDataMultiSelectLists);

                return controller.MakeActionResultOnSave(
                    controller.ModelState.IsValid,
                    () => storage.Handle(
                        batch =>
                        {
                            batch.Add(entity);
                            modifyRelateds(batch);
                        }),
                    () =>
                    {
                        setViewDataMultiSelectLists();
                        return controller.View(entity);
                    }
                );
            });
        }

        public async Task<IActionResult> Edit()
        {
            var routine = new MvcRoutine(controller);
            return await routine.HandleStorageAsync<IActionResult, TEntity>(repository =>
            {
                var (id, valid) = controller.BindId(meta.KeyConverter);

                meta.ReferencesMeta.PrepareOptions(controller, repository, out Action<TEntity> setViewDataMultiSelectLists);
                return controller.MakeActionResultOnRequest(
                    () => valid,
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

                    TEntity entity = controller.Bind(meta.Constructor, meta.editableBinders, meta.notEditableBinders);

                    meta.ReferencesMeta.ParseRequests(controller, entity, repository,
                       out Action<IBatch<TEntity>> modifyRelateds, out Action setViewDataMultiSelectLists);

                    return controller.MakeActionResultOnSave(
                        controller.ModelState.IsValid,
                        () => storage.Handle(
                            batch =>
                            {
                                batch.Modify(entity, meta.DisabledProperties);
                                modifyRelateds(batch);
                            }),
                        () =>
                        {
                            setViewDataMultiSelectLists();
                            return controller.View(entity);
                        }
                    );
                });
        }

        public async Task<IActionResult> Delete()
        {
            var routine = new MvcRoutine(controller);
            return await routine.HandleStorageAsync<IActionResult, TEntity>(repository =>
            {
                var (id, valid) = controller.BindId(meta.KeyConverter);
                return controller.MakeActionResultOnRequest(
                        () => valid,
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
                var (id, valid) = controller.BindId(meta.KeyConverter);
                var entity = repository.Find(meta.FindPredicate(id.Value));

                return controller.MakeActionResultOnSave(
                        true,
                        () => storage.Handle(batch => batch.Remove(entity)),
                        () => controller.View(nameof(Delete), entity)
                    );
            });
        }
    }
}