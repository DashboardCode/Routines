namespace DashboardCode.Routines.Storage
{
    public interface IOrmEntitySchemaAdapter
    {
        (string SchemaName, string TableName) GetTableName();
        string[] GetKeys();
        string[] GetRequireds();
        string[] GetBinaries();
        string[] GetUnique(string name);
        (string[] Attributes, string Message) GetConstraint(string name);
    }

    public interface IOrmEntitySchemaAdapter<TEntity> : IOrmEntitySchemaAdapter where TEntity : class
    {
        Include<TEntity> AppendModelFields(Include<TEntity> include);
        Include<TEntity> AppendModelFieldsIfEmpty(Include<TEntity> include);
        Include<TEntity> ExtractNavigations(Include<TEntity> include);
        Include<TEntity> ExtractNavigationsAppendKeyLeafs(Include<TEntity> include);
    }
}