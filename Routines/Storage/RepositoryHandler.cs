using System;
using System.Threading.Tasks;

namespace DashboardCode.Routines.Storage
{
    public class ReliantRepositoryHandler<TEntity> where TEntity : class
    {
        readonly IRepository<TEntity> repository;
        internal ReliantRepositoryHandler(
                IRepository<TEntity> repository
            )
        {
            this.repository = repository;
        }

        public void Handle(Action<IRepository<TEntity>> action)
        {
            action(repository);
        }

        public TOutput Handle<TOutput>(Func<IRepository<TEntity>, TOutput> func)
        {
            return func(repository);
        }

        public Task<TOutput> HandleAsync<TOutput>(Func<IRepository<TEntity>, Task<TOutput>> func)
        {
            return func(repository);
        }
    }

    public interface IIndependentHandler<TResource, TUserContext> 
    {
        void Handle(Action<TResource> action);
        TOutput Handle<TOutput>(Func<TResource, TOutput> func);
        Task<TOutput> HandleAsync<TOutput>(Func<TResource, Task<TOutput>> func);
        void Handle(Action<TResource, RoutineClosure<TUserContext>> action);
        TOutput Handle<TOutput>(Func<TResource, RoutineClosure<TUserContext>, TOutput> func);
        Task<TOutput> HandleAsync<TOutput>(Func<TResource, RoutineClosure<TUserContext>, Task<TOutput>> func);
    }


    public interface IIndependentRepositoryHandler<TUserContext, TEntity> : IIndependentHandler<IRepository<TEntity>, TUserContext>
        where TEntity : class
    {
    }

    public class IndependentRepositoryHandler<TUserContext, TDbContext, TEntity> : IIndependentRepositoryHandler<TUserContext, TEntity> where TEntity : class where TDbContext : IDisposable
    {
        readonly RoutineClosure<TUserContext> closure;
        readonly Func<TDbContext, IRepository<TEntity>> createRepository;
        Func<TDbContext> dbContextFactory;
        internal IndependentRepositoryHandler(
                RoutineClosure<TUserContext> closure,
                Func<TDbContext> dbContextFactory,
                Func<TDbContext, IRepository<TEntity>> createRepository
            )
        {
            this.closure = closure;
            this.dbContextFactory = dbContextFactory;
            this.createRepository = createRepository;
        }

        public void Handle(Action<IRepository<TEntity>> action)
        {
            using (var dbContext = dbContextFactory())
                action(createRepository(dbContext));
        }

        public TOutput Handle<TOutput>(Func<IRepository<TEntity>, TOutput> func)
        {
            using (var dbContext = dbContextFactory())
                return func(createRepository(dbContext));
        }

        public async Task<TOutput> HandleAsync<TOutput>(Func<IRepository<TEntity>, Task<TOutput>> func)
        {
            using (var dbContext = dbContextFactory())
                return await func(createRepository(dbContext));
        }

        public void Handle(Action<IRepository<TEntity>, RoutineClosure<TUserContext>> action)
        {
            using (var dbContext = dbContextFactory())
                action(createRepository(dbContext), closure);
        }

        public TOutput Handle<TOutput>(Func<IRepository<TEntity>, RoutineClosure<TUserContext>, TOutput> func)
        {
            using (var dbContext = dbContextFactory())
                return func(createRepository(dbContext), closure);
        }

        public async Task<TOutput> HandleAsync<TOutput>(Func<IRepository<TEntity>, RoutineClosure<TUserContext>, Task<TOutput>> func)
        {
            using (var dbContext = dbContextFactory())
                return await func(createRepository(dbContext), closure);
        }
    }

    public class IndependentDbContextHandler<TUserContext, TDbContext> : IIndependentHandler<TDbContext, TUserContext>
        where TDbContext : IDisposable
    {
        readonly RoutineClosure<TUserContext> closure;
        Func<TDbContext> dbContextFactory;
        internal IndependentDbContextHandler(
                RoutineClosure<TUserContext> closure,
                Func<TDbContext> dbContextFactory
            )
        {
            this.closure = closure;
            this.dbContextFactory = dbContextFactory;
        }

        public void Handle(Action<TDbContext> action)
        {
            using (var dbContext = dbContextFactory())
                action(dbContext);
        }

        public TOutput Handle<TOutput>(Func<TDbContext, TOutput> func)
        {
            using (var dbContext = dbContextFactory())
                return func(dbContext);
        }

        public async Task<TOutput> HandleAsync<TOutput>(Func<TDbContext, Task<TOutput>> func)
        {
            using (var dbContext = dbContextFactory())
                return await func(dbContext);
        }

        public void Handle(Action<TDbContext, RoutineClosure<TUserContext>> action)
        {
            using (var dbContext = dbContextFactory())
                action(dbContext, closure);
        }

        public TOutput Handle<TOutput>(Func<TDbContext, RoutineClosure<TUserContext>, TOutput> func)
        {
            using (var dbContext = dbContextFactory())
                return func(dbContext, closure);
        }

        public async Task<TOutput> HandleAsync<TOutput>(Func<TDbContext, RoutineClosure<TUserContext>, Task<TOutput>> func)
        {
            using (var dbContext = dbContextFactory())
                return await func(dbContext, closure);
        }
    }
}