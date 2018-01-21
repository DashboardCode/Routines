using System;
using System.Threading.Tasks;

namespace DashboardCode.Routines.Storage
{
    public class OrmHandler<TUserContext, TDbContext, TEntity> : IOrmHandler<TEntity> where TDbContext: IDisposable
        where TEntity : class
    {
        readonly OrmDbContextHandler<TUserContext, TDbContext> adminkaOrmDbContextHandler;
        readonly IOrmEntitySchemaAdapter ormEntitySchemaAdapter;
        readonly Func<Exception, StorageResult> analyzeException;
        readonly bool noTracking;

        readonly Func<TDbContext, bool, IRepository<TEntity>> createRepository;
        readonly Func<TDbContext, 
                 Func<Exception, StorageResult>, 
                 Func<object, bool>,
                 Action<object>,
                 IOrmStorage<TEntity>
                 > createOrmStorage;
        readonly Func<TDbContext, IOrmEntitySchemaAdapter, IOrmEntitySchemaAdapter<TEntity>> createOrmMetaAdapter;

        public OrmHandler(
            OrmDbContextHandler<TUserContext, TDbContext> adminkaOrmDbContextHandler,
            IOrmEntitySchemaAdapter ormEntitySchemaAdapter,
            Func<Exception, StorageResult> analyzeException,
            bool noTracking,

            Func<TDbContext, bool, IRepository<TEntity>> createRepository,
            Func<TDbContext,
                 Func<Exception, StorageResult>,
                 Func<object, bool>,
                 Action<object>,
                 IOrmStorage<TEntity>
                 > createOrmStorage,
            Func<TDbContext, IOrmEntitySchemaAdapter, IOrmEntitySchemaAdapter<TEntity>> createOrmMetaAdapter
            )
        {
            this.adminkaOrmDbContextHandler = adminkaOrmDbContextHandler;
            this.ormEntitySchemaAdapter = ormEntitySchemaAdapter;
            this.analyzeException = analyzeException;
            this.noTracking = noTracking;
            this.createRepository = createRepository;
            this.createOrmStorage = createOrmStorage;
            this.createOrmMetaAdapter = createOrmMetaAdapter;
        }

        public void Handle(Action<IRepository<TEntity>, IOrmStorage<TEntity>> action)
        {
            adminkaOrmDbContextHandler.Handle((dbContext, closure, hasAuditProperties, setAuditProperties) => action(
                createRepository(dbContext, noTracking),
                createOrmStorage(dbContext, analyzeException, hasAuditProperties, setAuditProperties)
                ));
        }

        public TOutput Handle<TOutput>(Func<IRepository<TEntity>, IOrmStorage<TEntity>, TOutput> func)
        {
            return adminkaOrmDbContextHandler.Handle((dbContext, closure, hasAuditProperties,setAuditProperties) => func(
                createRepository(dbContext, noTracking),
                createOrmStorage(dbContext, analyzeException, hasAuditProperties, setAuditProperties)
                ));
        }

        public Task<TOutput> HandleAsync<TOutput>(Func<IRepository<TEntity>, IOrmStorage<TEntity>, TOutput> func)
        {
            return Task.Run(() =>
            {
                var output = default(TOutput);
                adminkaOrmDbContextHandler.Handle((dbContext, closure, hasAuditProperties, setAuditProperties) =>
                {
                    output = func(
                        createRepository(dbContext, noTracking),
                        createOrmStorage(dbContext,  analyzeException, hasAuditProperties, setAuditProperties)
                        );
                });
                return output;
            });
        }

        public void Handle(Action<IRepository<TEntity>, IOrmStorage<TEntity>, IOrmEntitySchemaAdapter<TEntity>> action)
        {
            adminkaOrmDbContextHandler.Handle((dbContext, closure, hasAuditProperties, setAuditProperties) => action(
               createRepository(dbContext, noTracking),
               createOrmStorage(dbContext, analyzeException, hasAuditProperties, setAuditProperties),
               createOrmMetaAdapter(dbContext, ormEntitySchemaAdapter)
               ));
        }

        public TOutput Handle<TOutput>(Func<IRepository<TEntity>, IOrmStorage<TEntity>, IOrmEntitySchemaAdapter<TEntity>, TOutput> func)
        {
            return adminkaOrmDbContextHandler.Handle((dbContext, closure, hasAuditProperties, setAuditProperties) => func(
                createRepository(dbContext, noTracking),
                createOrmStorage(dbContext, analyzeException, hasAuditProperties, setAuditProperties),
                createOrmMetaAdapter(dbContext, ormEntitySchemaAdapter)
                ));
        }

        public Task<TOutput> HandleAsync<TOutput>(Func<IRepository<TEntity>, IOrmStorage<TEntity>, IOrmEntitySchemaAdapter<TEntity>, TOutput> func)
        {
            return Task.Run(() =>
            {
                var output = default(TOutput);
                adminkaOrmDbContextHandler.Handle((dbContext, closure, hasAuditProperties, setAuditProperties) =>
                {
                    output = func(
                        createRepository(dbContext, noTracking),
                        createOrmStorage(dbContext, analyzeException, hasAuditProperties, setAuditProperties),
                        createOrmMetaAdapter(dbContext, ormEntitySchemaAdapter)
                        );
                });
                return output;
            });
        }
    }
}