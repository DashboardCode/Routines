using System;

namespace DashboardCode.Routines.Storage
{
    public interface IOrmContainer<TDbContext>
    {
        Func<TDbContext, Func<Exception, StorageResult>, IAuditVisitor, IOrmStorage<TEntity>> ResolveCreateOrmStorage<TEntity>() where TEntity : class;
        Func<TDbContext, IOrmEntitySchemaAdapter, IOrmEntitySchemaAdapter<TEntity>> ResolveCreateOrmMetaAdapter<TEntity>() where TEntity : class;
    }
}