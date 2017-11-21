using System;
using System.Collections.Generic;
using System.Data.Entity;

namespace DashboardCode.Routines.Storage.Ef6
{
    public class OrmStorage<TEntity> : IOrmStorage<TEntity> where TEntity : class
    {
        private readonly DbContext dbContext;
        private readonly Func<Exception, List<FieldError>> analyzeException;
        private readonly Action<object> setAuditProperties;

        public OrmStorage(
            DbContext dbContext,
            Func<Exception, List<FieldError>> analyzeException,
            Action<object> setAuditProperties)
        {
            this.dbContext = dbContext;
            this.analyzeException = analyzeException;
            this.setAuditProperties = setAuditProperties;
        }

        public StorageError Handle(Action<IBatch<TEntity>> action)
        {
            var batch = new Batch<TEntity>(dbContext, setAuditProperties);
            try
            {
                using (var transaction = dbContext.Database.BeginTransaction())
                {
                    action(batch);
                    dbContext.SaveChanges();
                    transaction.Commit();
                }
            }
            catch (Exception exception)
            {
                var list = analyzeException(exception);
                if (list.Count > 0)
                    return new StorageError(exception, list);
                throw;
            }
            return null;
        }

        public StorageError HandleException(Action action)
        {
            try
            {
                action();
                dbContext.SaveChanges();
            }
            catch (Exception exception)
            {
                var list = analyzeException(exception);
                if (list.Count > 0)
                    return new StorageError(exception, list);
                throw;
            }
            return null;
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
        private readonly Func<Exception, List<FieldError>> analyzeException;
        private readonly Action<object> setAuditProperties;

        public OrmStorage(
            DbContext context,
            Func<Exception, List<FieldError>> analyzeException,
            Action<object> setAuditProperties)
        {
            this.context = context;
            this.analyzeException = analyzeException;
            this.setAuditProperties = setAuditProperties;
        }

        public StorageError Handle(Action<IBatch> action)
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
                    return new StorageError(exception, list);
                throw;
            }
            return null;
        }
    }
}