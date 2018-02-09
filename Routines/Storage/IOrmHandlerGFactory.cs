namespace DashboardCode.Routines.Storage
{
    public interface IOrmHandlerGFactory<TUserContext>
    {
        IIndependentOrmHandler<TUserContext, TEntity> Create<TEntity>(RoutineClosure<TUserContext> closure, bool noTracking = true) where TEntity : class;
    }
}
