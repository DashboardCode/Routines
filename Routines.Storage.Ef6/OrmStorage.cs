using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Threading.Tasks;

namespace DashboardCode.Routines.Storage.Ef6
{
    public class OrmStorage<TEntity> : IOrmStorage<TEntity> where TEntity : class
    {
        private readonly DbContext dbContext;
        private readonly Func<Exception, StorageResult> analyzeException;
        private readonly IAuditVisitor auditVisitor;

        public OrmStorage(
            DbContext dbContext,
            Func<Exception, StorageResult> analyzeException,
            IAuditVisitor auditVisitor)
        {
            this.dbContext          = dbContext;
            this.analyzeException   = analyzeException;
            this.auditVisitor = auditVisitor;
        }

        public StorageResult Handle(Action<IBatch<TEntity>> action)
        {
            return HandleAnalyzableException(()=> {
                HandleSave((batch) => {
                    action(batch);
                });
            });
        }

        //public StorageResult HandleAsync(Action<IBatch<TEntity>> action)
        //{
        //    return HandleAnalyzableException(() => {
        //        HandleSaveAsync((batch) => {
        //            action(batch);
        //        });
        //    });
        //}

        public StorageResult HandleAnalyzableException(Action action)
        {
            try
            {
                action();
            }
            catch (Exception exception)
            {
                var storageResult = analyzeException(exception);
                if (!storageResult.IsOk())
                    return storageResult;
                throw;
            }
            return new StorageResult();
        }

        public void HandleCommit(Action action)
        {
            using var transaction = dbContext.Database.BeginTransaction();
            action();
            transaction.Commit();
        }

        public void HandleSave(Action<IBatch<TEntity>> action)
        {
            action(new Batch<TEntity>(dbContext, auditVisitor));
            dbContext.SaveChanges();
        }

        public async Task HandleSaveAsync(Func<IBatch<TEntity>, Task> action)
        {
            await action(new Batch<TEntity>(dbContext, auditVisitor));
            await dbContext.SaveChangesAsync();
        }

        Task<StorageResult> IOrmStorage<TEntity>.HandleAsync(Func<IBatch<TEntity>,Task> action)
        {
            throw new NotImplementedException();
        }

        public Task<StorageResult> HandleAnalyzableExceptionAsync(Func<Task> func)
        {
            throw new NotImplementedException();
        }

        public Task HandleCommitAsync(Func<Task> func)
        {
            throw new NotImplementedException();
        }

        Task IOrmStorage<TEntity>.HandleSaveAsync(Func<IBatch<TEntity>,Task> action)
        {
            throw new NotImplementedException();
        }

        public StorageResult HandleAsync(Func<IBatch<TEntity>, Task> action)
        {
            return HandleAnalyzableException(async () =>
            {
                await HandleSaveAsync(async (batch) =>
                {
                    await action(batch);
                });
            });
        }
    }

    public class OrmStorage : IOrmStorage
    {
        private readonly DbContext context;
        private readonly Func<Exception, FormMessages> analyzeException;
        private readonly IAuditVisitor auditVisitor = null;

        public OrmStorage(
            DbContext context,
            Func<Exception, FormMessages> analyzeException,
            IAuditVisitor auditVisitor = null)
        {
            this.context = context;
            this.analyzeException = analyzeException;
            this.auditVisitor = auditVisitor ?? NoAuditVisitor.Singleton;
        }

        public StorageResult Handle(Action<IBatch> action)
        {
            var batch = new Batch(context, auditVisitor);
            try
            {
                using var transaction = context.Database.BeginTransaction();
                action(batch);
                context.SaveChanges();
                transaction.Commit();
            }
            catch (Exception exception)
            {
                var formMessages = analyzeException(exception);
                if (formMessages != null && formMessages.DirectMessages.Count > 0 && formMessages.EntityValidationMessages.Count > 0)
                    return new StorageResult(exception, formMessages);
                throw;
            }
            return new StorageResult();
        }
    }
}