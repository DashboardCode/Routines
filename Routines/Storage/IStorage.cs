using System;

namespace DashboardCode.Routines.Storage
{
    public interface IStorage<TEntity>
    {
        StorageError Handle(Action<IBatch<TEntity>> action);
    }
}
