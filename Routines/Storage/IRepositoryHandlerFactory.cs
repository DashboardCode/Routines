namespace DashboardCode.Routines.Storage
{
    public interface IRepositoryHandlerFactory<TUserContext>
    {
        IRepositoryHandler<TEntity> CreateRepositoryHandler<TEntity>(Routine<TUserContext> state) where TEntity : class;
    }
}
