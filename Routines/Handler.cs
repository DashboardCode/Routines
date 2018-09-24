using System;
using System.Threading.Tasks;

namespace DashboardCode.Routines
{
    public class DisposeHandler<TClosure, TSuperClosure> : IHandler<TClosure, TSuperClosure> where TClosure : IDisposable
    {
        readonly TSuperClosure closure;
        readonly Func<TClosure> createResource;

        public DisposeHandler(
                Func<TClosure> createResource,
                TSuperClosure closure
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

        public void Handle(Action<TClosure, TSuperClosure> action)
        {
            using (var resource = createResource())
                action(resource, closure);
        }

        public TOutput Handle<TOutput>(Func<TClosure, TSuperClosure, TOutput> func)
        {
            using (var resource = createResource())
                return func(resource, closure);
        }

        public async Task<TOutput> HandleAsync<TOutput>(Func<TClosure, TSuperClosure, Task<TOutput>> func)
        {
            using (var resource = createResource())
                return await func(resource, closure);
        }

        public async Task HandleAsync(Func<TClosure, Task> func)
        {
            using (var dbContext = createResource())
                await func(dbContext);
        }

        public async Task HandleAsync(Func<TClosure, TSuperClosure, Task> func)
        {
            using (var resource = createResource())
                await func(resource, closure);
        }
    }

    public class DisposeHandler<TClosure, TSuperClosure, TDerivedClosure> : IHandler<TClosure, TSuperClosure> where TDerivedClosure : IDisposable, TClosure
    {
        readonly TSuperClosure closure;
        readonly Func<TDerivedClosure> createResource;

        public DisposeHandler(
                Func<TDerivedClosure> createResource,
                TSuperClosure closure
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

        public void Handle(Action<TClosure, TSuperClosure> action)
        {
            using (var resource = createResource())
                action(resource, closure);
        }

        public TOutput Handle<TOutput>(Func<TClosure, TSuperClosure, TOutput> func)
        {
            using (var resource = createResource())
                return func(resource, closure);
        }

        public async Task<TOutput> HandleAsync<TOutput>(Func<TClosure, TSuperClosure, Task<TOutput>> func)
        {
            using (var resource = createResource())
                return await func(resource, closure);
        }

        public async Task HandleAsync(Func<TClosure, Task> func)
        {
            using (var dbContext = createResource())
                await func(dbContext);
        }

        public async Task HandleAsync(Func<TClosure, TSuperClosure, Task> func)
        {
            using (var resource = createResource())
                await func(resource, closure);
        }

    }

    public class Handler<TClosure, TSuperClosure> : IHandler<TClosure, TSuperClosure>
    {
        readonly TSuperClosure closure;
        readonly Func<TClosure> createResource;

        public Handler(
                Func<TClosure> createResource,
                TSuperClosure closure
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

        public void Handle(Action<TClosure, TSuperClosure> action)
        {
            var resource = createResource();
            action(resource, closure);
        }

        public TOutput Handle<TOutput>(Func<TClosure, TSuperClosure, TOutput> func)
        {
            var resource = createResource();
            return func(resource, closure);
        }

        public async Task<TOutput> HandleAsync<TOutput>(Func<TClosure, TSuperClosure, Task<TOutput>> func)
        {
            var resource = createResource();
            return await func(resource, closure);
        }

        public async Task HandleAsync(Func<TClosure, Task> func)
        {
            var dbContext = createResource();
            await func(dbContext);
        }

        public async Task HandleAsync(Func<TClosure, TSuperClosure, Task> func)
        {
            var resource = createResource();
            await func(resource, closure);
        }
    }
}
