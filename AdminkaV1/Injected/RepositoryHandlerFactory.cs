using DashboardCode.Routines;
using DashboardCode.Routines.Storage;
using DashboardCode.AdminkaV1.DataAccessEfCore.Services;

namespace DashboardCode.AdminkaV1.Injected
{
    public class RepositoryHandlerFactory : IRepositoryHandlerFactory<UserContext>
    {
        readonly StorageMetaService storageMetaService;
        readonly AdminkaStorageConfiguration adminkaStorageConfiguration;
        public RepositoryHandlerFactory(AdminkaStorageConfiguration adminkaStorageConfiguration, StorageMetaService storageMetaService)
        {
            this.adminkaStorageConfiguration = adminkaStorageConfiguration;
            this.storageMetaService = storageMetaService;
        }
        public DataAccessFactory CreateDataAccessFactory(Routine<UserContext> state)
        {
            var dataAccessFactory = new DataAccessFactory(state, adminkaStorageConfiguration, storageMetaService, InjectedManager.Analyze);
            return dataAccessFactory;
        }

        public IRepositoryHandler<TEntity> CreateRepositoryHandler<TEntity>(Routine<UserContext> state) where TEntity : class
        {
            var dataAccessFactory = CreateDataAccessFactory(state);
            var repositoryHandler = dataAccessFactory.CreateRepositoryHandler<TEntity>();
            return repositoryHandler;
        }
    }
}