using System;
using System.Threading.Tasks;

namespace DashboardCode.Routines.Storage
{
    public interface IOrmStorage<TEntity>
    {
        StorageResult Handle(Action<IBatch<TEntity>> action);
        StorageResult HandleAnalyzableException(Action action);
        void HandleCommit(Action action);
        void HandleSave(Action<IBatch<TEntity>> action);

        Task<StorageResult> HandleAsync(Action<IBatch<TEntity>> action);
        Task<StorageResult> HandleAnalyzableExceptionAsync(Func<Task> func);
        Task HandleCommitAsync(Func<Task> func);
        Task HandleSaveAsync(Action<IBatch<TEntity>> action);
    }

    public interface IOrmStorage
    {
        StorageResult Handle(Action<IBatch> action);
    }
}