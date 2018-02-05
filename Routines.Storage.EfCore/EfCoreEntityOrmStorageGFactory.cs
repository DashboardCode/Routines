using System;
using Microsoft.EntityFrameworkCore;


namespace DashboardCode.Routines.Storage.EfCore
{
    public class EfCoreEntityOrmStorageGFactory<TDbContext> : IRepositoryGFactory<TDbContext> , IOrmGFactory<TDbContext> where TDbContext: DbContext
    {
        public Func<TDbContext, bool, IRepository<TEntity>> ComposeCreateRepository<TEntity>() where TEntity : class =>
            (dContext, noTracking) => new Repository<TEntity>(dContext, noTracking);

        public Func<TDbContext, IOrmEntitySchemaAdapter, IOrmEntitySchemaAdapter<TEntity>> ComposeCreateOrmMetaAdapter<TEntity>() where TEntity : class =>
            (dContext, ormEntitySchemaAdapter) => new OrmMetaAdapter<TEntity>(dContext.Model, ormEntitySchemaAdapter);

        public Func<TDbContext, Func<Exception, StorageResult>, IAuditVisitor, IOrmStorage<TEntity>> ComposeCreateOrmStorage<TEntity>() where TEntity : class =>
            (dContext, analyzeException, auditVisitor) => new OrmStorage<TEntity>(dContext, analyzeException, auditVisitor);
    }
}