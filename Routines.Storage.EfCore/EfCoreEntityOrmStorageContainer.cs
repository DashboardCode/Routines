using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace DashboardCode.Routines.Storage.EfCore
{
    public class RepositoryContainer<TDbContext> : IRepositoryContainer<TDbContext> where TDbContext : DbContext
    {
        public Func<TDbContext, bool, IRepository<TEntity>> ResolveCreateRepository<TEntity>() where TEntity : class =>
            (dContext, noTracking) => new Repository<TEntity>(dContext, noTracking);
    }

    public class OrmContainer<TDbContext> : IOrmContainer<TDbContext> where TDbContext: DbContext
    {
        public Func<TDbContext, IOrmEntitySchemaAdapter, IOrmEntitySchemaAdapter<TEntity>> ResolveCreateOrmMetaAdapter<TEntity>() where TEntity : class =>
            (dContext, ormEntitySchemaAdapter) => new OrmEntitySchemaAdapter<TEntity>((IMutableModel)dContext.Model, ormEntitySchemaAdapter);

        public Func<TDbContext, Func<Exception, StorageResult>, IAuditVisitor, IOrmStorage<TEntity>> ResolveCreateOrmStorage<TEntity>() where TEntity : class =>
            (dContext, analyzeException, auditVisitor) => new OrmStorage<TEntity>(dContext, analyzeException, auditVisitor);
    }
}