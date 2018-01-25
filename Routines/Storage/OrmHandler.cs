using System;
using System.Threading.Tasks;

namespace DashboardCode.Routines.Storage
{
    public class ReliantOrmHandler<TEntity>  
        where TEntity : class
    {
        //readonly TDbContext dbContext;
        //readonly IOrmEntitySchemaAdapter ormEntitySchemaAdapter;
        //readonly Func<Exception, StorageResult> analyzeException;
        //readonly bool noTracking;
        //readonly IAuditVisitor auditVisitor;

        //readonly Func<TDbContext, bool, IRepository<TEntity>> createRepository;
        //readonly Func<TDbContext, 
        //         Func<Exception, StorageResult>,
        //         IAuditVisitor,
        //         IOrmStorage<TEntity>
        //         > createOrmStorage;
        //readonly Func<TDbContext, IOrmEntitySchemaAdapter, IOrmEntitySchemaAdapter<TEntity>> createOrmMetaAdapter;

        readonly IOrmStorage<TEntity> ormStorage;
        readonly IRepository<TEntity> repository;
        readonly IOrmEntitySchemaAdapter<TEntity> ormEntitySchemaAdapter;

        public ReliantOrmHandler(
            IRepository<TEntity> repository,
            IOrmStorage<TEntity> ormStorage,
            IOrmEntitySchemaAdapter<TEntity> ormEntitySchemaAdapter
            //TDbContext dbContext,
            //IAuditVisitor auditVisitor,

            //IOrmEntitySchemaAdapter ormEntitySchemaAdapter,
            //Func<Exception, StorageResult> analyzeException,

            //Func<TDbContext, bool, IRepository<TEntity>> createRepository,
            //bool noTracking,

            //Func<TDbContext,
            //     Func<Exception, StorageResult>,
            //     IAuditVisitor,
            //     IOrmStorage<TEntity>
            //     > createOrmStorage,

            //Func<TDbContext, IOrmEntitySchemaAdapter, IOrmEntitySchemaAdapter<TEntity>> createOrmMetaAdapter
            )
        {
            this.repository = repository;
            this.ormStorage = ormStorage;
            this.ormEntitySchemaAdapter = ormEntitySchemaAdapter;
            //this.dbContext = dbContext;
            //this.auditVisitor = auditVisitor;
            //this.ormEntitySchemaAdapter = ormEntitySchemaAdapter;
            //this.analyzeException = analyzeException;
            //this.noTracking = noTracking;
            //this.createRepository = createRepository;
            //this.createOrmStorage = createOrmStorage;
            //this.createOrmMetaAdapter = createOrmMetaAdapter;
        }

        public void Handle(Action<IRepository<TEntity>, IOrmStorage<TEntity>> action)
        {
            action(repository, ormStorage );
        }

        public TOutput Handle<TOutput>(Func<IRepository<TEntity>, IOrmStorage<TEntity>, TOutput> func)
        {
            return func(repository, ormStorage);
        }

        public Task<TOutput> HandleAsync<TOutput>(Func<IRepository<TEntity>, IOrmStorage<TEntity>, Task<TOutput>> func)
        {
            return func(repository, ormStorage);
        }

        public void Handle(Action<IRepository<TEntity>, IOrmStorage<TEntity>, IOrmEntitySchemaAdapter<TEntity>> action)
        {
            action(repository, ormStorage, ormEntitySchemaAdapter);
        }

        public TOutput Handle<TOutput>(Func<IRepository<TEntity>, IOrmStorage<TEntity>, IOrmEntitySchemaAdapter<TEntity>, TOutput> func)
        {
            return func(repository, ormStorage, ormEntitySchemaAdapter
                );
        }

        public Task<TOutput> HandleAsync<TOutput>(Func<IRepository<TEntity>, IOrmStorage<TEntity>, IOrmEntitySchemaAdapter<TEntity>, Task<TOutput>> func)
        {
            return func(repository, ormStorage, ormEntitySchemaAdapter);
        }
    }

    public interface IIndependentOrmHandler<TUserContext, TEntity> where TEntity : class
    {
        void Handle(Action<IRepository<TEntity>, IOrmStorage<TEntity>> action);
        TOutput Handle<TOutput>(Func<IRepository<TEntity>, IOrmStorage<TEntity>, TOutput> func);
        Task<TOutput> HandleAsync<TOutput>(Func<IRepository<TEntity>, IOrmStorage<TEntity>, Task<TOutput>> func);
        void Handle(Action<IRepository<TEntity>, IOrmStorage<TEntity>, IOrmEntitySchemaAdapter<TEntity>> action);
        TOutput Handle<TOutput>(Func<IRepository<TEntity>, IOrmStorage<TEntity>, IOrmEntitySchemaAdapter<TEntity>, TOutput> func);
        Task<TOutput> HandleAsync<TOutput>(Func<IRepository<TEntity>, IOrmStorage<TEntity>, IOrmEntitySchemaAdapter<TEntity>, Task<TOutput>> func);
    }

    public class IndependentOrmHandler<TUserContext, TDbContext, TEntity>: IIndependentOrmHandler<TUserContext, TEntity> where TDbContext : IDisposable
        where TEntity : class
    {
        readonly IOrmEntitySchemaAdapter ormEntitySchemaAdapter;
        readonly Func<Exception, StorageResult> analyzeException;
        readonly RoutineClosure<TUserContext> closure;
        readonly bool noTracking;

        readonly Func<TDbContext, bool, IRepository<TEntity>> createRepository;
        readonly Func<TDbContext,
                 Func<Exception, StorageResult>,
                 IAuditVisitor,
                 IOrmStorage<TEntity>
                 > createOrmStorage;
        readonly Func<TDbContext, IOrmEntitySchemaAdapter, IOrmEntitySchemaAdapter<TEntity>> createOrmMetaAdapter;

        readonly Func<RoutineClosure<TUserContext>, TDbContext> dbContextFactory;
        readonly IAuditVisitor auditVisitor;
        public IndependentOrmHandler(
            RoutineClosure<TUserContext> closure,
            Func<RoutineClosure<TUserContext>, TDbContext> dbContextFactory,
            IAuditVisitor auditVisitor,
            IOrmEntitySchemaAdapter ormEntitySchemaAdapter,
            Func<Exception, StorageResult> analyzeException,
            Func<TDbContext, bool, IRepository<TEntity>> createRepository,
            bool noTracking,
            Func<TDbContext,
                 Func<Exception, StorageResult>,
                 IAuditVisitor,
                 IOrmStorage<TEntity>
                 > createOrmStorage,
            Func<TDbContext, IOrmEntitySchemaAdapter, IOrmEntitySchemaAdapter<TEntity>> createOrmMetaAdapter
            )
        {
            this.closure = closure;
            this.dbContextFactory = dbContextFactory;
            this.ormEntitySchemaAdapter = ormEntitySchemaAdapter;
            this.analyzeException = analyzeException;
            this.noTracking = noTracking;
            this.createRepository = createRepository;
            this.createOrmStorage = createOrmStorage;
            this.createOrmMetaAdapter = createOrmMetaAdapter;
            this.auditVisitor = auditVisitor;
        }

        public void Handle(Action<IRepository<TEntity>, IOrmStorage<TEntity>> action)
        {
            using (var dbContext = dbContextFactory(closure))
                action(createRepository(dbContext, noTracking),
                createOrmStorage(dbContext, analyzeException, auditVisitor)
                );
        }

        public TOutput Handle<TOutput>(Func<IRepository<TEntity>, IOrmStorage<TEntity>, TOutput> func)
        {
            using (var dbContext = dbContextFactory(closure))
                return func(
                createRepository(dbContext, noTracking),
                createOrmStorage(dbContext, analyzeException, auditVisitor)
                );
        }

        public Task<TOutput> HandleAsync<TOutput>(Func<IRepository<TEntity>, IOrmStorage<TEntity>, Task<TOutput>> func)
        {
            using (var dbContext = dbContextFactory(closure))
                return func(
                createRepository(dbContext, noTracking),
                createOrmStorage(dbContext, analyzeException, auditVisitor)
                );
        }

        public void Handle(Action<IRepository<TEntity>, IOrmStorage<TEntity>, IOrmEntitySchemaAdapter<TEntity>> action)
        {
            using (var dbContext = dbContextFactory(closure))
                action(createRepository(dbContext, noTracking),
                createOrmStorage(dbContext, analyzeException, auditVisitor),
                createOrmMetaAdapter(dbContext, ormEntitySchemaAdapter)
                );
        }

        public TOutput Handle<TOutput>(Func<IRepository<TEntity>, IOrmStorage<TEntity>, IOrmEntitySchemaAdapter<TEntity>, TOutput> func)
        {
            using (var dbContext = dbContextFactory(closure))
                return func(
                createRepository(dbContext, noTracking),
                createOrmStorage(dbContext, analyzeException, auditVisitor),
                createOrmMetaAdapter(dbContext, ormEntitySchemaAdapter)
                );
        }

        public Task<TOutput> HandleAsync<TOutput>(Func<IRepository<TEntity>, IOrmStorage<TEntity>, IOrmEntitySchemaAdapter<TEntity>, Task<TOutput>> func)
        {
            using (var dbContext = dbContextFactory(closure))
                return func(
                createRepository(dbContext, noTracking),
                createOrmStorage(dbContext, analyzeException, auditVisitor),
                createOrmMetaAdapter(dbContext, ormEntitySchemaAdapter)
                );
        }
    }
}