using System;
using System.Threading.Tasks;

namespace DashboardCode.Routines
{
    public interface IComplexHandler<TClosure, TSuperClosure>
    {
        void Handle(Action<TClosure> action);
        TOutput Handle<TOutput>(Func<TClosure, TOutput> func);
        void Handle(Action<TClosure, TSuperClosure> action);
        TOutput Handle<TOutput>(Func<TClosure, TSuperClosure, TOutput> func);
    }

    public interface IComplexHandlerAsync<TClosure, TSuperClosure>
    {
        Task<TOutput> HandleAsync<TOutput>(Func<TClosure, Task<TOutput>> func);
        Task HandleAsync(Func<TClosure, Task> func);
        Task<TOutput> HandleAsync<TOutput>(Func<TClosure, TSuperClosure, Task<TOutput>> func);
        Task HandleAsync(Func<TClosure, TSuperClosure, Task> func);
    }

    // -----------------------
    public class ComplexHandler<TClosure, TSuperClosure> : IHandler<TClosure, TSuperClosure>
    {
        readonly Func<TSuperClosure, TClosure> createClosure;
        readonly IHandler<TSuperClosure> handler;

        public ComplexHandler(
            Func<TSuperClosure, TClosure> createClosure,
            IHandler<TSuperClosure> handler
            )
        {
            this.createClosure = createClosure;
            this.handler = handler;
        }

        public void Handle(Action<TClosure> action) =>
            handler.Handle(superClosure =>
            {
                var closure = createClosure(superClosure);
                action(closure);
            });

        public TOutput Handle<TOutput>(Func<TClosure, TOutput> func) =>
            handler.Handle(superClosure =>
            {
                var closure = createClosure(superClosure);
                return func(closure);
            });

        public void Handle(Action<TClosure, TSuperClosure> action) =>
            handler.Handle(superClosure =>
            {
                var closure = createClosure(superClosure);
                action(closure, superClosure);
            });

        public TOutput Handle<TOutput>(Func<TClosure, TSuperClosure, TOutput> func) =>
            handler.Handle(superClosure =>
            {
                var resource = createClosure(superClosure);
                return func(resource, superClosure);
            });
    }
    public class ComplexDisposeHandler<TClosure, TSuperClosure> : IHandler<TClosure, TSuperClosure> where TClosure : IDisposable
    {
        readonly Func<TSuperClosure, TClosure> createClosure;
        readonly IHandler<TSuperClosure> handler;

        public ComplexDisposeHandler(
            Func<TSuperClosure, TClosure> createClosure,
            IHandler<TSuperClosure> handler
            )
        {
            this.createClosure = createClosure;
            this.handler = handler;
        }

        public void Handle(Action<TClosure> action) =>
            handler.Handle(superClosure =>
            {
                using var resource = createClosure(superClosure);
                action(resource);
            });

        public TOutput Handle<TOutput>(Func<TClosure, TOutput> func) =>
            handler.Handle(superClosure =>
            {
                using var resource = createClosure(superClosure);
                return func(resource);
            });


        public void Handle(Action<TClosure, TSuperClosure> action) =>
            handler.Handle(superClosure =>
            {
                using var resource = createClosure(superClosure);
                action(resource, superClosure);
            });

        public TOutput Handle<TOutput>(Func<TClosure, TSuperClosure, TOutput> func) =>
            handler.Handle(superClosure =>
            {
                using var resource = createClosure(superClosure);
                return func(resource, superClosure);
            });

        
    }
    public class ComplexDisposeHandler<TClosure, TSuperClosure, TDerivedClosure> : IHandler<TClosure, TSuperClosure>
        where TDerivedClosure : IDisposable, TClosure
    {
        readonly Func<TSuperClosure, TDerivedClosure> createClosure;
        readonly IHandler<TSuperClosure> routineHandler;

        public ComplexDisposeHandler(
            Func<TSuperClosure, TDerivedClosure> createResource,
            IHandler<TSuperClosure> routineHandler
            )
        {
            this.createClosure = createResource;
            this.routineHandler = routineHandler;
        }

        public void Handle(Action<TClosure> action) =>
            routineHandler.Handle(superClosure =>
            {
                using var closure = createClosure(superClosure);
                action(closure);
            });

        public TOutput Handle<TOutput>(Func<TClosure, TOutput> func) =>
            routineHandler.Handle(superClosure =>
            {
                using var resource = createClosure(superClosure);
                return func(resource);
            });

        public void Handle(Action<TClosure, TSuperClosure> action) =>
            routineHandler.Handle(superClosure =>
            {
                using var closure = createClosure(superClosure);
                action(closure, superClosure);
            });

        public TOutput Handle<TOutput>(Func<TClosure, TSuperClosure, TOutput> func) =>
            routineHandler.Handle(superClosure =>
            {
                using var closure = createClosure(superClosure);
                return func(closure, superClosure);
            });
    }
    // -----------------------
    public class ComplexHandlerAsync<TClosure, TSuperClosure> : IHandlerAsync<TClosure, TSuperClosure>
    {
        readonly Func<TSuperClosure, TClosure> createClosure;
        readonly IHandlerAsync<TSuperClosure> handler;

        public ComplexHandlerAsync(
             Func<TSuperClosure, TClosure> createClosure,
             IHandlerAsync<TSuperClosure> handler
            )
        {
            this.createClosure = createClosure;
            this.handler = handler;
        }

        public Task<TOutput> HandleAsync<TOutput>(Func<TClosure, Task<TOutput>> func) =>
            handler.HandleAsync(async superClosure =>
            {
                var closure = createClosure(superClosure);
                return await func(closure);
            });

        public Task HandleAsync(Func<TClosure, Task> func) =>
            handler.HandleAsync(async superClosure =>
            {
                var closure = createClosure(superClosure);
                await func(closure);
            });

        public Task<TOutput> HandleAsync<TOutput>(Func<TClosure, TSuperClosure, Task<TOutput>> func) =>
            handler.HandleAsync(async superClosure =>
            {
                var closure = createClosure(superClosure);
                return await func(closure, superClosure);
            });

        public Task HandleAsync(Func<TClosure, TSuperClosure, Task> func) =>
            handler.HandleAsync(async superClosure =>
            {
                var closure = createClosure(superClosure);
                await func(closure, superClosure);
            });
    }
    public class ComplexDisposeHandlerAsync<TClosure, TSuperClosure> : IHandlerAsync<TClosure, TSuperClosure> where TClosure : IDisposable
    {
        readonly Func<TSuperClosure, TClosure> createClosure;
        readonly IHandlerAsync<TSuperClosure> handler;

        public ComplexDisposeHandlerAsync(
            Func<TSuperClosure, TClosure> createClosure,
            IHandlerAsync<TSuperClosure> handler
            )
        {
            this.createClosure = createClosure;
            this.handler = handler;
        }

        public Task<TOutput> HandleAsync<TOutput>(Func<TClosure, Task<TOutput>> func) =>
            handler.HandleAsync(async superClosure =>
            {
                using var resource = createClosure(superClosure);
                return await func(resource);
            });

        public Task HandleAsync(Func<TClosure, Task> func) =>
            handler.HandleAsync(async superClosure =>
            {
                using var resource = createClosure(superClosure);
                await func(resource);
            });

        public Task<TOutput> HandleAsync<TOutput>(Func<TClosure, TSuperClosure, Task<TOutput>> func) =>
            handler.HandleAsync(async superClosure =>
            {
                using var resource = createClosure(superClosure);
                return await func(resource, superClosure);
            });

        public Task HandleAsync(Func<TClosure, TSuperClosure, Task> func) =>
            handler.HandleAsync(async superClosure =>
            {
                using var resource = createClosure(superClosure);
                await func(resource, superClosure);
            });
    }
    public class ComplexDisposeHandlerAsync<TClosure, TSuperClosure, TDerivedClosure> : IHandlerAsync<TClosure, TSuperClosure>
        where TDerivedClosure : IDisposable, TClosure
    {
        readonly Func<TSuperClosure, TDerivedClosure> createClosure;
        readonly IHandlerAsync<TSuperClosure> routineHandler;

        public ComplexDisposeHandlerAsync(
            Func<TSuperClosure, TDerivedClosure> createResource,
            IHandlerAsync<TSuperClosure> routineHandler
            )
        {
            this.createClosure = createResource;
            this.routineHandler = routineHandler;
        }

        public Task<TOutput> HandleAsync<TOutput>(Func<TClosure, Task<TOutput>> func) =>
            routineHandler.HandleAsync(async superClosure =>
            {
                using var closure = createClosure(superClosure);
                return await func(closure);
            });

        public Task HandleAsync(Func<TClosure, Task> func) =>
            routineHandler.HandleAsync(async superClosure =>
            {
                using var closure = createClosure(superClosure);
                await func(closure);
            });

        public Task<TOutput> HandleAsync<TOutput>(Func<TClosure, TSuperClosure, Task<TOutput>> func) =>
            routineHandler.HandleAsync(async superClosure =>
            {
                using var closure = createClosure(superClosure);
                return await func(closure, superClosure);
            });

        public Task HandleAsync(Func<TClosure, TSuperClosure, Task> func) =>
            routineHandler.HandleAsync(async superClosure =>
            {
                using var closure = createClosure(superClosure);
                await func(closure, superClosure);
            });
    }
    // -----------------------
    public class ComplexHandlerAsync2<TClosure, TSuperClosure> : IHandlerAsync<TClosure, TSuperClosure>
    {
        readonly Func<TSuperClosure, Task<TClosure>> createClosure;
        readonly IHandlerAsync<TSuperClosure> handler;

        public ComplexHandlerAsync2(
             Func<TSuperClosure, Task<TClosure>> createClosure,
             IHandlerAsync<TSuperClosure> handler
            )
        {
            this.createClosure = createClosure;
            this.handler = handler;
        }

        public Task<TOutput> HandleAsync<TOutput>(Func<TClosure, Task<TOutput>> func) =>
            handler.HandleAsync(async superClosure =>
            {
                var closure = await createClosure(superClosure);
                return await func(closure);
            });

        public Task HandleAsync(Func<TClosure, Task> func) =>
            handler.HandleAsync(async superClosure =>
            {
                var closure = await createClosure(superClosure);
                await func(closure);
            });

        public Task<TOutput> HandleAsync<TOutput>(Func<TClosure, TSuperClosure, Task<TOutput>> func) =>
            handler.HandleAsync(async superClosure =>
            {
                var closure = await createClosure(superClosure);
                return await func(closure, superClosure);
            });

        public Task HandleAsync(Func<TClosure, TSuperClosure, Task> func) =>
            handler.HandleAsync(async superClosure =>
            {
                var closure = await createClosure(superClosure);
                await func(closure, superClosure);
            });
    }
    public class ComplexDisposeHandlerAsync2<TClosure, TSuperClosure> : IHandlerAsync<TClosure, TSuperClosure> where TClosure : IDisposable
    {
        readonly Func<TSuperClosure, Task<TClosure>> createClosure;
        readonly IHandlerAsync<TSuperClosure> handler;

        public ComplexDisposeHandlerAsync2(
            Func<TSuperClosure, Task<TClosure>> createClosure,
            IHandlerAsync<TSuperClosure> handler
            )
        {
            this.createClosure = createClosure;
            this.handler = handler;
        }

        public Task<TOutput> HandleAsync<TOutput>(Func<TClosure, Task<TOutput>> func) =>
            handler.HandleAsync(async superClosure =>
            {
                using var resource = await createClosure(superClosure);
                return await func(resource);
            });

        public Task HandleAsync(Func<TClosure, Task> func) =>
            handler.HandleAsync(async superClosure =>
            {
                using var resource = await createClosure(superClosure);
                await func(resource);
            });

        public Task<TOutput> HandleAsync<TOutput>(Func<TClosure, TSuperClosure, Task<TOutput>> func) =>
            handler.HandleAsync(async superClosure =>
            {
                using var resource = await createClosure(superClosure);
                return await func(resource, superClosure);
            });

        public Task HandleAsync(Func<TClosure, TSuperClosure, Task> func) =>
            handler.HandleAsync(async superClosure =>
            {
                using var resource = await createClosure(superClosure);
                await func(resource, superClosure);
            });
    }
    public class ComplexDisposeHandlerAsync2<TClosure, TSuperClosure, TDerivedClosure> : IHandlerAsync<TClosure, TSuperClosure>
        where TDerivedClosure : IDisposable, TClosure
    {
        readonly Func<TSuperClosure, Task<TDerivedClosure>> createClosure;
        readonly IHandlerAsync<TSuperClosure> routineHandler;

        public ComplexDisposeHandlerAsync2(
            Func<TSuperClosure, Task<TDerivedClosure>> createResource,
            IHandlerAsync<TSuperClosure> routineHandler
            )
        {
            this.createClosure = createResource;
            this.routineHandler = routineHandler;
        }

        public Task<TOutput> HandleAsync<TOutput>(Func<TClosure, Task<TOutput>> func) =>
            routineHandler.HandleAsync(async superClosure =>
            {
                using var closure = await createClosure(superClosure);
                return await func(closure);
            });

        public Task HandleAsync(Func<TClosure, Task> func) =>
            routineHandler.HandleAsync(async superClosure =>
            {
                using var closure = await createClosure(superClosure);
                await func(closure);
            });

        public Task<TOutput> HandleAsync<TOutput>(Func<TClosure, TSuperClosure, Task<TOutput>> func) =>
            routineHandler.HandleAsync(async superClosure =>
            {
                using var closure = await createClosure(superClosure);
                return await func(closure, superClosure);
            });

        public Task HandleAsync(Func<TClosure, TSuperClosure, Task> func) =>
            routineHandler.HandleAsync(async superClosure =>
            {
                using var closure = await createClosure(superClosure);
                await func(closure, superClosure);
            });
    }

    // -----------------------
    public class ComplexHandlerOmni<TClosure, TSuperClosure> : 
        IHandler<TClosure, TSuperClosure>, IHandlerAsync<TClosure, TSuperClosure>
    {
        readonly Func<TSuperClosure, TClosure> createClosure;
        readonly IHandlerOmni<TSuperClosure> handler;

        public ComplexHandlerOmni(
            Func<TSuperClosure, TClosure> createClosure,
            IHandlerOmni<TSuperClosure> handler
            )
        {
            this.createClosure = createClosure;
            this.handler = handler;
        }

        public void Handle(Action<TClosure> action) =>
            handler.Handle(superClosure =>
            {
                var closure = createClosure(superClosure);
                action(closure);
            });

        public TOutput Handle<TOutput>(Func<TClosure, TOutput> func) =>
            handler.Handle(superClosure =>
            {
                var closure = createClosure(superClosure);
                return func(closure);
            });

        public void Handle(Action<TClosure, TSuperClosure> action) =>
            handler.Handle(superClosure =>
            {
                var closure = createClosure(superClosure);
                action(closure, superClosure);
            });

        public TOutput Handle<TOutput>(Func<TClosure, TSuperClosure, TOutput> func) =>
            handler.Handle(superClosure =>
            {
                var resource = createClosure(superClosure);
                return func(resource, superClosure);
            });

        public Task<TOutput> HandleAsync<TOutput>(Func<TClosure, Task<TOutput>> func) =>
            handler.HandleAsync(async superClosure =>
            {
                var closure = createClosure(superClosure);
                return await func(closure);
            });

        public Task HandleAsync(Func<TClosure, Task> func) =>
            handler.HandleAsync(async superClosure =>
            {
                var closure = createClosure(superClosure);
                await func(closure);
            });

        public Task<TOutput> HandleAsync<TOutput>(Func<TClosure, TSuperClosure, Task<TOutput>> func) =>
            handler.HandleAsync(async superClosure =>
            {
                var closure = createClosure(superClosure);
                return await func(closure, superClosure);
            });

        public Task HandleAsync(Func<TClosure, TSuperClosure, Task> func) =>
            handler.HandleAsync(async superClosure =>
            {
                var closure = createClosure(superClosure);
                await func(closure, superClosure);
            });
    }
    public class ComplexDisposeHandlerOmni<TClosure, TSuperClosure> : 
        IHandler<TClosure, TSuperClosure>, 
        IHandlerAsync<TClosure, TSuperClosure> where TClosure : IDisposable
    {
        readonly Func<TSuperClosure, TClosure> createClosure;
        readonly IHandlerOmni<TSuperClosure> handler;

        public ComplexDisposeHandlerOmni(
            Func<TSuperClosure, TClosure> createClosure,
            IHandlerOmni<TSuperClosure> handler
            )
        {
            this.createClosure = createClosure;
            this.handler = handler;
        }

        public void Handle(Action<TClosure> action) =>
            handler.Handle(superClosure =>
            {
                using var resource = createClosure(superClosure);
                action(resource);
            });

        public TOutput Handle<TOutput>(Func<TClosure, TOutput> func) =>
            handler.Handle(superClosure =>
            {
                using var resource = createClosure(superClosure);
                return func(resource);
            });


        public void Handle(Action<TClosure, TSuperClosure> action) =>
            handler.Handle(superClosure =>
            {
                using var resource = createClosure(superClosure);
                action(resource, superClosure);
            });

        public TOutput Handle<TOutput>(Func<TClosure, TSuperClosure, TOutput> func) =>
            handler.Handle(superClosure =>
            {
                using var resource = createClosure(superClosure);
                return func(resource, superClosure);
            });
        public Task<TOutput> HandleAsync<TOutput>(Func<TClosure, Task<TOutput>> func) =>
            handler.HandleAsync(async superClosure =>
            {
                using var resource = createClosure(superClosure);
                return await func(resource);
            });

        public Task HandleAsync(Func<TClosure, Task> func) =>
            handler.HandleAsync(async superClosure =>
            {
                using var resource = createClosure(superClosure);
                await func(resource);
            });

        public Task<TOutput> HandleAsync<TOutput>(Func<TClosure, TSuperClosure, Task<TOutput>> func) =>
            handler.HandleAsync(async superClosure =>
            {
                using var resource = createClosure(superClosure);
                return await func(resource, superClosure);
            });

        public Task HandleAsync(Func<TClosure, TSuperClosure, Task> func) =>
            handler.HandleAsync(async superClosure =>
            {
                using var resource = createClosure(superClosure);
                await func(resource, superClosure);
            });

    }
    public class ComplexDisposeHandlerOmni<TClosure, TSuperClosure, TDerivedClosure> : 
        IHandler<TClosure, TSuperClosure>,
        IHandlerAsync<TClosure, TSuperClosure>
        where TDerivedClosure : IDisposable, TClosure
    {
        readonly Func<TSuperClosure, TDerivedClosure> createClosure;
        readonly IHandlerOmni<TSuperClosure> routineHandler;

        public ComplexDisposeHandlerOmni(
            Func<TSuperClosure, TDerivedClosure> createResource,
            IHandlerOmni<TSuperClosure> routineHandler
            )
        {
            this.createClosure = createResource;
            this.routineHandler = routineHandler;
        }

        public void Handle(Action<TClosure> action) =>
            routineHandler.Handle(superClosure =>
            {
                using var closure = createClosure(superClosure);
                action(closure);
            });

        public TOutput Handle<TOutput>(Func<TClosure, TOutput> func) =>
            routineHandler.Handle(superClosure =>
            {
                using var resource = createClosure(superClosure);
                return func(resource);
            });

        public void Handle(Action<TClosure, TSuperClosure> action) =>
            routineHandler.Handle(superClosure =>
            {
                using var closure = createClosure(superClosure);
                action(closure, superClosure);
            });

        public TOutput Handle<TOutput>(Func<TClosure, TSuperClosure, TOutput> func) =>
            routineHandler.Handle(superClosure =>
            {
                using var closure = createClosure(superClosure);
                return func(closure, superClosure);
            });

        public Task<TOutput> HandleAsync<TOutput>(Func<TClosure, Task<TOutput>> func) =>
            routineHandler.HandleAsync(async superClosure =>
            {
                using var closure = createClosure(superClosure);
                return await func(closure);
            });

        public Task HandleAsync(Func<TClosure, Task> func) =>
            routineHandler.HandleAsync(async superClosure =>
            {
                using var closure = createClosure(superClosure);
                await func(closure);
            });

        public Task<TOutput> HandleAsync<TOutput>(Func<TClosure, TSuperClosure, Task<TOutput>> func) =>
            routineHandler.HandleAsync(async superClosure =>
            {
                using var closure = createClosure(superClosure);
                return await func(closure, superClosure);
            });

        public Task HandleAsync(Func<TClosure, TSuperClosure, Task> func) =>
            routineHandler.HandleAsync(async superClosure =>
            {
                using var closure = createClosure(superClosure);
                await func(closure, superClosure);
            });
    }

    // -----------------------

    public class ComplexHandlerOmni2<TClosure, TSuperClosure> :
       IHandler<TClosure, TSuperClosure>, IHandlerAsync<TClosure, TSuperClosure>
    {
        readonly Func<TSuperClosure, Task<TClosure>> createClosure;
        readonly IHandlerOmni<TSuperClosure> handler;

        public ComplexHandlerOmni2(
            Func<TSuperClosure, Task<TClosure>> createClosure,
            IHandlerOmni<TSuperClosure> handler
            )
        {
            this.createClosure = createClosure;
            this.handler = handler;
        }

        public void Handle(Action<TClosure> action) =>
            handler.Handle(superClosure =>
            {
                var closure = createClosure(superClosure).Result;
                action(closure);
            });

        public TOutput Handle<TOutput>(Func<TClosure, TOutput> func) =>
            handler.Handle(superClosure =>
            {
                var closure = createClosure(superClosure).Result;
                return func(closure);
            });

        public void Handle(Action<TClosure, TSuperClosure> action) =>
            handler.Handle(superClosure =>
            {
                var closure = createClosure(superClosure).Result;
                action(closure, superClosure);
            });

        public TOutput Handle<TOutput>(Func<TClosure, TSuperClosure, TOutput> func) =>
            handler.Handle(superClosure =>
            {
                var resource = createClosure(superClosure).Result;
                return func(resource, superClosure);
            });

        public Task<TOutput> HandleAsync<TOutput>(Func<TClosure, Task<TOutput>> func) =>
                    handler.HandleAsync(async superClosure =>
                    {
                        var closure = await createClosure(superClosure);
                        return await func(closure);
                    });

        public Task HandleAsync(Func<TClosure, Task> func) =>
            handler.HandleAsync(async superClosure =>
            {
                var closure = await createClosure(superClosure);
                await func(closure);
            });

        public Task<TOutput> HandleAsync<TOutput>(Func<TClosure, TSuperClosure, Task<TOutput>> func) =>
            handler.HandleAsync(async superClosure =>
            {
                var closure = await createClosure(superClosure);
                return await func(closure, superClosure);
            });

        public Task HandleAsync(Func<TClosure, TSuperClosure, Task> func) =>
            handler.HandleAsync(async superClosure =>
            {
                var closure = await createClosure(superClosure);
                await func(closure, superClosure);
            });
    }
    public class ComplexDisposeHandlerOmni2<TClosure, TSuperClosure> :
        IHandler<TClosure, TSuperClosure>,
        IHandlerAsync<TClosure, TSuperClosure> where TClosure : IDisposable
    {
        readonly Func<TSuperClosure, Task<TClosure>> createClosure;
        readonly IHandlerOmni<TSuperClosure> handler;

        public ComplexDisposeHandlerOmni2(
            Func<TSuperClosure, Task<TClosure>> createClosure,
            IHandlerOmni<TSuperClosure> handler
            )
        {
            this.createClosure = createClosure;
            this.handler = handler;
        }

        public void Handle(Action<TClosure> action) =>
            handler.Handle(superClosure =>
            {
                using var resource = createClosure(superClosure).Result;
                action(resource);
            });

        public TOutput Handle<TOutput>(Func<TClosure, TOutput> func) =>
            handler.Handle(superClosure =>
            {
                using var resource = createClosure(superClosure).Result;
                return func(resource);
            });


        public void Handle(Action<TClosure, TSuperClosure> action) =>
            handler.Handle(superClosure =>
            {
                using var resource = createClosure(superClosure).Result;
                action(resource, superClosure);
            });

        public TOutput Handle<TOutput>(Func<TClosure, TSuperClosure, TOutput> func) =>
            handler.Handle(superClosure =>
            {
                using var resource = createClosure(superClosure).Result;
                return func(resource, superClosure);
            });

        public Task<TOutput> HandleAsync<TOutput>(Func<TClosure, Task<TOutput>> func) =>
            handler.HandleAsync(async superClosure =>
            {
                using var resource = await createClosure(superClosure);
                return await func(resource);
            });

        public Task HandleAsync(Func<TClosure, Task> func) =>
            handler.HandleAsync(async superClosure =>
            {
                using var resource = await createClosure(superClosure);
                await func(resource);
            });

        public Task<TOutput> HandleAsync<TOutput>(Func<TClosure, TSuperClosure, Task<TOutput>> func) =>
            handler.HandleAsync(async superClosure =>
            {
                using var resource = await createClosure(superClosure);
                return await func(resource, superClosure);
            });

        public Task HandleAsync(Func<TClosure, TSuperClosure, Task> func) =>
            handler.HandleAsync(async superClosure =>
            {
                using var resource = await createClosure(superClosure);
                await func(resource, superClosure);
            });
    }
    public class ComplexDisposeHandlerOmni2<TClosure, TSuperClosure, TDerivedClosure> :
        IHandler<TClosure, TSuperClosure>,
        IHandlerAsync<TClosure, TSuperClosure>
        where TDerivedClosure : IDisposable, TClosure
    {
        readonly Func<TSuperClosure, Task<TDerivedClosure>> createClosure;
        readonly IHandlerOmni<TSuperClosure> routineHandler;

        public ComplexDisposeHandlerOmni2(
            Func<TSuperClosure, Task<TDerivedClosure>> createClosure,
            IHandlerOmni<TSuperClosure> routineHandler
            )
        {
            this.createClosure = createClosure;
            this.routineHandler = routineHandler;
        }

        public void Handle(Action<TClosure> action) =>
            routineHandler.Handle(superClosure =>
            {
                using var closure = createClosure(superClosure).Result;
                action(closure);
            });

        public TOutput Handle<TOutput>(Func<TClosure, TOutput> func) =>
            routineHandler.Handle(superClosure =>
            {
                using var resource = createClosure(superClosure).Result;
                return func(resource);
            });

        public void Handle(Action<TClosure, TSuperClosure> action) =>
            routineHandler.Handle(superClosure =>
            {
                using var closure = createClosure(superClosure).Result;
                action(closure, superClosure);
            });

        public TOutput Handle<TOutput>(Func<TClosure, TSuperClosure, TOutput> func) =>
            routineHandler.Handle(superClosure =>
            {
                using var closure = createClosure(superClosure).Result;
                return func(closure, superClosure);
            });

        public Task<TOutput> HandleAsync<TOutput>(Func<TClosure, Task<TOutput>> func) =>
            routineHandler.HandleAsync(async superClosure =>
            {
                using var closure = await createClosure(superClosure);
                return await func(closure);
            });

        public Task HandleAsync(Func<TClosure, Task> func) =>
            routineHandler.HandleAsync(async superClosure =>
            {
                using var closure = await createClosure(superClosure);
                await func(closure);
            });

        public Task<TOutput> HandleAsync<TOutput>(Func<TClosure, TSuperClosure, Task<TOutput>> func) =>
            routineHandler.HandleAsync(async superClosure =>
            {
                using var closure = await createClosure(superClosure);
                return await func(closure, superClosure);
            });

        public Task HandleAsync(Func<TClosure, TSuperClosure, Task> func) =>
            routineHandler.HandleAsync(async superClosure =>
            {
                using var closure = await createClosure(superClosure);
                await func(closure, superClosure);
            });

    }
}
