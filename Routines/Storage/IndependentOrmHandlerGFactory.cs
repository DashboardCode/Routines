using System;

namespace DashboardCode.Routines.Storage
{
    public class IndependentOrmHandlerGFactory<TUserContext, TDataAccess> : IOrmHandlerGFactory<TUserContext>
        where TDataAccess : IDisposable
    {
        IRepositoryGFactory<TDataAccess> repositoryGFactory;
        IOrmGFactory<TDataAccess> ormGFactory;
        IEntityMetaServiceContainer entityMetaServiceContainer;
        Func<RoutineClosure<TUserContext>, (TDataAccess, IAuditVisitor)> dbContextFactoryForStorage;

        public IndependentOrmHandlerGFactory(
                IRepositoryGFactory<TDataAccess> repositoryGFactory,
                IOrmGFactory<TDataAccess> ormGFactory,
                IEntityMetaServiceContainer entityMetaServiceContainer,
                Func<RoutineClosure<TUserContext>, (TDataAccess, IAuditVisitor)> dbContextFactoryForStorage
            )
        {
            this.repositoryGFactory = repositoryGFactory;
            this.ormGFactory = ormGFactory;
            this.entityMetaServiceContainer = entityMetaServiceContainer;
            this.dbContextFactoryForStorage = dbContextFactoryForStorage;
        }

        public IIndependentOrmHandler<TUserContext, TEntity> Create<TEntity>(RoutineClosure<TUserContext> closure, bool noTracking = true) where TEntity : class
        {
            var entityStorageMetaService = entityMetaServiceContainer.Resolve<TEntity>();
            IOrmEntitySchemaAdapter ormEntitySchemaAdapter  = entityStorageMetaService.GetOrmEntitySchemaAdapter();
            Func<Exception, StorageResult> analyzeException = entityStorageMetaService.Analyze;
            Func< TDataAccess, bool, IRepository< TEntity >> createRepository = repositoryGFactory.ComposeCreateRepository<TEntity>();
            Func< TDataAccess,
                 Func<Exception, StorageResult>,
                 IAuditVisitor,
                 IOrmStorage < TEntity >
                 > createOrmStorage = ormGFactory.ComposeCreateOrmStorage<TEntity>();
            Func<TDataAccess, IOrmEntitySchemaAdapter, IOrmEntitySchemaAdapter<TEntity>> createOrmMetaAdapter = ormGFactory.ComposeCreateOrmMetaAdapter<TEntity>();

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