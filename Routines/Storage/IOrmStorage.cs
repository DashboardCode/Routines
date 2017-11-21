using System;

namespace DashboardCode.Routines.Storage
{
    public interface IOrmStorage<TEntity>
    {
        StorageError Handle(Action<IBatch<TEntity>> action);

        StorageError HandleException(Action action);
        void HandleCommit(Action action);
        void HandleSave(Action<IBatch<TEntity>> action);
    }

    public interface IOrmStorage
    {
        StorageError Handle(Action<IBatch> action);
    }
}