using System;
using System.Threading.Tasks;

namespace DashboardCode.Routines.Injected
{
    public class ResourceRoutineHandler<TUserContext, TResource> : IResourceHandler<TUserContext, TResource>
        where TResource : IDisposable
    {
        readonly Func<RoutineClosure<TUserContext>, TResource> createResource;
        readonly IRoutineHandler<RoutineClosure<TUserContext>> routineHandler;

        public ResourceRoutineHandler(
            Func<RoutineClosure<TUserContext>, TResource> createResource,
            IRoutineHandler<RoutineClosure<TUserContext>> routineHandler
            ) 
        {
            this.createResource = createResource;
            this.routineHandler = routineHandler;
        }

        public ResourceHandler<TUserContext, TResource> CreateResource(RoutineClosure<TUserContext> closure)
        {
            var dbContextHandler = new ResourceHandler<TUserContext, TResource>(closure, ()=>createResource(closure));
            return dbContextHandler;
        }

        private Action<RoutineClosure<TUserContext>> ComposeResourceHandled(Action<TResource, RoutineClosure<TUserContext>> action)
        {
            return closure =>
            {
                using (var resource = createResource(closure))
                    action(resource, closure);
            };
        }

        private Func<RoutineClosure<TUserContext>, TOutput> ComposeResourceFuncHandled<TOutput>(Func<TResource, RoutineClosure<TUserContext>, TOutput> func)
        {
            return closure =>
            {
                using (var resource = createResource(closure))
                    return func(resource, closure);
            };
        }

        public Action<RoutineClosure<TUserContext>> ComposeResourceHandled(Action<TResource> action)
        {
            return closure =>
            {
                using (var resource = createResource(closure))
                    action(resource);
            };
        }

        public Func<RoutineClosure<TUserContext>, TOutput> ComposeResourceHandled<TOutput>(Func<TResource, TOutput> func)
        {
            return closure =>
            {
                using (var resource = createResource(closure))
                    return func(resource);
            };
        }

        #region Handle with AdminkaDbContext
        public void Handle(Action<TResource> action) =>
            routineHandler.Handle(ComposeResourceHandled(action));

        public TOutput Handle<TOutput>(Func<TResource, TOutput> func) =>
            routineHandler.Handle(ComposeResourceHandled(func));

        public Task<TOutput> HandleAsync<TOutput>(Func<TResource, Task<TOutput>> func) =>
            routineHandler.HandleAsync(ComposeResourceHandled(func));

        public Task HandleAsync(Func<TResource, Task> func) =>
            routineHandler.HandleAsync(ComposeResourceHandled(func));

        public void Handle(Action<TResource, RoutineClosure<TUserContext>> action) =>
            routineHandler.Handle(ComposeResourceHandled(action));

        public TOutput Handle<TOutput>(Func<TResource, RoutineClosure<TUserContext>, TOutput> func) =>
            routineHandler.Handle(ComposeResourceFuncHandled(func));

        public Task<TOutput> HandleAsync<TOutput>(Func<TResource, RoutineClosure<TUserContext>, Task<TOutput>> func) =>
            routineHandler.HandleAsync(ComposeResourceFuncHandled(func));

        public Task HandleAsync(Func<TResource, RoutineClosure<TUserContext>, Task> func) =>
            routineHandler.HandleAsync(ComposeResourceFuncHandled(func));
        #endregion
    }


    public class ResourceRoutineHandler<TUserContext, TIResource, TResource> : IResourceHandler<TUserContext, TIResource>
        where TResource : IDisposable, TIResource
    {
        readonly Func<RoutineClosure<TUserContext>, TResource> createResource;
        readonly IRoutineHandler<RoutineClosure<TUserContext>> routineHandler;

        public ResourceRoutineHandler(
            Func<RoutineClosure<TUserContext>, TResource> createResource,
            IRoutineHandler<RoutineClosure<TUserContext>> routineHandler
            )
        {
            this.createResource = createResource;
            this.routineHandler = routineHandler;
        }

        public ResourceHandler<TUserContext, TResource> CreateResource(RoutineClosure<TUserContext> closure)
        {
            var dbContextHandler = new ResourceHandler<TUserContext, TResource>(closure, () => createResource(closure));
            return dbContextHandler;
        }

        private Action<RoutineClosure<TUserContext>> ComposeResourceHandled(Action<TIResource, RoutineClosure<TUserContext>> action)
        {
            return closure =>
            {
                using (var resource = createResource(closure))
                    action(resource, closure);
            };
        }

        private Func<RoutineClosure<TUserContext>, TOutput> ComposeResourceFuncHandled<TOutput>(Func<TIResource, RoutineClosure<TUserContext>, TOutput> func)
        {
            return closure =>
            {
                using (var resource = createResource(closure))
                    return func(resource, closure);
            };
        }

        public Action<RoutineClosure<TUserContext>> ComposeResourceHandled(Action<TIResource> action)
        {
            return closure =>
            {
                using (var resource = createResource(closure))
                    action(resource);
            };
        }

        public Func<RoutineClosure<TUserContext>, TOutput> ComposeResourceHandled<TOutput>(Func<TIResource, TOutput> func)
        {
            return closure =>
            {
                using (var resource = createResource(closure))
                    return func(resource);
            };
        }

        #region Handle with AdminkaDbContext
        public void Handle(Action<TIResource> action) =>
            routineHandler.Handle(ComposeResourceHandled(action));

        public TOutput Handle<TOutput>(Func<TIResource, TOutput> func) =>
            routineHandler.Handle(ComposeResourceHandled(func));

        public Task<TOutput> HandleAsync<TOutput>(Func<TIResource, Task<TOutput>> func) =>
            routineHandler.HandleAsync(ComposeResourceHandled(func));

        public Task HandleAsync(Func<TIResource, Task> func) =>
            routineHandler.HandleAsync(ComposeResourceHandled(func));

        public void Handle(Action<TIResource, RoutineClosure<TUserContext>> action) =>
            routineHandler.Handle(ComposeResourceHandled(action));

        public TOutput Handle<TOutput>(Func<TIResource, RoutineClosure<TUserContext>, TOutput> func) =>
            routineHandler.Handle(ComposeResourceFuncHandled(func));

        public Task<TOutput> HandleAsync<TOutput>(Func<TIResource, RoutineClosure<TUserContext>, Task<TOutput>> func) =>
            routineHandler.HandleAsync(ComposeResourceFuncHandled(func));

        public Task HandleAsync(Func<TIResource, RoutineClosure<TUserContext>, Task> func) =>
            routineHandler.HandleAsync(ComposeResourceFuncHandled(func));
        #endregion
    }
}
