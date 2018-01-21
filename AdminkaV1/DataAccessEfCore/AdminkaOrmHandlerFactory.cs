using System;

using DashboardCode.Routines.Storage;
using DashboardCode.AdminkaV1.DataAccessEfCore.Services;

namespace DashboardCode.AdminkaV1.DataAccessEfCore
{
    public class AdminkaOrmHandlerFactory: OrmHandlerFactory<UserContext, AdminkaDbContext>
    {
        public AdminkaOrmHandlerFactory(
            OrmDbContextHandlerContainer<UserContext, AdminkaDbContext> ormDbContextHandlerContainer,
            StorageMetaService storageMetaService,
            IOrmFactoryFactory<AdminkaDbContext> ormFactoryFactory
            ):base(ormDbContextHandlerContainer,  storageMetaService, ormFactoryFactory)
        { }
    }

    public class OrmHandlerFactory<TUserContext, TDbContext> where TDbContext : IDisposable
    {
        readonly OrmDbContextHandlerContainer<TUserContext, TDbContext> ormDbContextHandlerContainer;
        readonly StorageMetaService storageMetaService;
        readonly IOrmFactoryFactory<TDbContext> ormFactoryFactory;

        public OrmHandlerFactory(
            OrmDbContextHandlerContainer<TUserContext, TDbContext> ormDbContextHandlerContainer,
            StorageMetaService storageMetaService,
            IOrmFactoryFactory<TDbContext> ormFactoryFactory
            )
        {
            this.ormDbContextHandlerContainer = ormDbContextHandlerContainer;
            this.storageMetaService = storageMetaService;
            this.ormFactoryFactory = ormFactoryFactory;
        }

        public IOrmHandler<TEntity> Create<TEntity>(bool noTracking = true) where TEntity : class
        {
            var x = ormFactoryFactory.CreateOrmFactory<TEntity>();
            var ormDbContextHandler = ormDbContextHandlerContainer.Resolve();
            var ormHandler = new OrmHandler<TUserContext, TDbContext, TEntity>(
                ormDbContextHandler,
                storageMetaService.GetOrmMetaAdapter<TEntity>(),
                ex =>storageMetaService.Analyze<TEntity>(ex),
                noTracking,
                x.CreateRepository,
                x.CreateOrmStorage,
                x.CreateOrmMetaAdapter
            );
            return ormHandler;
        }
    }
}