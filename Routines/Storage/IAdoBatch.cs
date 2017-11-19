namespace DashboardCode.Routines.Storage
{
    public interface IAdoBatch
    {
        void RemoveAll<TEntity>() where TEntity : class;
    }
}