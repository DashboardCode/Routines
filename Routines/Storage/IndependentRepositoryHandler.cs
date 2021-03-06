﻿using DashboardCode.Routines.Logging;
using System;
using System.Threading.Tasks;

namespace DashboardCode.Routines.Storage
{

    public interface IIndependentRepositoryHandler<TUserContext, TEntity> : IRoutineHandler<IRepository<TEntity>, TUserContext>
        where TEntity : class
    {
    }

    public class IndependentRepositoryHandler<TUserContext, TDbContext, TEntity> : IIndependentRepositoryHandler<TUserContext, TEntity> where TEntity : class where TDbContext : IDisposable
    {
        readonly RoutineClosure<TUserContext> closure;
        readonly Func<TDbContext, IRepository<TEntity>> createRepository;
        readonly Func<TDbContext> dbContextFactory;
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

        public async Task HandleAsync(Func<IRepository<TEntity>, Task> func)
        {
            using (var dbContext = dbContextFactory())
                await func(createRepository(dbContext));
        }

        public async Task HandleAsync(Func<IRepository<TEntity>, RoutineClosure<TUserContext>, Task> func)
        {
            using (var dbContext = dbContextFactory())
                await func(createRepository(dbContext), closure);
        }
    }
}