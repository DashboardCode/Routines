using System;

namespace DashboardCode.Routines.Storage
{
    public class ReliantRepositoryHandlerGFactory<TUserContext, TDbContext>
        where TDbContext : IDisposable
    {
        TDbContext dbContext;
        IRepositoryGFactory<TDbContext> repositoryGFactory;

        public ReliantRepositoryHandlerGFactory(
                IRepositoryGFactory<TDbContext> repositoryGFactory,
                TDbContext dbContext
            )
        {
            this.dbContext = dbContext;
            this.repositoryGFactory = repositoryGFactory;
        }

        public ReliantRepositoryHandler<TEntity> Create<TEntity>(bool noTracking = true) where TEntity : class
        {
            Func<TDbContext, bool, IRepository<TEntity>> createRepository = repositoryGFactory.ComposeCreateRepository<TEntity>();
            var repository = createRepository(dbContext, noTracking);
            var repositoryHandler = new ReliantRepositoryHandler<TEntity>(repository);
            return repositoryHandler;
        }
    }
}