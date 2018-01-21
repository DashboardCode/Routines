namespace DashboardCode.Routines.Storage
{
    public interface IOrmHandlerFactory<TUserContext>
    {
        IOrmHandler<TEntity> CreateAdminkaOrmHandler<TEntity>(RoutineClosure<TUserContext> state) where TEntity : class;
    }

    public interface IRepositoryHandlerFactory<TUserContext>
    {
        IRepositoryHandler<TEntity> CreateAdminkaRespositoryHandler<TEntity>(RoutineClosure<TUserContext> state) where TEntity : class;
    }
}
