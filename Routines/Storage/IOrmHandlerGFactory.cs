namespace DashboardCode.Routines.Storage
{
    public interface IOrmHandlerGFactory<TUserContext>
    {
        IOrmHandler<TEntity> Create<TEntity>(RoutineClosure<TUserContext> closure, bool noTracking = true) where TEntity : class;
    }
}