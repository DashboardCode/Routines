using System;
using System.Threading.Tasks;

namespace DashboardCode.Routines
{
    public class ComplexRoutineDisposeHandler<TClosure, TUserContext> : IRoutineHandler<TClosure, TUserContext> where TClosure : IDisposable
    {
        readonly Func<RoutineClosure<TUserContext>, TClosure> createResource;
        readonly IHandler<RoutineClosure<TUserContext>> handler;

        public ComplexRoutineDisposeHandler(
            Func<RoutineClosure<TUserContext>, TClosure> createResource,
            IHandler<RoutineClosure<TUserContext>> handler
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
    }

    public class ComplexRoutineDisposeHandler<TClosure, TUserContext, TDerivedClosure> : IRoutineHandler<TClosure, TUserContext>
        where TDerivedClosure : IDisposable, TClosure
    {
        readonly Func<RoutineClosure<TUserContext>, TDerivedClosure> createResource;
        readonly IHandler<RoutineClosure<TUserContext>> routineHandler;

        public ComplexRoutineDisposeHandler(
            Func<RoutineClosure<TUserContext>, TDerivedClosure> createResource,
            IHandler<RoutineClosure<TUserContext>> routineHandler
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

        public void Handle(Action<TClosure, RoutineClosure<TUserContext>> action) =>
            routineHandler.Handle(closure =>
            {
                using (var resource = createResource(closure))
                    action(resource, closure);
            });

        public TOutput Handle<TOutput>(Func<TClosure, RoutineClosure<TUserContext>, TOutput> func) =>
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

        public Task<TOutput> HandleAsync<TOutput>(Func<TClosure, RoutineClosure<TUserContext>, Task<TOutput>> func) =>
            routineHandler.HandleAsync(async closure =>
            {
                using (var resource = createResource(closure))
                    return await func(resource, closure);
            });

        public Task HandleAsync(Func<TClosure, RoutineClosure<TUserContext>, Task> func) =>
            routineHandler.HandleAsync(async closure =>
            {
                using (var resource = createResource(closure))
                    await func(resource, closure);
            });
    }

    public class ComplexRoutineHandler<TClosure, TUserContext> : IRoutineHandler<TClosure, TUserContext>
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


        public void Handle(Action<TClosure, RoutineClosure<TUserContext>> action) =>
            handler.Handle(closure =>
            {
                var resource = createResource(closure);
                action(resource, closure);
            });

        public TOutput Handle<TOutput>(Func<TClosure, RoutineClosure<TUserContext>, TOutput> func) =>
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

        public Task<TOutput> HandleAsync<TOutput>(Func<TClosure, RoutineClosure<TUserContext>, Task<TOutput>> func) =>
            handler.HandleAsync(async closure =>
            {
                var resource = createResource(closure);
                return await func(resource, closure);
            });

        public Task HandleAsync(Func<TClosure, RoutineClosure<TUserContext>, Task> func) =>
            handler.HandleAsync(async closure =>
            {
                var resource = createResource(closure);
                await func(resource, closure);
            });
    }

    public class ComplexRoutineHandler<TClosure, TUserContext, TDerivedClosure> : IRoutineHandler<TClosure, TUserContext>
        where TDerivedClosure : TClosure
    {
        readonly Func<RoutineClosure<TUserContext>, TDerivedClosure> createResource;
        readonly IHandler<RoutineClosure<TUserContext>> routineHandler;

        public ComplexRoutineHandler(
            Func<RoutineClosure<TUserContext>, TDerivedClosure> createResource,
            IHandler<RoutineClosure<TUserContext>> routineHandler
            )
        {
            this.createResource = createResource;
            this.routineHandler = routineHandler;
        }

        public void Handle(Action<TClosure> action) =>
            routineHandler.Handle(closure =>
            {
                var resource = createResource(closure);
                action(resource);
            });

        public TOutput Handle<TOutput>(Func<TClosure, TOutput> func) =>
            routineHandler.Handle(closure =>
            {
                var resource = createResource(closure);
                return func(resource);
            });

        public void Handle(Action<TClosure, RoutineClosure<TUserContext>> action) =>
            routineHandler.Handle(closure =>
            {
                var resource = createResource(closure);
                action(resource, closure);
            });

        public TOutput Handle<TOutput>(Func<TClosure, RoutineClosure<TUserContext>, TOutput> func) =>
            routineHandler.Handle(closure =>
            {
                var resource = createResource(closure);
                return func(resource, closure);
            });

        public Task<TOutput> HandleAsync<TOutput>(Func<TClosure, Task<TOutput>> func) =>
            routineHandler.HandleAsync(async closure =>
            {
                var resource = createResource(closure);
                return await func(resource);
            });

        public Task HandleAsync(Func<TClosure, Task> func) =>
            routineHandler.HandleAsync(async closure =>
            {
                var resource = createResource(closure);
                await func(resource);
            });

        public Task<TOutput> HandleAsync<TOutput>(Func<TClosure, RoutineClosure<TUserContext>, Task<TOutput>> func) =>
            routineHandler.HandleAsync(async closure =>
            {
                var resource = createResource(closure);
                return await func(resource, closure);
            });

        public Task HandleAsync(Func<TClosure, RoutineClosure<TUserContext>, Task> func) =>
            routineHandler.HandleAsync(async closure =>
            {
                var resource = createResource(closure);
                await func(resource, closure);
            });
    }
}
