using System;
using System.Threading.Tasks;

namespace DashboardCode.Routines
{
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

    public class HandlerAsync<TClosure, TSuperClosure> : IHandler<TClosure, TSuperClosure>
    {
        readonly TSuperClosure closure;
        readonly Func<Task<TClosure>> createResource;

        public HandlerAsync(
                Func<Task<TClosure>> createResource,
                TSuperClosure closure
            )
        {
            this.closure = closure;
            this.createResource = createResource;
        }

        public void Handle(Action<TClosure> action)
        {
            var resource = createResource().Result;
            action(resource);
        }

        public TOutput Handle<TOutput>(Func<TClosure, TOutput> func)
        {
            var resource = createResource().Result;
            return func(resource);
        }

        public async Task<TOutput> HandleAsync<TOutput>(Func<TClosure, Task<TOutput>> func)
        {
            var dbContext = await createResource();
            return await func(dbContext);
        }

        public void Handle(Action<TClosure, TSuperClosure> action)
        {
            var resource = createResource().Result;
            action(resource, closure);
        }

        public TOutput Handle<TOutput>(Func<TClosure, TSuperClosure, TOutput> func)
        {
            var resource = createResource().Result;
            return func(resource, closure);
        }

        public async Task<TOutput> HandleAsync<TOutput>(Func<TClosure, TSuperClosure, Task<TOutput>> func)
        {
            var resource = await createResource();
            return await func(resource, closure);
        }

        public async Task HandleAsync(Func<TClosure, Task> func)
        {
            var dbContext = await createResource();
            await func(dbContext);
        }

        public async Task HandleAsync(Func<TClosure, TSuperClosure, Task> func)
        {
            var resource = await createResource();
            await func(resource, closure);
        }
    }

    public class DisposeHandlerAsync<TClosure, TSuperClosure> : IHandler<TClosure, TSuperClosure> where TClosure : IDisposable
    {
        readonly TSuperClosure closure;
        readonly Func<Task<TClosure>> createResource;

        public DisposeHandlerAsync(
                 Func<Task<TClosure>> createResource,
                TSuperClosure closure
            )
        {
            this.closure = closure;
            this.createResource = createResource;
        }

        public void Handle(Action<TClosure> action)
        {
            using (var resource = createResource().Result)
                action(resource);
        }

        public TOutput Handle<TOutput>(Func<TClosure, TOutput> func)
        {
            using (var resource = createResource().Result)
                return func(resource);
        }

        public async Task<TOutput> HandleAsync<TOutput>(Func<TClosure, Task<TOutput>> func)
        {
            using (var dbContext = await createResource())
                return await func(dbContext);
        }

        public void Handle(Action<TClosure, TSuperClosure> action)
        {
            using (var resource = createResource().Result)
                action(resource, closure);
        }

        public TOutput Handle<TOutput>(Func<TClosure, TSuperClosure, TOutput> func)
        {
            using (var resource = createResource().Result)
                return func(resource, closure);
        }

        public async Task<TOutput> HandleAsync<TOutput>(Func<TClosure, TSuperClosure, Task<TOutput>> func)
        {
            using (var resource = await createResource())
                return await func(resource, closure);
        }

        public async Task HandleAsync(Func<TClosure, Task> func)
        {
            using (var dbContext = await createResource())
                await func(dbContext);
        }

        public async Task HandleAsync(Func<TClosure, TSuperClosure, Task> func)
        {
            using (var resource = await createResource())
                await func(resource, closure);
        }
    }

    public class DisposeHandlerAsync<TClosure, TSuperClosure, TDerivedClosure> : IHandler<TClosure, TSuperClosure> where TDerivedClosure : IDisposable, TClosure
    {
        readonly TSuperClosure closure;
        readonly Func<Task<TDerivedClosure>> createResource;

        public DisposeHandlerAsync(
                Func<Task<TDerivedClosure>> createResource,
                TSuperClosure closure
            )
        {
            this.closure = closure;
            this.createResource = createResource;
        }

        public void Handle(Action<TClosure> action)
        {
            using (var resource = createResource().Result)
                action(resource);
        }

        public TOutput Handle<TOutput>(Func<TClosure, TOutput> func)
        {
            using (var resource = createResource().Result)
                return func(resource);
        }

        public async Task<TOutput> HandleAsync<TOutput>(Func<TClosure, Task<TOutput>> func)
        {
            using (var dbContext = await createResource())
                return await func(dbContext);
        }

        public void Handle(Action<TClosure, TSuperClosure> action)
        {
            using (var resource = createResource().Result)
                action(resource, closure);
        }

        public TOutput Handle<TOutput>(Func<TClosure, TSuperClosure, TOutput> func)
        {
            using (var resource = createResource().Result)
                return func(resource, closure);
        }

        public async Task<TOutput> HandleAsync<TOutput>(Func<TClosure, TSuperClosure, Task<TOutput>> func)
        {
            using (var resource = await createResource())
                return await func(resource, closure);
        }

        public async Task HandleAsync(Func<TClosure, Task> func)
        {
            using (var dbContext = await createResource())
                await func(dbContext);
        }

        public async Task HandleAsync(Func<TClosure, TSuperClosure, Task> func)
        {
            using (var resource = await createResource())
                await func(resource, closure);
        }

    }

    // ---------------------------------------------------------------------------

    public class HandlerUno<TClosure> : IHandler<TClosure>
    {
        readonly Func<TClosure> createResource;

        public HandlerUno(
                Func<TClosure> createResource
            )
        {
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


        public async Task HandleAsync(Func<TClosure, Task> func)
        {
            var dbContext = createResource();
            await func(dbContext);
        }

    }

    public class DisposeHandlerUno<TClosure> : IHandler<TClosure> where TClosure : IDisposable
    {
        readonly Func<TClosure> createResource;

        public DisposeHandlerUno(
                Func<TClosure> createResource
            )
        {
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
        

        public async Task HandleAsync(Func<TClosure, Task> func)
        {
            using (var dbContext = createResource())
                await func(dbContext);
        }
    }

    public class DisposeHandlerUno<TClosure, TDerivedClosure> : IHandler<TClosure> where TDerivedClosure : IDisposable, TClosure
    {
        readonly Func<TDerivedClosure> createResource;

        public DisposeHandlerUno(
                Func<TDerivedClosure> createResource
            )
        {
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

        public async Task HandleAsync(Func<TClosure, Task> func)
        {
            using (var dbContext = createResource())
                await func(dbContext);
        }
    }

    public class HandlerUnoAsync<TClosure> : IHandler<TClosure>
    {
        readonly Func<Task<TClosure>> createResource;

        public HandlerUnoAsync(
                Func<Task<TClosure>> createResource
            )
        {
            this.createResource = createResource;
        }

        public void Handle(Action<TClosure> action)
        {
            var resource = createResource().Result;
            action(resource);
        }

        public TOutput Handle<TOutput>(Func<TClosure, TOutput> func)
        {
            var resource = createResource().Result;
            return func(resource);
        }

        public async Task<TOutput> HandleAsync<TOutput>(Func<TClosure, Task<TOutput>> func)
        {
            var dbContext = await createResource();
            return await func(dbContext);
        }

        public async Task HandleAsync(Func<TClosure, Task> func)
        {
            var dbContext = await createResource();
            await func(dbContext);
        }

    }

    public class DisposeHandlerUnoAsync<TClosure> : IHandler<TClosure> where TClosure : IDisposable
    {
        readonly Func<Task<TClosure>> createResource;

        public DisposeHandlerUnoAsync(
                Func<Task<TClosure>> createResource
            )
        {
            this.createResource = createResource;
        }

        public void Handle(Action<TClosure> action)
        {
            using (var resource = createResource().Result)
                action(resource);
        }

        public TOutput Handle<TOutput>(Func<TClosure, TOutput> func)
        {
            using (var resource = createResource().Result)
                return func(resource);
        }

        public async Task<TOutput> HandleAsync<TOutput>(Func<TClosure, Task<TOutput>> func)
        {
            using (var dbContext = await createResource())
                return await func(dbContext);
        }

        public async Task HandleAsync(Func<TClosure, Task> func)
        {
            using (var dbContext = await createResource())
                await func(dbContext);
        }
    }

    public class DisposeHandlerUnoAsync<TClosure, TDerivedClosure> : IHandler<TClosure> where TDerivedClosure : IDisposable, TClosure
    {
        readonly Func<Task<TDerivedClosure>> createResource;

        public DisposeHandlerUnoAsync(
                Func<Task<TDerivedClosure>> createResource
            )
        {
            this.createResource = createResource;
        }

        public void Handle(Action<TClosure> action)
        {
            using (var resource = createResource().Result)
                action(resource);
        }

        public TOutput Handle<TOutput>(Func<TClosure, TOutput> func)
        {
            using (var resource = createResource().Result)
                return func(resource);
        }

        public async Task<TOutput> HandleAsync<TOutput>(Func<TClosure, Task<TOutput>> func)
        {
            using (var dbContext = await createResource())
                return await func(dbContext);
        }

        public async Task HandleAsync(Func<TClosure, Task> func)
        {
            using (var dbContext = await createResource())
                await func(dbContext);
        }

    }
}
