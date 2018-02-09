namespace DashboardCode.Routines.Storage
{
    public interface IRepositoryHandlerGFactory<TUserContext>
    {
        IIndependentRepositoryHandler<TUserContext, TEntity> Create<TEntity>(RoutineClosure<TUserContext> closure, bool noTracking = true) where TEntity : class;
    }
}