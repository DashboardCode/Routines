using System;
using System.Collections.Generic;
using System.Data.Entity;

namespace DashboardCode.Routines.Storage.Ef6
{
    public class OrmStorage<TEntity> : IOrmStorage<TEntity> where TEntity : class
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

        public StorageError Handle(Action<IBatch<TEntity>> action)
        {
            var batch = new Batch<TEntity>(context, setAuditProperties);
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