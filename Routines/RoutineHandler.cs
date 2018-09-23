using System;
using System.Threading.Tasks;

namespace DashboardCode.Routines
{
    public class RoutineDisposeHandler<TClosure, TUserContext> : IRoutineHandler<TClosure, TUserContext> where TClosure : IDisposable
    {
        readonly RoutineClosure<TUserContext> closure;
        readonly Func<TClosure> createResource;

        public RoutineDisposeHandler(
                Func<TClosure> createResource,
                RoutineClosure<TUserContext> closure
            )
        {
            this.closure = closure;
            this.createResource = createResource;
        }

        public void Handle(Action<TClosure> action)
        {
            using (var resource = createResource())
                action(resource);
        }

        public TOutput Handle<TOutput>(Func<TClosure, TOutput> func)
        {
            using (var resource = createResource())
                return func(resource);
        }

        public async Task<TOutput> HandleAsync<TOutput>(Func<TClosure, Task<TOutput>> func)
        {
            using (var dbContext = createResource())
                return await func(dbContext);
        }

        public void Handle(Action<TClosure, RoutineClosure<TUserContext>> action)
        {
            using (var resource = createResource())
                action(resource, closure);
        }

        public TOutput Handle<TOutput>(Func<TClosure, RoutineClosure<TUserContext>, TOutput> func)
        {
            using (var resource = createResource())
                return func(resource, closure);
        }

        public async Task<TOutput> HandleAsync<TOutput>(Func<TClosure, RoutineClosure<TUserContext>, Task<TOutput>> func)
        {
            using (var resource = createResource())
                return await func(resource, closure);
        }

        public async Task HandleAsync(Func<TClosure, Task> func)
        {
            using (var dbContext = createResource())
                 await func(dbContext);
        }

        public async Task HandleAsync(Func<TClosure, RoutineClosure<TUserContext>, Task> func)
        {
            using (var resource = createResource())
                 await func(resource, closure);
        }
    }

    public class RoutineDisposeHandler<TClosure, TUserContext, TDerivedClosure> : IRoutineHandler<TClosure, TUserContext> where TDerivedClosure : IDisposable, TClosure
    {
        readonly RoutineClosure<TUserContext> closure;
        readonly Func<TDerivedClosure> createResource;

        public RoutineDisposeHandler(
                Func<TDerivedClosure> createResource,
                RoutineClosure<TUserContext> closure
            )
        {
            this.closure = closure;
            this.createResource = createResource;
        }

        public void Handle(Action<TClosure> action)
        {
            using (var resource = createResource())
                action(resource);
        }

        public TOutput Handle<TOutput>(Func<TClosure, TOutput> func)
        {
            using (var resource = createResource())
                return func(resource);
        }

        public async Task<TOutput> HandleAsync<TOutput>(Func<TClosure, Task<TOutput>> func)
        {
            using (var dbContext = createResource())
                return await func(dbContext);
        }

        public void Handle(Action<TClosure, RoutineClosure<TUserContext>> action)
        {
            using (var resource = createResource())
                action(resource, closure);
        }

        public TOutput Handle<TOutput>(Func<TClosure, RoutineClosure<TUserContext>, TOutput> func)
        {
            using (var resource = createResource())
                return func(resource, closure);
        }

        public async Task<TOutput> HandleAsync<TOutput>(Func<TClosure, RoutineClosure<TUserContext>, Task<TOutput>> func)
        {
            using (var resource = createResource())
                return await func(resource, closure);
        }

        public async Task HandleAsync(Func<TClosure, Task> func)
        {
            using (var dbContext = createResource())
                await func(dbContext);
        }

        public async Task HandleAsync(Func<TClosure, RoutineClosure<TUserContext>, Task> func)
        {
            using (var resource = createResource())
                await func(resource, closure);
        }

    }

    public class RoutineHandler<TClosure, TUserContext> : IRoutineHandler<TClosure, TUserContext>
    {
        readonly RoutineClosure<TUserContext> closure;
        readonly Func<TClosure> createResource;

        public RoutineHandler(
                Func<TClosure> createResource,
                RoutineClosure<TUserContext> closure
            )
        {
            this.closure = closure;
            this.createResource = createResource;
        }

        public void Handle(Action<TClosure> action)
        {
            var resource = createResource();
            action(resource);
        }

        public TOutput Handle<TOutput>(Func<TClosure, TOutput> func)
        {
            var resource = createResource();
            return func(resource);
        }

        public async Task<TOutput> HandleAsync<TOutput>(Func<TClosure, Task<TOutput>> func)
        {
            var dbContext = createResource();
            return await func(dbContext);
        }

        public void Handle(Action<TClosure, RoutineClosure<TUserContext>> action)
        {
            var resource = createResource();
            action(resource, closure);
        }

        public TOutput Handle<TOutput>(Func<TClosure, RoutineClosure<TUserContext>, TOutput> func)
        {
            var resource = createResource();
            return func(resource, closure);
        }

        public async Task<TOutput> HandleAsync<TOutput>(Func<TClosure, RoutineClosure<TUserContext>, Task<TOutput>> func)
        {
            var resource = createResource();
            return await func(resource, closure);
        }

        public async Task HandleAsync(Func<TClosure, Task> func)
        {
            var dbContext = createResource();
            await func(dbContext);
        }

        public async Task HandleAsync(Func<TClosure, RoutineClosure<TUserContext>, Task> func)
        {
            var resource = createResource();
            await func(resource, closure);
        }
    }

    public class RoutineHandler<TClosure, TUserContext, TDerivedClosure> : IRoutineHandler<TClosure, TUserContext> where TDerivedClosure : TClosure
    {
        readonly RoutineClosure<TUserContext> closure;
        readonly Func<TDerivedClosure> createResource;

        public RoutineHandler(
                Func<TDerivedClosure> createResource,
                RoutineClosure<TUserContext> closure
            )
        {
            this.closure = closure;
            this.createResource = createResource;
        }

        public void Handle(Action<TClosure> action)
        {
            var resource = createResource();
            action(resource);
        }

        public TOutput Handle<TOutput>(Func<TClosure, TOutput> func)
        {
            var resource = createResource();
            return func(resource);
        }

        public async Task<TOutput> HandleAsync<TOutput>(Func<TClosure, Task<TOutput>> func)
        {
            var dbContext = createResource();
            return await func(dbContext);
        }

        public void Handle(Action<TClosure, RoutineClosure<TUserContext>> action)
        {
            var resource = createResource();
            action(resource, closure);
        }

        public TOutput Handle<TOutput>(Func<TClosure, RoutineClosure<TUserContext>, TOutput> func)
        {
            var resource = createResource();
            return func(resource, closure);
        }

        public async Task<TOutput> HandleAsync<TOutput>(Func<TClosure, RoutineClosure<TUserContext>, Task<TOutput>> func)
        {
            var resource = createResource();
            return await func(resource, closure);
        }

        public async Task HandleAsync(Func<TClosure, Task> func)
        {
            var dbContext = createResource();
            await func(dbContext);
        }

        public async Task HandleAsync(Func<TClosure, RoutineClosure<TUserContext>, Task> func)
        {
            var resource = createResource();
            await func(resource, closure);
        }

    }
}
