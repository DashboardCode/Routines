using System;
using System.Threading.Tasks;

namespace DashboardCode.Routines.Injected
{
    public interface IResourceHandler<TUserContext, TResource>
    {
        void Handle(Action<TResource> action);
        TOutput Handle<TOutput>(Func<TResource, TOutput> func);
        Task<TOutput> HandleAsync<TOutput>(Func<TResource, Task<TOutput>> func);
        void Handle(Action<TResource, RoutineClosure<TUserContext>> action);
        TOutput Handle<TOutput>(Func<TResource, RoutineClosure<TUserContext>, TOutput> func);
        Task<TOutput> HandleAsync<TOutput>(Func<TResource, RoutineClosure<TUserContext>, Task<TOutput>> func);
    }

    public class ResourceHandler<TUserContext, TResource> : IResourceHandler<TUserContext, TResource>
        where TResource : IDisposable
    {
        readonly RoutineClosure<TUserContext> closure;
        Func<TResource> resourceFactory;
        internal ResourceHandler(
                RoutineClosure<TUserContext> closure,
                Func<TResource> resourceFactory
            )
        {
            this.closure = closure;
            this.resourceFactory = resourceFactory;
        }

        public void Handle(Action<TResource> action)
        {
            using (var resource = resourceFactory())
                action(resource);
        }

        public TOutput Handle<TOutput>(Func<TResource, TOutput> func)
        {
            using (var resource = resourceFactory())
                return func(resource);
        }

        public async Task<TOutput> HandleAsync<TOutput>(Func<TResource, Task<TOutput>> func)
        {
            using (var dbContext = resourceFactory())
                return await func(dbContext);
        }

        public void Handle(Action<TResource, RoutineClosure<TUserContext>> action)
        {
            using (var resource = resourceFactory())
                action(resource, closure);
        }

        public TOutput Handle<TOutput>(Func<TResource, RoutineClosure<TUserContext>, TOutput> func)
        {
            using (var resource = resourceFactory())
                return func(resource, closure);
        }

        public async Task<TOutput> HandleAsync<TOutput>(Func<TResource, RoutineClosure<TUserContext>, Task<TOutput>> func)
        {
            using (var resource = resourceFactory())
                return await func(resource, closure);
        }
    }
}
