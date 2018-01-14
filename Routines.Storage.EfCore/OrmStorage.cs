using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace DashboardCode.Routines.Storage.EfCore
{
    public class OrmStorage<TEntity> : IOrmStorage<TEntity> where TEntity : class
    {
        private readonly DbContext dbContext;
        private readonly Func<Exception, List<FieldMessage>> analyzeException;
        private readonly Action<object> setAuditProperties;
        private readonly Func<object, bool> isAuditable;

        public OrmStorage(
            DbContext dbContext,
            Func<Exception, List<FieldMessage>> analyzeException,
            Func<object, bool> isAuditable,
            Action<object> setAuditProperties)
        {
            this.dbContext = dbContext;
            this.analyzeException = analyzeException;
            this.setAuditProperties = setAuditProperties;
            this.isAuditable = isAuditable;
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
                var list = analyzeException(exception);
                if (list.Count > 0)
                    return new StorageResult(exception, list);
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
            action(new Batch<TEntity>(dbContext, isAuditable, setAuditProperties));
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
                action(batch);
                context.SaveChanges();
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