using System;
using System.Threading.Tasks; 
using Microsoft.AspNetCore.Mvc; 

using DashboardCode.Routines.Storage;
using DashboardCode.Routines.AspNetCore;
          
namespace DashboardCode.AdminkaV1.Injected.AspCore.MvcApp
{
    class CrudRoutineControllerConsumer<TEntity, TKey> where TEntity: class
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

        public async Task<IActionResult> Details(TKey id)
        {
            var routine = new MvcRoutine(controller, new { id = id });
            return await routine.HandleStorageAsync<IActionResult, TEntity>(repository =>
            {
                return controller.MakeActionResultOnRequest(
                    () => id != null,
                    () => repository.Find(meta.FindPredicate(id), meta.DetailsIncludes)
                );
            });
        }

        public async Task<IActionResult> Create()
        {
            var routine = new MvcRoutine(controller, null);
            return await routine.HandleStorageAsync<IActionResult, TEntity>(repository =>
            {
                meta.ReferencesManager.SetViewDataMultiSelectLists(controller, repository);
                return controller.View();
            });
        }

        public async Task<IActionResult> CreateConfirmed()
        {
            var routine = new MvcRoutine(controller);
            return await routine.HandleStorageAsync<IActionResult, TEntity>((repository, storage, state) =>
            {
                if (! authorize(nameof(Create), state.UserContext))
                    return controller.Unauthorized();

                TEntity entity = controller.Bind(meta.Constructor, meta.editableBinders);
                meta.ReferencesManager.ParseRequests(controller, entity, repository,
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

        public async Task<IActionResult> Edit(TKey id)
        {
            var routine = new MvcRoutine(controller, new { id = id });
            return await routine.HandleStorageAsync<IActionResult, TEntity>(repository =>
            {
                meta.ReferencesManager.PrepareOptions(controller, repository, out Action<TEntity> setViewDataMultiSelectLists);
                return controller.MakeActionResultOnRequest(
                    () => id != null,
                    () => repository.Find(meta.FindPredicate(id), meta.EditIncludes),
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

                    TEntity entity = controller.Bind(meta.Constructor, meta.editableBinders, meta.notEditableBinders);

                    meta.ReferencesManager.ParseRequests(controller, entity, repository,
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

        public async Task<IActionResult> Delete(TKey id)
        {
            var routine = new MvcRoutine(controller, new { id = id });
            return await routine.HandleStorageAsync<IActionResult, TEntity>(repository =>
            {
                return controller.MakeActionResultOnRequest(
                        () => id != null,
                        () => repository.Find(meta.FindPredicate(id), meta.DeleteIncludes)
                    );
            });
        }

        public async Task<IActionResult> DeleteConfirmed(TKey id)
        {
            var routine = new MvcRoutine(controller, new { id = id });
            return await routine.HandleStorageAsync<IActionResult, TEntity>((repository, storage, state) =>
            {
                if (!authorize(nameof(Delete), state.UserContext))
                    return controller.Unauthorized();
                var entity = repository.Find(meta.FindPredicate(id));

                return controller.MakeActionResultOnSave(
                        true,
                        () => storage.Handle(batch => batch.Remove(entity)),
                        () => controller.View(nameof(Delete), entity)
                    );
            });
        }
    }
}