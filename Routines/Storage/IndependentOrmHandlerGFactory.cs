using System;

namespace DashboardCode.Routines.Storage
{
    public class IndependentOrmHandlerGFactory<TUserContext, TDataAccess> : IOrmHandlerGFactory<TUserContext>
        where TDataAccess : IDisposable
    {
        readonly IRepositoryContainer<TDataAccess> repositoryGFactory;
        readonly IOrmContainer<TDataAccess> ormGFactory;
        readonly IEntityMetaServiceContainer entityMetaServiceContainer;
        readonly Func<(TDataAccess, IAuditVisitor)> dbContextFactoryForStorage;

        public IndependentOrmHandlerGFactory(
                IRepositoryContainer<TDataAccess> repositoryGFactory,
                IOrmContainer<TDataAccess> ormGFactory,
                IEntityMetaServiceContainer entityMetaServiceContainer,
                Func<(TDataAccess, IAuditVisitor)> dbContextFactoryForStorage
            )
        {
            this.repositoryGFactory = repositoryGFactory;
            this.ormGFactory = ormGFactory;
            this.entityMetaServiceContainer = entityMetaServiceContainer;
            this.dbContextFactoryForStorage = dbContextFactoryForStorage;
        }

        public IOrmHandler<TEntity> Create<TEntity>(RoutineClosure<TUserContext> closure, bool noTracking = true) where TEntity : class
        {
            var entityStorageMetaService = entityMetaServiceContainer.Resolve<TEntity>();
            IOrmEntitySchemaAdapter ormEntitySchemaAdapter  = entityStorageMetaService.GetOrmEntitySchemaAdapter();
            Func<Exception, StorageResult> analyzeException = entityStorageMetaService.Analyze;
            Func< TDataAccess, bool, IRepository< TEntity >> createRepository = repositoryGFactory.ResolveCreateRepository<TEntity>();
            Func< TDataAccess,
                 Func<Exception, StorageResult>,
                 IAuditVisitor,
                 IOrmStorage < TEntity >
                 > createOrmStorage = ormGFactory.ResolveCreateOrmStorage<TEntity>();
            Func<TDataAccess, IOrmEntitySchemaAdapter, IOrmEntitySchemaAdapter<TEntity>> createOrmMetaAdapter = ormGFactory.ResolveCreateOrmMetaAdapter<TEntity>();

            var ormHandler = new IndependentOrmHandler<TUserContext, TDataAccess, TEntity>(closure, 
                dbContextFactoryForStorage, 
                ormEntitySchemaAdapter, 
                analyzeException,
                createRepository, 
                noTracking, 
                createOrmStorage, 
                createOrmMetaAdapter);
            return ormHandler;
        }
    }
}