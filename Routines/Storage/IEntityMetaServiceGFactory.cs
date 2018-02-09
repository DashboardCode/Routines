namespace DashboardCode.Routines.Storage
{
    public interface IEntityMetaServiceContainer
    {
        IEntityMetaService<TEntity> Resolve<TEntity>() where TEntity : class;
    }
}