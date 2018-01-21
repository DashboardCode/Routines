using System;

namespace DashboardCode.Routines.Storage
{
    public interface IRepositoryFactory<TDbContext, TEntity> where TEntity : class
    {
        IRepository<TEntity> CreateRepository(TDbContext dbContext, bool noTracking2);
    }

    public interface IOrmFactory<TDbContext, TEntity>: IRepositoryFactory<TDbContext, TEntity> where TEntity : class
    {
        IOrmStorage<TEntity> CreateOrmStorage(TDbContext dbContext, Func<Exception, StorageResult> analyzeException, Func<object, bool> hasAuditProperties, Action<object> setAuditProperties);
        IOrmEntitySchemaAdapter<TEntity> CreateOrmMetaAdapter(TDbContext dbContext, IOrmEntitySchemaAdapter ormEntitySchemaAdapter);
    }

    public interface IOrmFactoryFactory<TDbContext> where TDbContext : IDisposable
    {
        IOrmFactory<TDbContext, TEntity> CreateOrmFactory<TEntity>() where TEntity : class;
    }

    public interface IRepositoryFactoryFactory<TDbContext> where TDbContext : IDisposable
    {
        IRepositoryFactory<TDbContext, TEntity> CreateRepositoryFactory<TEntity>() where TEntity : class;
    }
}
