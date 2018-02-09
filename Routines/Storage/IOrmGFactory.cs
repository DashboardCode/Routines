using System;

namespace DashboardCode.Routines.Storage
{
    public interface IOrmGFactory<TDbContext>
    {
        Func<TDbContext, Func<Exception, StorageResult>, IAuditVisitor, IOrmStorage<TEntity>> ComposeCreateOrmStorage<TEntity>() where TEntity : class;
        Func<TDbContext, IOrmEntitySchemaAdapter, IOrmEntitySchemaAdapter<TEntity>> ComposeCreateOrmMetaAdapter<TEntity>() where TEntity : class;
    }
}
