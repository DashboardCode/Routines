using System;

using DashboardCode.Routines.Storage;

namespace DashboardCode.AdminkaV1.DataAccessEfCore
{
    public class AdminkaRepositoryHandlerFactory : RepositoryHandlerFactory<UserContext, AdminkaDbContext>
    {
        public AdminkaRepositoryHandlerFactory(
            RepositoryDbContextHandlerContainer<UserContext, AdminkaDbContext> repositoryDbContextHandlerContainer,
            IRepositoryFactoryFactory<AdminkaDbContext> ormFactoryFactory
            ) : base(repositoryDbContextHandlerContainer, ormFactoryFactory)
        { }
    }

    public class RepositoryHandlerFactory<TUserContext, TDbContext> where TDbContext : IDisposable
    {
        readonly RepositoryDbContextHandlerContainer<TUserContext, TDbContext> repositoryDbContextHandlerContainer;
        readonly IRepositoryFactoryFactory<TDbContext> repositoryFactoryFactory;

        public RepositoryHandlerFactory(
            RepositoryDbContextHandlerContainer<TUserContext, TDbContext> repositoryDbContextHandlerContainer,
            IRepositoryFactoryFactory<TDbContext> repositoryFactoryFactory
            )
        {
            this.repositoryDbContextHandlerContainer = repositoryDbContextHandlerContainer;
            this.repositoryFactoryFactory = repositoryFactoryFactory;
        }

        public IRepositoryHandler<TEntity> Create<TEntity>(bool noTracking = true) where TEntity : class
        {
            var x = repositoryFactoryFactory.CreateRepositoryFactory<TEntity>();
            var adminkaDbContextPrimitiveHandler = repositoryDbContextHandlerContainer.Resolve();

            var repositoryHandler = new RepositoryHandler<TUserContext, TDbContext, TEntity>(
                //  RepositoryDbContextHandler<TUserContext, TDbContext> repositoryDbContextPrimitiveHandler
                adminkaDbContextPrimitiveHandler,
                noTracking,
                x.CreateRepository
            );
            return repositoryHandler;
        }
    }
}