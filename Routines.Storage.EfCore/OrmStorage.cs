using System;
using Microsoft.EntityFrameworkCore;

namespace DashboardCode.Routines.Storage.EfCore
{
    public class OrmStorage<TEntity> : IOrmStorage<TEntity> where TEntity : class
    {
        static NoAuditVisitor noAuditVisitor = new NoAuditVisitor();
        private class NoAuditVisitor : IAuditVisitor
        {
            public bool HasAuditProperties(object o)
            {
                return false;
            }

            public void SetAuditProperties(object o)
            {
            }
        }

        private readonly DbContext dbContext;
        private readonly Func<Exception, StorageResult> analyzeException;
        private readonly IAuditVisitor auditVisitor;

        public OrmStorage(
            DbContext dbContext,
            Func<Exception, StorageResult> analyzeException,
            IAuditVisitor auditVisitor=null)
        {
            this.dbContext = dbContext;
            this.analyzeException = analyzeException;
            this.auditVisitor = auditVisitor ?? noAuditVisitor;
        }

        public StorageResult Handle(Action<IBatch<TEntity>> action)
        {
            return HandleAnalyzableException(() => {
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
            action(new Batch<TEntity>(dbContext, auditVisitor));
            dbContext.SaveChanges();
        }
    }

    public class OrmStorage : IOrmStorage
    {
        private readonly DbContext context;
        private readonly Func<Exception, StorageResult> analyzeException;
        private readonly Action<object> setAuditProperties;

        public OrmStorage(
            DbContext context,
            Func<Exception, StorageResult> analyzeException,
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
                action(batch);
                context.SaveChanges();
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
    }
}