namespace Vse.Routines.Storage
{
    public interface IRepositoryHandlerFactory<TUserContext>
    {
        IRepositoryHandler<TEntity> CreateRepositoryHandler<TEntity>(RoutineState<TUserContext> state) where TEntity : class;
    }
}
