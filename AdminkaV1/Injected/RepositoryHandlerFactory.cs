using System;
using DashboardCode.AdminkaV1.DataAccessEfCore.Services;
using DashboardCode.Routines;
using DashboardCode.Routines.Storage;

namespace DashboardCode.AdminkaV1.Injected
{
    public class RepositoryHandlerFactory : IRepositoryHandlerFactory<UserContext>
    {
        readonly StorageMetaService storageMetaService;
        public RepositoryHandlerFactory(StorageMetaService storageMetaService)
        {
            this.storageMetaService = storageMetaService;
        }
        public DataAccessFactory CreateDataAccessFactory(RoutineState<UserContext> state)
        {
            var dataAccessFactory = new DataAccessFactory(state, storageMetaService);
            return dataAccessFactory;
        }

        public IRepositoryHandler<TEntity> CreateRepositoryHandler<TEntity>(RoutineState<UserContext> state) where TEntity : class
        {
            var dataAccessFactory = CreateDataAccessFactory(state);
            var repositoryHandler = dataAccessFactory.CreateRepositoryHandler<TEntity>();
            return repositoryHandler;
        }
    }
}
