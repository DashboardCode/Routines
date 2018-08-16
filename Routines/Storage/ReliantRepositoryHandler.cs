using System;
using System.Threading.Tasks;

namespace DashboardCode.Routines.Storage
{
    public class ReliantRepositoryHandler<TEntity> where TEntity : class
    {
        readonly IRepository<TEntity> repository;
        internal ReliantRepositoryHandler(
                IRepository<TEntity> repository
            )
        {
            this.repository = repository;
        }

        public void Handle(Action<IRepository<TEntity>> action)
        {
            action(repository);
        }

        public TOutput Handle<TOutput>(Func<IRepository<TEntity>, TOutput> func)
        {
            return func(repository);
        }

        public Task<TOutput> HandleAsync<TOutput>(Func<IRepository<TEntity>, Task<TOutput>> func)
        {
            return func(repository);
        }
    }
}
