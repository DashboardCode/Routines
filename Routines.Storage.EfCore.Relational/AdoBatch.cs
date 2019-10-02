using Microsoft.EntityFrameworkCore;


namespace DashboardCode.Routines.Storage.EfCore.Relational
{
    public class AdoBatch : IAdoBatch
    {
        private readonly DbContext context;

        public AdoBatch(DbContext context) =>
            this.context = context;

        public void RemoveAll<TEntity>() where TEntity : class
        {
            var entityType = context.Model.FindEntityType(typeof(TEntity));
            //var relationalEntityTypeAnnotations = entityType.Relational();
            var schema = entityType.GetSchema();
            var tableName = entityType.GetTableName();

            var dml = string.IsNullOrEmpty(schema)? $"DELETE FROM {tableName}" : $"DELETE FROM {schema}.{tableName}";
            context.Database.ExecuteSqlRaw(dml);
        }
    }
}