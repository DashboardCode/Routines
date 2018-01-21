using Microsoft.EntityFrameworkCore.Metadata;

namespace DashboardCode.Routines.Storage.EfCore
{
    public class OrmMetaAdapter<TEntity> : IOrmEntitySchemaAdapter<TEntity> where TEntity : class
    {
        private readonly IModel model;
        private readonly IOrmEntitySchemaAdapter ormEntitySchemaAdapter;

        public OrmMetaAdapter(IModel model, IOrmEntitySchemaAdapter ormEntitySchemaAdapter)
        {
            this.model = model;
            this.ormEntitySchemaAdapter = ormEntitySchemaAdapter;
        }

        public Include<TEntity> AppendModelFields(Include<TEntity> include) =>
            EfCoreExtensions.AppendModelFields(include, model);

        public Include<TEntity> AppendModelFieldsIfEmpty(Include<TEntity> include) =>
            EfCoreExtensions.AppendModelFieldsIfEmpty(include, model);

        public Include<TEntity> ExtractNavigations(Include<TEntity> include) =>
            EfCoreExtensions.ExtractNavigations(include, model);

        public Include<TEntity> ExtractNavigationsAppendKeyLeafs(Include<TEntity> include) =>
            EfCoreExtensions.ExtractNavigationsAppendKeyLeafs(include, model);

        public string[] GetBinaries()
        {
            return ormEntitySchemaAdapter.GetBinaries();
        }

        public (string[] Attributes, string Message) GetConstraint(string name)
        {
            return ormEntitySchemaAdapter.GetConstraint(name);
        }

        public string[] GetKeys()
        {
            return ormEntitySchemaAdapter.GetKeys();
        }

        public (string SchemaName, string TableName) GetTableName()
        {
            return ormEntitySchemaAdapter.GetTableName();
        }

        public string[] GetUnique(string name)
        {
            return ormEntitySchemaAdapter.GetUnique(name);
        }

        public string[] GetRequireds()
        {
            return ormEntitySchemaAdapter.GetRequireds();
        }
    }
}