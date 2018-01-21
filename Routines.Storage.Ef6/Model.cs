using System.Data.Entity;

namespace DashboardCode.Routines.Storage.Ef6
{
    public class Model<TEntity> :  IOrmEntitySchemaAdapter<TEntity> where TEntity : class
    {
        private readonly DbContext context;

        public Model(DbContext context) =>
            this.context = context;

        public Include<TEntity> AppendModelFields(Include<TEntity> include) =>
            Ef6Extensions.AppendModelProperties(include, context);

        public Include<TEntity> AppendModelFieldsIfEmpty(Include<TEntity> include) =>
            Ef6Extensions.AppendModelPropertiesIfEmpty(include, context);

        public Include<TEntity> ExtractNavigations(Include<TEntity> include) =>
            Ef6Extensions.ExtractNavigations(include, context);

        public Include<TEntity> ExtractNavigationsAppendKeyLeafs(Include<TEntity> include) =>
            Ef6Extensions.ExtractNavigationsAppendKeyProperties(include, context);

        public string[] Binaries()
        {
            throw new System.NotImplementedException();
        }

        public string[] GetKeys()
        {
            throw new System.NotImplementedException();
        }

        public string[] Requireds()
        {
            throw new System.NotImplementedException();
        }

        (string SchemaName, string TableName) IOrmEntitySchemaAdapter.GetTableName()
        {
            throw new System.NotImplementedException();
        }

        public string[] GetRequireds()
        {
            throw new System.NotImplementedException();
        }

        public string[] GetBinaries()
        {
            throw new System.NotImplementedException();
        }

        public string[] GetUnique()
        {
            throw new System.NotImplementedException();
        }

        public (string[] Attributes, string Message) GetConstraint()
        {
            throw new System.NotImplementedException();
        }

        Include<TEntity> IOrmEntitySchemaAdapter<TEntity>.AppendModelFields(Include<TEntity> include)
        {
            throw new System.NotImplementedException();
        }

        Include<TEntity> IOrmEntitySchemaAdapter<TEntity>.AppendModelFieldsIfEmpty(Include<TEntity> include)
        {
            throw new System.NotImplementedException();
        }

        Include<TEntity> IOrmEntitySchemaAdapter<TEntity>.ExtractNavigations(Include<TEntity> include)
        {
            throw new System.NotImplementedException();
        }

        Include<TEntity> IOrmEntitySchemaAdapter<TEntity>.ExtractNavigationsAppendKeyLeafs(Include<TEntity> include)
        {
            throw new System.NotImplementedException();
        }

        string[] IOrmEntitySchemaAdapter.GetKeys()
        {
            throw new System.NotImplementedException();
        }

        string[] IOrmEntitySchemaAdapter.GetRequireds()
        {
            throw new System.NotImplementedException();
        }

        string[] IOrmEntitySchemaAdapter.GetBinaries()
        {
            throw new System.NotImplementedException();
        }

        string[] IOrmEntitySchemaAdapter.GetUnique(string name)
        {
            throw new System.NotImplementedException();
        }

        (string[] Attributes, string Message) IOrmEntitySchemaAdapter.GetConstraint(string name)
        {
            throw new System.NotImplementedException();
        }
    }
}
