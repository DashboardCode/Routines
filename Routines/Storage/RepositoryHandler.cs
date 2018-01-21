using System;
using System.Threading.Tasks;

namespace DashboardCode.Routines.Storage
{
    public class RepositoryHandler<TUserContext, TDbContext, TEntity> : IRepositoryHandler<TEntity> where TEntity : class where TDbContext : IDisposable 
    {
        readonly RepositoryDbContextHandler<TUserContext, TDbContext> repositoryDbContextPrimitiveHandler;
        readonly bool noTracking;
        readonly Func<TDbContext, bool, IRepository<TEntity>> createRepositoryFactoryMethod;
        public RepositoryHandler(
                RepositoryDbContextHandler<TUserContext, TDbContext> repositoryDbContextPrimitiveHandler,
                bool noTracking,
                Func<TDbContext, bool, IRepository<TEntity>> createRepositoryFactoryMethod
            )
        {
            this.repositoryDbContextPrimitiveHandler = repositoryDbContextPrimitiveHandler;
            this.noTracking = noTracking;
            this.createRepositoryFactoryMethod = createRepositoryFactoryMethod;
        }

        public void Handle(Action<IRepository<TEntity>> action)
        {
            repositoryDbContextPrimitiveHandler.Handle((dbContext) => action(createRepositoryFactoryMethod(dbContext, noTracking)));
        }

        public TOutput Handle<TOutput>(Func<IRepository<TEntity>, TOutput> func)
        {
            return repositoryDbContextPrimitiveHandler.Handle((dbContext) => func(createRepositoryFactoryMethod(dbContext, noTracking)));
        }

        public Task<TOutput> HandleAsync<TOutput>(Func<IRepository<TEntity>, TOutput> func)
        {
            return Task.Run(() =>
            {
                var output = default(TOutput);
                repositoryDbContextPrimitiveHandler.Handle((dbContext) =>
                {
                    output = func(createRepositoryFactoryMethod(dbContext, noTracking));
                });
                return output;
            });
        }
    }
}