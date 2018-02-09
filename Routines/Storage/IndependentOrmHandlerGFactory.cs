using System;

namespace DashboardCode.Routines.Storage
{
    public class IndependentOrmHandlerGFactory<TUserContext, TDataAccess> : IOrmHandlerGFactory<TUserContext>
        where TDataAccess : IDisposable
    {
        Func<RoutineClosure<TUserContext>, (TDataAccess, IAuditVisitor)> dbContextFactoryForStorage;

        IRepositoryGFactory<TDataAccess> repositoryGFactory;
        IOrmGFactory<TDataAccess> ormGFactory;
        IStorageMetaService storageMetaService;

        public IndependentOrmHandlerGFactory(
                IRepositoryGFactory<TDataAccess> repositoryGFactory,
                IOrmGFactory<TDataAccess> ormGFactory,
                IStorageMetaService storageMetaService,
                Func<RoutineClosure<TUserContext>, (TDataAccess, IAuditVisitor)> dbContextFactoryForStorage
            )
        {
            this.dbContextFactoryForStorage = dbContextFactoryForStorage;
            this.repositoryGFactory = repositoryGFactory;
            this.ormGFactory = ormGFactory;
            this.storageMetaService = storageMetaService;
        }

        public IIndependentOrmHandler<TUserContext, TEntity> Create<TEntity>(RoutineClosure<TUserContext> closure, bool noTracking = true) where TEntity : class
        {
            IOrmEntitySchemaAdapter ormEntitySchemaAdapter  = storageMetaService.GetOrmEntitySchemaAdapter<TEntity>();
            Func<Exception, StorageResult> analyzeException = storageMetaService.Analyze<TEntity> ;
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