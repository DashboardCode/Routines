namespace DashboardCode.Routines.Storage
{
    public interface IModel<TEntity> where TEntity : class
    {
        Include<TEntity> AppendModelFields(Include<TEntity> include);

        Include<TEntity> AppendModelFieldsIfEmpty(Include<TEntity> include);

        Include<TEntity> ExtractNavigations(Include<TEntity> include);

        Include<TEntity> ExtractNavigationsAppendKeyLeafs(Include<TEntity> include);
    }
}
