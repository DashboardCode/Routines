using System;

namespace DashboardCode.Routines.Storage
{
    public interface IRepositoryGFactory<TDbContext>
    {
        Func<TDbContext, bool, IRepository<TEntity>> ComposeCreateRepository<TEntity>() where TEntity : class;
    }

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

    public interface IRepositoryHandlerGFactory<TUserContext>
    {
        IIndependentRepositoryHandler<TUserContext, TEntity> Create<TEntity>(RoutineClosure<TUserContext> closure, bool noTracking = true) where TEntity : class;
    }

    public class IndependentRepositoryHandlerGFactory<TUserContext, TDbContext> : IRepositoryHandlerGFactory<TUserContext>
        where TDbContext : IDisposable
    {
        IRepositoryGFactory<TDbContext> repositoryGFactory;
        Func<RoutineClosure<TUserContext>, TDbContext> dbContextFactory;

        public IndependentRepositoryHandlerGFactory(
                IRepositoryGFactory<TDbContext> repositoryGFactory,
                Func<RoutineClosure<TUserContext>, TDbContext> dbContextFactory
            )
        {
            this.repositoryGFactory = repositoryGFactory;
            this.dbContextFactory = dbContextFactory;
        }

        public IIndependentRepositoryHandler<TUserContext, TEntity> Create<TEntity>(RoutineClosure<TUserContext> closure, bool noTracking) where TEntity : class
        {
            var createRepository = repositoryGFactory.ComposeCreateRepository<TEntity>();
            var repositoryHandler = new IndependentRepositoryHandler<TUserContext, TDbContext, TEntity>(closure, dbContextFactory, (dbContext)=>createRepository(dbContext, noTracking) );
            return repositoryHandler;
        }
    }
}