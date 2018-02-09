using System;

namespace DashboardCode.Routines.Storage
{
    public interface IEntityMetaService<TEntity> where TEntity : class
    {
        StorageResult Analyze(Exception ex);
        IOrmEntitySchemaAdapter<TEntity> GetOrmEntitySchemaAdapter();
    }
}