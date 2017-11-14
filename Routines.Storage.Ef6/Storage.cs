using System;
using System.Collections.Generic;
using System.Data.Entity;

namespace DashboardCode.Routines.Storage.Ef6
{
    public class Storage<TEntity> : IStorage<TEntity> where TEntity : class
    {
        private readonly DbContext context;
        private readonly Func<Exception, List<FieldError>> analyzeException;
        private readonly Action<object> setAudit;

        public Storage(
            DbContext context,
            Func<Exception, List<FieldError>> analyzeException,
            Action<object> setAudit)
        {
            this.context = context;
            this.analyzeException = analyzeException;
            this.setAudit = setAudit;
        }

        public StorageError Handle(Action<IBatch<TEntity>> action)
        {
            var batch = new Batch<TEntity>(context, setAudit);
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