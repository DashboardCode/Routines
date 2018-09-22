using System;
using System.Threading.Tasks;

namespace DashboardCode.Routines.Injected
{
    public class ComplexRoutineHandler<TUserContext, TClosure> : IRoutineHandler<TClosure, TUserContext> where TClosure : IDisposable
    {
        readonly Func<RoutineClosure<TUserContext>, TClosure> createResource;
        readonly IHandler<RoutineClosure<TUserContext>> handler;

        public ComplexRoutineHandler(
            Func<RoutineClosure<TUserContext>, TClosure> createResource,
            IHandler<RoutineClosure<TUserContext>> handler
            ) 
        {
            this.createResource = createResource;
            this.handler = handler;
        }

        public RoutineHandler<TUserContext, TClosure> CreateResource(RoutineClosure<TUserContext> closure)
        {
            var dbContextHandler = new RoutineHandler<TUserContext, TClosure>(()=>createResource(closure), closure);
            return dbContextHandler;
        }

        private Action<RoutineClosure<TUserContext>> ComposeResourceHandled(Action<TClosure, RoutineClosure<TUserContext>> action)
        {
            return closure =>
            {
                using (var resource = createResource(closure))
                    action(resource, closure);
            };
        }

        private Func<RoutineClosure<TUserContext>, TOutput> ComposeResourceFuncHandled<TOutput>(Func<TClosure, RoutineClosure<TUserContext>, TOutput> func)
        {
            return closure =>
            {
                using (var resource = createResource(closure))
                    return func(resource, closure);
            };
        }

        public Action<RoutineClosure<TUserContext>> ComposeResourceHandled(Action<TClosure> action)
        {
            return closure =>
            {
                using (var resource = createResource(closure))
                    action(resource);
            };
        }

        public Func<RoutineClosure<TUserContext>, TOutput> ComposeResourceHandled<TOutput>(Func<TClosure, TOutput> func)
        {
            return closure =>
            {
                using (var resource = createResource(closure))
                    return func(resource);
            };
        }

        #region IRoutineHandler
        public void Handle(Action<TClosure> action) =>
            handler.Handle(closure =>
            {
                using (var resource = createResource(closure))
                    action(resource);
            });

        public TOutput Handle<TOutput>(Func<TClosure, TOutput> func) =>
            handler.Handle(closure =>
            {
                using (var resource = createResource(closure))
                    return func(resource);
            });


        public void Handle(Action<TClosure, RoutineClosure<TUserContext>> action) =>
            handler.Handle(closure =>
            {
                using (var resource = createResource(closure))
                    action(resource, closure);
            });

        public TOutput Handle<TOutput>(Func<TClosure, RoutineClosure<TUserContext>, TOutput> func) =>
            handler.Handle(closure =>
            {
                using (var resource = createResource(closure))
                    return func(resource, closure);
            });

        public Task<TOutput> HandleAsync<TOutput>(Func<TClosure, Task<TOutput>> func) =>
            handler.HandleAsync(async closure =>
            {
                using (var resource = createResource(closure))
                    return await func(resource);
            });

        public Task HandleAsync(Func<TClosure, Task> func) =>
            handler.HandleAsync(async closure =>
            {
                using (var resource = createResource(closure))
                    await func(resource);
            });

        public Task<TOutput> HandleAsync<TOutput>(Func<TClosure, RoutineClosure<TUserContext>, Task<TOutput>> func) =>
            handler.HandleAsync(async closure =>
            {
                using (var resource = createResource(closure))
                    return await func(resource, closure);
            });

        public Task HandleAsync(Func<TClosure, RoutineClosure<TUserContext>, Task> func) =>
            handler.HandleAsync(async closure => 
            {
                using (var resource = createResource(closure))
                    await func(resource, closure);
            });
        #endregion
    }

    public class ComplexRoutineHandler<TUserContext, TIResource, TResource> : IRoutineHandler<TIResource, TUserContext>
        where TResource : IDisposable, TIResource
    {
        readonly Func<RoutineClosure<TUserContext>, TResource> createResource;
        readonly IHandler<RoutineClosure<TUserContext>> routineHandler;

        public ComplexRoutineHandler(
            Func<RoutineClosure<TUserContext>, TResource> createResource,
            IHandler<RoutineClosure<TUserContext>> routineHandler
            )
        {
            this.createResource = createResource;
            this.routineHandler = routineHandler;
        }

        public RoutineHandler<TUserContext, TResource> CreateResource(RoutineClosure<TUserContext> closure)
        {
            var dbContextHandler = new RoutineHandler<TUserContext, TResource>(() => createResource(closure), closure);
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
            routineHandler.Handle(closure =>
            {
                using (var resource = createResource(closure))
                    action(resource);
            });

        public TOutput Handle<TOutput>(Func<TIResource, TOutput> func) =>
            routineHandler.Handle(closure =>
            {
                using (var resource = createResource(closure))
                    return func(resource);
            });

        public void Handle(Action<TIResource, RoutineClosure<TUserContext>> action) =>
            routineHandler.Handle(closure =>
            {
                using (var resource = createResource(closure))
                    action(resource, closure);
            });

        public TOutput Handle<TOutput>(Func<TIResource, RoutineClosure<TUserContext>, TOutput> func) =>
            routineHandler.Handle(closure =>
            {
                using (var resource = createResource(closure))
                    return func(resource, closure);
            });

        public Task<TOutput> HandleAsync<TOutput>(Func<TIResource, Task<TOutput>> func) =>
            routineHandler.HandleAsync(async closure =>
            {
                using (var resource = createResource(closure))
                    return await func(resource);
            });

        public Task HandleAsync(Func<TIResource, Task> func) =>
            routineHandler.HandleAsync(async closure =>
            {
                using (var resource = createResource(closure))
                    await func(resource);
            });

        public Task<TOutput> HandleAsync<TOutput>(Func<TIResource, RoutineClosure<TUserContext>, Task<TOutput>> func) =>
            routineHandler.HandleAsync(async closure =>
            {
                using (var resource = createResource(closure))
                    return await func(resource, closure);
            });

        public Task HandleAsync(Func<TIResource, RoutineClosure<TUserContext>, Task> func) =>
            routineHandler.HandleAsync(async closure =>
            {
                using (var resource = createResource(closure))
                    await func(resource, closure);
            });
        #endregion
    }
}
