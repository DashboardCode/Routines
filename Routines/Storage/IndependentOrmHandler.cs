﻿using System;
using System.Threading.Tasks;

namespace DashboardCode.Routines.Storage
{
    public class IndependentOrmHandler<TUserContext, TDataAccess, TEntity> : IOrmHandler<TEntity> where TDataAccess : IDisposable
        where TEntity : class
    {
        readonly IOrmEntitySchemaAdapter ormEntitySchemaAdapter;
        readonly Func<Exception, StorageResult> analyzeException;
        readonly RoutineClosure<TUserContext> closure;
        readonly bool noTracking;

        readonly Func<TDataAccess, bool, IRepository<TEntity>> createRepository;
        readonly Func<TDataAccess,
                 Func<Exception, StorageResult>,
                 IAuditVisitor,
                 IOrmStorage<TEntity>
                 > createOrmStorage;
        readonly Func<TDataAccess, IOrmEntitySchemaAdapter, IOrmEntitySchemaAdapter<TEntity>> createOrmMetaAdapter;
        readonly Func<(TDataAccess, IAuditVisitor)> dbContextFactoryForStorage;

        public IndependentOrmHandler(
            RoutineClosure<TUserContext> closure,
            Func<(TDataAccess, IAuditVisitor)> dbContextFactoryForStorage,
            IOrmEntitySchemaAdapter ormEntitySchemaAdapter,
            Func<Exception, StorageResult> analyzeException,
            Func<TDataAccess, bool, IRepository<TEntity>> createRepository,
            bool noTracking,
            Func<TDataAccess,
                 Func<Exception, StorageResult>,
                 IAuditVisitor,
                 IOrmStorage<TEntity>
                 > createOrmStorage,
            Func<TDataAccess, IOrmEntitySchemaAdapter, IOrmEntitySchemaAdapter<TEntity>> createOrmMetaAdapter
            )
        {
            this.closure = closure;
            this.ormEntitySchemaAdapter = ormEntitySchemaAdapter;
            this.analyzeException = analyzeException;
            this.noTracking = noTracking;
            this.createRepository = createRepository;
            this.createOrmStorage = createOrmStorage;
            this.createOrmMetaAdapter = createOrmMetaAdapter;
            this.dbContextFactoryForStorage = dbContextFactoryForStorage;
        }

        public void Handle(Action<IRepository<TEntity>> action)
        {
            var (dbContext, auditVisitor) = dbContextFactoryForStorage();
            using (dbContext)
                action(createRepository(dbContext, noTracking)
                );
        }

        public TOutput Handle<TOutput>(Func<IRepository<TEntity>, TOutput> func)
        {
            var (dbContext, auditVisitor) = dbContextFactoryForStorage();
            using (dbContext)
                return func(
                    createRepository(dbContext, noTracking)
                );
        }

        public async Task<TOutput> HandleAsync<TOutput>(Func<IRepository<TEntity>, Task<TOutput>> func)
        {
            var (dbContext, auditVisitor) = dbContextFactoryForStorage();
            using (dbContext)
                return await func(
                    createRepository(dbContext, noTracking)
                );
        }

        public async Task HandleAsync(Func<IRepository<TEntity>, Task> func)
        {
            var (dbContext, auditVisitor) = dbContextFactoryForStorage();
            using (dbContext)
                await func(
                    createRepository(dbContext, noTracking)
                );
        }

        public void Handle(Action<IRepository<TEntity>, IOrmStorage<TEntity>> action)
        {
            var (dbContext, auditVisitor) = dbContextFactoryForStorage();
            using (dbContext)
                action(createRepository(dbContext, noTracking),
                    createOrmStorage(dbContext, analyzeException, auditVisitor)
                );
        }

        public TOutput Handle<TOutput>(Func<IRepository<TEntity>, IOrmStorage<TEntity>, TOutput> func)
        {
            var (dbContext, auditVisitor) = dbContextFactoryForStorage();
            using (dbContext)
                return func(
                    createRepository(dbContext, noTracking),
                    createOrmStorage(dbContext, analyzeException, auditVisitor)
                );
        }

        public async Task<TOutput> HandleAsync<TOutput>(Func<IRepository<TEntity>, IOrmStorage<TEntity>, Task<TOutput>> func)
        {
            var (dbContext, auditVisitor) = dbContextFactoryForStorage();
            using (dbContext)
                return await func(
                    createRepository(dbContext, noTracking),
                    createOrmStorage(dbContext, analyzeException, auditVisitor)
                );
        }

        public async Task HandleAsync(Func<IRepository<TEntity>, IOrmStorage<TEntity>, Task> func)
        {
            var (dbContext, auditVisitor) = dbContextFactoryForStorage();
            using (dbContext)
                await func(
                    createRepository(dbContext, noTracking),
                    createOrmStorage(dbContext, analyzeException, auditVisitor)
                );
        }

        public void Handle(Action<IRepository<TEntity>, IOrmStorage<TEntity>, IOrmEntitySchemaAdapter<TEntity>> action)
        {
            var (dbContext, auditVisitor) = dbContextFactoryForStorage();
            using (dbContext)
                action(createRepository(dbContext, noTracking),
                    createOrmStorage(dbContext, analyzeException, auditVisitor),
                    createOrmMetaAdapter(dbContext, ormEntitySchemaAdapter)
                );
        }

        public TOutput Handle<TOutput>(Func<IRepository<TEntity>, IOrmStorage<TEntity>, IOrmEntitySchemaAdapter<TEntity>, TOutput> func)
        {
            var (dbContext, auditVisitor) = dbContextFactoryForStorage();
            using (dbContext)
                return func(
                    createRepository(dbContext, noTracking),
                    createOrmStorage(dbContext, analyzeException, auditVisitor),
                    createOrmMetaAdapter(dbContext, ormEntitySchemaAdapter)
                );
        }

        public async Task<TOutput> HandleAsync<TOutput>(Func<IRepository<TEntity>, IOrmStorage<TEntity>, IOrmEntitySchemaAdapter<TEntity>, Task<TOutput>> func)
        {
            var (dbContext, auditVisitor) = dbContextFactoryForStorage();
            using (dbContext)
                return await func(
                    createRepository(dbContext, noTracking),
                    createOrmStorage(dbContext, analyzeException, auditVisitor),
                    createOrmMetaAdapter(dbContext, ormEntitySchemaAdapter)
                );
        }

       

        public async Task HandleAsync(Func<IRepository<TEntity>, IOrmStorage<TEntity>, IOrmEntitySchemaAdapter<TEntity>, Task> func)
        {
            var (dbContext, auditVisitor) = dbContextFactoryForStorage();
            using (dbContext)
                await func(
                    createRepository(dbContext, noTracking),
                    createOrmStorage(dbContext, analyzeException, auditVisitor),
                    createOrmMetaAdapter(dbContext, ormEntitySchemaAdapter)
                );
        }
    }
}