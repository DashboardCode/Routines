using System.Data.Entity;

namespace DashboardCode.Routines.Storage.Ef6
{
    public class AdoBatch : IAdoBatch
    {
        private readonly DbContext context;

        public AdoBatch(DbContext context)
        {
            this.context = context;
        }

        public void RemoveAll<TEntity>() where TEntity : class
        {
            var tableName = this.context.GetTableName<TEntity>();
            var dml = $"DELETE FROM {tableName}";
            context.Database.ExecuteSqlCommand(dml);
        }
    }
}