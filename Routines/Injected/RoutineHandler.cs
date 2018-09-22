using System;
using System.Threading.Tasks;

namespace DashboardCode.Routines.Injected
{
    public class RoutineHandler<TUserContext, TResource> : IRoutineHandler<TResource, TUserContext> where TResource : IDisposable
    {
        readonly RoutineClosure<TUserContext> closure;
        readonly Func<TResource> createResource;

        public RoutineHandler(
                Func<TResource> createResource,
                RoutineClosure<TUserContext> closure
            )
        {
            this.closure = closure;
            this.createResource = createResource;
        }

        public void Handle(Action<TResource> action)
        {
            using (var resource = createResource())
                action(resource);
        }

        public TOutput Handle<TOutput>(Func<TResource, TOutput> func)
        {
            using (var resource = createResource())
                return func(resource);
        }

        public async Task<TOutput> HandleAsync<TOutput>(Func<TResource, Task<TOutput>> func)
        {
            using (var dbContext = createResource())
                return await func(dbContext);
        }

        public void Handle(Action<TResource, RoutineClosure<TUserContext>> action)
        {
            using (var resource = createResource())
                action(resource, closure);
        }

        public TOutput Handle<TOutput>(Func<TResource, RoutineClosure<TUserContext>, TOutput> func)
        {
            using (var resource = createResource())
                return func(resource, closure);
        }

        public async Task<TOutput> HandleAsync<TOutput>(Func<TResource, RoutineClosure<TUserContext>, Task<TOutput>> func)
        {
            using (var resource = createResource())
                return await func(resource, closure);
        }

        public async Task HandleAsync(Func<TResource, Task> func)
        {
            using (var dbContext = createResource())
                 await func(dbContext);
        }

        public async Task HandleAsync(Func<TResource, RoutineClosure<TUserContext>, Task> func)
        {
            using (var resource = createResource())
                 await func(resource, closure);
        }
    }

    public class RoutineHandler<TIResource, TUserContext, TResource> : IRoutineHandler<TIResource, TUserContext> where TResource : IDisposable, TIResource
    {
        readonly RoutineClosure<TUserContext> closure;
        readonly Func<TResource> createResource;

        public RoutineHandler(
                Func<TResource> createResource,
                RoutineClosure<TUserContext> closure
            )
        {
            this.closure = closure;
            this.createResource = createResource;
        }

        public void Handle(Action<TIResource> action)
        {
            using (var resource = createResource())
                action(resource);
        }

        public TOutput Handle<TOutput>(Func<TIResource, TOutput> func)
        {
            using (var resource = createResource())
                return func(resource);
        }

        public async Task<TOutput> HandleAsync<TOutput>(Func<TIResource, Task<TOutput>> func)
        {
            using (var dbContext = createResource())
                return await func(dbContext);
        }

        public void Handle(Action<TIResource, RoutineClosure<TUserContext>> action)
        {
            using (var resource = createResource())
                action(resource, closure);
        }

        public TOutput Handle<TOutput>(Func<TIResource, RoutineClosure<TUserContext>, TOutput> func)
        {
            using (var resource = createResource())
                return func(resource, closure);
        }

        public async Task<TOutput> HandleAsync<TOutput>(Func<TIResource, RoutineClosure<TUserContext>, Task<TOutput>> func)
        {
            using (var resource = createResource())
                return await func(resource, closure);
        }

        public async Task HandleAsync(Func<TIResource, Task> func)
        {
            using (var dbContext = createResource())
                await func(dbContext);
        }

        public async Task HandleAsync(Func<TIResource, RoutineClosure<TUserContext>, Task> func)
        {
            using (var resource = createResource())
                await func(resource, closure);
        }

    }
}
