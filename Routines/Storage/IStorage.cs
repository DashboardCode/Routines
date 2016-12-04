using System;

namespace Vse.Routines.Storage
{
    public interface IStorage<TEntity>
    {
        StorageError Handle(Action<IBatch<TEntity>> action);
    }
}
