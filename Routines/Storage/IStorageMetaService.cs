using System;

namespace DashboardCode.Routines.Storage
{
    public interface IStorageMetaService
    {
        StorageResult Analyze<TEntity>(Exception ex);
        IOrmEntitySchemaAdapter<TEntity> GetOrmEntitySchemaAdapter<TEntity>() where TEntity : class;
    }

    //public interface StorageMetaServiceGFactory
    //{
    //    IStorageMetaService<TEntity> GetStorageMetaService<TEntity>() where TEntity : class;
    //}

    //public interface IStorageMetaService<TEntity> where TEntity : class
    //{
    //    StorageResult Analyze(Exception ex);
    //    IOrmEntitySchemaAdapter<TEntity> GetOrmEntitySchemaAdapter(); 
    //}
}
