using System;

namespace DashboardCode.Routines.Storage
{
    public interface IOrmStorage<TEntity>
    {
        StorageError Handle(Action<IBatch<TEntity>> action);
    }

    public interface IOrmStorage
    {
        StorageError Handle(Action<IBatch> action);
    }
}