using System;
using System.Collections.Generic;
using System.Data.Entity;

namespace DashboardCode.Routines.Storage.Ef6
{
    public class OrmStorage<TEntity> : IOrmStorage<TEntity> where TEntity : class
    {
        private readonly DbContext dbContext;
        private readonly Func<Exception, StorageResult> analyzeException;
        private readonly Action<object> setAuditProperties;

        public OrmStorage(
            DbContext dbContext,
            Func<Exception, StorageResult> analyzeException,
            Action<object> setAuditProperties)
        {
            this.dbContext          = dbContext;
            this.analyzeException   = analyzeException;
            this.setAuditProperties = setAuditProperties;
        }

        public StorageResult Handle(Action<IBatch<TEntity>> action)
        {
            return HandleAnalyzableException(()=> {
                HandleSave((batch) => {
                    action(batch);
                });
            });
        }

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
            using (var transaction = dbContext.Database.BeginTransaction())
            {
                action();
                transaction.Commit();
            }
        }

        public void HandleSave(Action<IBatch<TEntity>> action)
        {
            action(new Batch<TEntity>(dbContext, setAuditProperties));
            dbContext.SaveChanges();
        }
    }

    public class OrmStorage : IOrmStorage
    {
        private readonly DbContext context;
        private readonly Func<Exception, List<FieldMessage>> analyzeException;
        private readonly Action<object> setAuditProperties;

        public OrmStorage(
            DbContext context,
            Func<Exception, List<FieldMessage>> analyzeException,
            Action<object> setAuditProperties)
        {
            this.context = context;
            this.analyzeException = analyzeException;
            this.setAuditProperties = setAuditProperties;
        }

        public StorageResult Handle(Action<IBatch> action)
        {
            var batch = new Batch(context, setAuditProperties);
            try
            {
                using (var transaction = context.Database.BeginTransaction())
                {
                    action(batch);
                    context.SaveChanges();
                    transaction.Commit();
                }
            }
            catch (Exception exception)
            {
                var list = analyzeException(exception);
                if (list.Count > 0)
                    return new StorageResult(exception, list);
                throw;
            }
            return new StorageResult();
        }
    }
}