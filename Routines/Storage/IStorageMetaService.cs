using System;

namespace DashboardCode.Routines.Storage
{
    public interface IStorageMetaService
    {
        StorageResult Analyze<TEntity>(Exception ex);
        IOrmEntitySchemaAdapter<TEntity> GetOrmEntitySchemaAdapter<TEntity>() where TEntity : class;
    }
}