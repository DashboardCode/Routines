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

        public void Handle(Action<TResource, RoutineClosure<TUserContext>> action) =>
            routineHandler.Handle(ComposeResourceHandled(action));

        public TOutput Handle<TOutput>(Func<TResource, RoutineClosure<TUserContext>, TOutput> func) =>
            routineHandler.Handle(ComposeResourceFuncHandled(func));

        public Task<TOutput> HandleAsync<TOutput>(Func<TResource, RoutineClosure<TUserContext>, Task<TOutput>> func) =>
            routineHandler.HandleAsync(ComposeResourceFuncHandled(func));
        #endregion
    }
}
