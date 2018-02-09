using System;

namespace DashboardCode.Routines.Storage
{
    public class IndependentRepositoryHandlerGFactory<TUserContext, TDbContext> : IRepositoryHandlerGFactory<TUserContext>
        where TDbContext : IDisposable
    {
        IRepositoryContainer<TDbContext> repositoryGFactory;
        Func<RoutineClosure<TUserContext>, TDbContext> dbContextFactory;

        public IndependentRepositoryHandlerGFactory(
                IRepositoryContainer<TDbContext> repositoryGFactory,
                Func<RoutineClosure<TUserContext>, TDbContext> dbContextFactory
            )
        {
            this.repositoryGFactory = repositoryGFactory;
            this.dbContextFactory = dbContextFactory;
        }

        public IIndependentRepositoryHandler<TUserContext, TEntity> Create<TEntity>(RoutineClosure<TUserContext> closure, bool noTracking) where TEntity : class
        {
            var createRepository = repositoryGFactory.ResolveCreateRepository<TEntity>();
            var repositoryHandler = new IndependentRepositoryHandler<TUserContext, TDbContext, TEntity>(closure, dbContextFactory, (dbContext)=>createRepository(dbContext, noTracking) );
            return repositoryHandler;
        }
    }
}