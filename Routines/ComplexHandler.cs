using System;
using System.Threading.Tasks;

namespace DashboardCode.Routines
{
    public class ComplexHandler<TClosure, TSuperClosure> : IHandler<TClosure, TSuperClosure>
    {
        readonly Func<TSuperClosure, TClosure> createResource;
        readonly IHandler<TSuperClosure> handler;

        public ComplexHandler(
            Func<TSuperClosure, TClosure> createResource,
            IHandler<TSuperClosure> handler
            )
        {
            this.createResource = createResource;
            this.handler = handler;
        }

        public void Handle(Action<TClosure> action) =>
            handler.Handle(closure =>
            {
                var resource = createResource(closure);
                action(resource);
            });

        public TOutput Handle<TOutput>(Func<TClosure, TOutput> func) =>
            handler.Handle(closure =>
            {
                var resource = createResource(closure);
                return func(resource);
            });


        public void Handle(Action<TClosure, TSuperClosure> action) =>
            handler.Handle(closure =>
            {
                var resource = createResource(closure);
                action(resource, closure);
            });

        public TOutput Handle<TOutput>(Func<TClosure, TSuperClosure, TOutput> func) =>
            handler.Handle(closure =>
            {
                var resource = createResource(closure);
                return func(resource, closure);
            });

        public Task<TOutput> HandleAsync<TOutput>(Func<TClosure, Task<TOutput>> func) =>
            handler.HandleAsync(async closure =>
            {
                var resource = createResource(closure);
                return await func(resource);
            });

        public Task HandleAsync(Func<TClosure, Task> func) =>
            handler.HandleAsync(async closure =>
            {
                var resource = createResource(closure);
                await func(resource);
            });

        public Task<TOutput> HandleAsync<TOutput>(Func<TClosure, TSuperClosure, Task<TOutput>> func) =>
            handler.HandleAsync(async closure =>
            {
                var resource = createResource(closure);
                return await func(resource, closure);
            });

        public Task HandleAsync(Func<TClosure, TSuperClosure, Task> func) =>
            handler.HandleAsync(async closure =>
            {
                var resource = createResource(closure);
                await func(resource, closure);
            });
    }

    public class ComplexDisposeHandler<TClosure, TSuperClosure> : IHandler<TClosure, TSuperClosure> where TClosure : IDisposable
    {
        readonly Func<TSuperClosure, TClosure> createResource;
        readonly IHandler<TSuperClosure> handler;

        public ComplexDisposeHandler(
            Func<TSuperClosure, TClosure> createResource,
            IHandler<TSuperClosure> handler
            )
        {
            this.createResource = createResource;
            this.handler = handler;
        }

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


        public void Handle(Action<TClosure, TSuperClosure> action) =>
            handler.Handle(closure =>
            {
                using (var resource = createResource(closure))
                    action(resource, closure);
            });

        public TOutput Handle<TOutput>(Func<TClosure, TSuperClosure, TOutput> func) =>
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

        public Task<TOutput> HandleAsync<TOutput>(Func<TClosure, TSuperClosure, Task<TOutput>> func) =>
            handler.HandleAsync(async closure =>
            {
                using (var resource = createResource(closure))
                    return await func(resource, closure);
            });

        public Task HandleAsync(Func<TClosure, TSuperClosure, Task> func) =>
            handler.HandleAsync(async closure =>
            {
                using (var resource = createResource(closure))
                    await func(resource, closure);
            });
    }

    public class ComplexDisposeHandler<TClosure, TSuperClosure, TDerivedClosure> : IHandler<TClosure, TSuperClosure>
        where TDerivedClosure : IDisposable, TClosure
    {
        readonly Func<TSuperClosure, TDerivedClosure> createResource;
        readonly IHandler<TSuperClosure> routineHandler;

        public ComplexDisposeHandler(
            Func<TSuperClosure, TDerivedClosure> createResource,
            IHandler<TSuperClosure> routineHandler
            )
        {
            this.createResource = createResource;
            this.routineHandler = routineHandler;
        }

        public void Handle(Action<TClosure> action) =>
            routineHandler.Handle(closure =>
            {
                using (var resource = createResource(closure))
                    action(resource);
            });

        public TOutput Handle<TOutput>(Func<TClosure, TOutput> func) =>
            routineHandler.Handle(closure =>
            {
                using (var resource = createResource(closure))
                    return func(resource);
            });

        public void Handle(Action<TClosure, TSuperClosure> action) =>
            routineHandler.Handle(closure =>
            {
                using (var resource = createResource(closure))
                    action(resource, closure);
            });

        public TOutput Handle<TOutput>(Func<TClosure, TSuperClosure, TOutput> func) =>
            routineHandler.Handle(closure =>
            {
                using (var resource = createResource(closure))
                    return func(resource, closure);
            });

        public Task<TOutput> HandleAsync<TOutput>(Func<TClosure, Task<TOutput>> func) =>
            routineHandler.HandleAsync(async closure =>
            {
                using (var resource = createResource(closure))
                    return await func(resource);
            });

        public Task HandleAsync(Func<TClosure, Task> func) =>
            routineHandler.HandleAsync(async closure =>
            {
                using (var resource = createResource(closure))
                    await func(resource);
            });

        public Task<TOutput> HandleAsync<TOutput>(Func<TClosure, TSuperClosure, Task<TOutput>> func) =>
            routineHandler.HandleAsync(async closure =>
            {
                using (var resource = createResource(closure))
                    return await func(resource, closure);
            });

        public Task HandleAsync(Func<TClosure, TSuperClosure, Task> func) =>
            routineHandler.HandleAsync(async closure =>
            {
                using (var resource = createResource(closure))
                    await func(resource, closure);
            });
    }

    public class ComplexHandlerAsync<TClosure, TSuperClosure> : IHandler<TClosure, TSuperClosure>
    {
        readonly Func<TSuperClosure, Task<TClosure>> createResource;
        readonly IHandler<TSuperClosure> handler;

        public ComplexHandlerAsync(
            Func<TSuperClosure, Task<TClosure>> createResource,
            IHandler<TSuperClosure> handler
            )
        {
            this.createResource = createResource;
            this.handler = handler;
        }

        public void Handle(Action<TClosure> action) =>
            handler.Handle(closure =>
            {
                var resource = createResource(closure).Result;
                action(resource);
            });

        public TOutput Handle<TOutput>(Func<TClosure, TOutput> func) =>
            handler.Handle(closure =>
            {
                var resource = createResource(closure).Result;
                return func(resource);
            });


        public void Handle(Action<TClosure, TSuperClosure> action) =>
            handler.Handle(closure =>
            {
                var resource = createResource(closure).Result;
                action(resource, closure);
            });

        public TOutput Handle<TOutput>(Func<TClosure, TSuperClosure, TOutput> func) =>
            handler.Handle(closure =>
            {
                var resource = createResource(closure).Result;
                return func(resource, closure);
            });

        public Task<TOutput> HandleAsync<TOutput>(Func<TClosure, Task<TOutput>> func) =>
            handler.HandleAsync(async closure =>
            {
                var resource = await createResource(closure);
                return await func(resource);
            });

        public Task HandleAsync(Func<TClosure, Task> func) =>
            handler.HandleAsync(async closure =>
            {
                var resource = await createResource(closure);
                await func(resource);
            });

        public Task<TOutput> HandleAsync<TOutput>(Func<TClosure, TSuperClosure, Task<TOutput>> func) =>
            handler.HandleAsync(async closure =>
            {
                var resource = await createResource(closure);
                return await func(resource, closure);
            });

        public Task HandleAsync(Func<TClosure, TSuperClosure, Task> func) =>
            handler.HandleAsync(async closure =>
            {
                var resource = await createResource(closure);
                await func(resource, closure);
            });
    }

    public class ComplexDisposeHandlerAsync<TClosure, TSuperClosure> : IHandler<TClosure, TSuperClosure> where TClosure : IDisposable
    {
        readonly Func<TSuperClosure, Task<TClosure>> createResource;
        readonly IHandler<TSuperClosure> handler;

        public ComplexDisposeHandlerAsync(
            Func<TSuperClosure, Task<TClosure>> createResource,
            IHandler<TSuperClosure> handler
            )
        {
            this.createResource = createResource;
            this.handler = handler;
        }

        public void Handle(Action<TClosure> action) =>
            handler.Handle(closure =>
            {
                using (var resource = createResource(closure).Result)
                    action(resource);
            });

        public TOutput Handle<TOutput>(Func<TClosure, TOutput> func) =>
            handler.Handle(closure =>
            {
                using (var resource = createResource(closure).Result)
                    return func(resource);
            });


        public void Handle(Action<TClosure, TSuperClosure> action) =>
            handler.Handle(closure =>
            {
                using (var resource = createResource(closure).Result)
                    action(resource, closure);
            });

        public TOutput Handle<TOutput>(Func<TClosure, TSuperClosure, TOutput> func) =>
            handler.Handle(closure =>
            {
                using (var resource = createResource(closure).Result)
                    return func(resource, closure);
            });

        public Task<TOutput> HandleAsync<TOutput>(Func<TClosure, Task<TOutput>> func) =>
            handler.HandleAsync(async closure =>
            {
                using (var resource = await createResource(closure))
                    return await func(resource);
            });

        public Task HandleAsync(Func<TClosure, Task> func) =>
            handler.HandleAsync(async closure =>
            {
                using (var resource = await createResource(closure))
                    await func(resource);
            });

        public Task<TOutput> HandleAsync<TOutput>(Func<TClosure, TSuperClosure, Task<TOutput>> func) =>
            handler.HandleAsync(async closure =>
            {
                using (var resource = await createResource(closure))
                    return await func(resource, closure);
            });

        public Task HandleAsync(Func<TClosure, TSuperClosure, Task> func) =>
            handler.HandleAsync(async closure =>
            {
                using (var resource = await createResource(closure))
                    await func(resource, closure);
            });
    }

    public class ComplexDisposeHandlerAsync<TClosure, TSuperClosure, TDerivedClosure> : IHandler<TClosure, TSuperClosure>
        where TDerivedClosure : IDisposable, TClosure
    {
        readonly Func<TSuperClosure, Task<TDerivedClosure>> createResource;
        readonly IHandler<TSuperClosure> routineHandler;

        public ComplexDisposeHandlerAsync(
            Func<TSuperClosure, Task<TDerivedClosure>> createResource,
            IHandler<TSuperClosure> routineHandler
            )
        {
            this.createResource = createResource;
            this.routineHandler = routineHandler;
        }

        public void Handle(Action<TClosure> action) =>
            routineHandler.Handle(closure =>
            {
                using (var resource = createResource(closure).Result)
                    action(resource);
            });

        public TOutput Handle<TOutput>(Func<TClosure, TOutput> func) =>
            routineHandler.Handle(closure =>
            {
                using (var resource = createResource(closure).Result)
                    return func(resource);
            });

        public void Handle(Action<TClosure, TSuperClosure> action) =>
            routineHandler.Handle(closure =>
            {
                using (var resource = createResource(closure).Result)
                    action(resource, closure);
            });

        public TOutput Handle<TOutput>(Func<TClosure, TSuperClosure, TOutput> func) =>
            routineHandler.Handle(closure =>
            {
                using (var resource = createResource(closure).Result)
                    return func(resource, closure);
            });

        public Task<TOutput> HandleAsync<TOutput>(Func<TClosure, Task<TOutput>> func) =>
            routineHandler.HandleAsync(async closure =>
            {
                using (var resource = await createResource(closure))
                    return await func(resource);
            });

        public Task HandleAsync(Func<TClosure, Task> func) =>
            routineHandler.HandleAsync(async closure =>
            {
                using (var resource = await createResource(closure))
                    await func(resource);
            });

        public Task<TOutput> HandleAsync<TOutput>(Func<TClosure, TSuperClosure, Task<TOutput>> func) =>
            routineHandler.HandleAsync(async closure =>
            {
                using (var resource = await createResource(closure))
                    return await func(resource, closure);
            });

        public Task HandleAsync(Func<TClosure, TSuperClosure, Task> func) =>
            routineHandler.HandleAsync(async closure =>
            {
                using (var resource = await createResource(closure))
                    await func(resource, closure);
            });
    }
}
