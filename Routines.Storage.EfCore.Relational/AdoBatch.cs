using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace DashboardCode.Routines.Storage.EfCore.Relational
{
    public class AdoBatch : IAdoBatch
    {
        private readonly DbContext context;
        private readonly Action<object> setAudit;

        public AdoBatch(DbContext context, Action<object> setAudit)
        {
            this.context = context;
            this.setAudit = setAudit;
        }

        public void RemoveAll<TEntity>() where TEntity : class
        {
            var entityType = context.Model.FindEntityType(typeof(TEntity));
            var relationalEntityTypeAnnotations = entityType.Relational();
            var name = relationalEntityTypeAnnotations.TableName;
            context.Database.ExecuteSqlCommand($"DELETE FROM {relationalEntityTypeAnnotations.TableName}");
        }
    }
}
