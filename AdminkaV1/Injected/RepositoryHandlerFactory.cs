using System;
using Vse.AdminkaV1.DataAccessEfCore.Services;
using Vse.Routines;
using Vse.Routines.Storage;

namespace Vse.AdminkaV1.Injected
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
