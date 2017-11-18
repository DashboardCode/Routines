using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DashboardCode.Routines.Storage;
using DashboardCode.Routines.Storage.EfCore;

namespace DashboardCode.AdminkaV1.DataAccessEfCore
{
    public class RepositoryHandler<TEntity> : IRepositoryHandler<TEntity> 
        where TEntity : class
    {
        readonly AdminkaDbContextHandler adminkaDbContextHandler;
        readonly Func<Exception, List<FieldError>> analyzeException;
        readonly bool noTracking;

        public RepositoryHandler(
            AdminkaDbContextHandler adminkaDbContextHandler,
            Func<Exception, List<FieldError>> analyzeException,
            bool noTracking
            )
        {
            this.adminkaDbContextHandler = adminkaDbContextHandler;
            this.analyzeException = analyzeException;
            this.noTracking = noTracking;
        }

        public void Handle(Action<IRepository<TEntity>> action)
        {
            adminkaDbContextHandler.Handle((container, context) => action(new Repository<TEntity>(container, noTracking)));
        }

        public TOutput Handle<TOutput>(Func<IRepository<TEntity>, TOutput> func)
        {
            return adminkaDbContextHandler.Handle((container, context) => func(new Repository<TEntity>(container, noTracking)));
        }

        public Task<TOutput> HandleAsync<TOutput>(Func<IRepository<TEntity>, TOutput> func)
        {
            return Task.Run(() =>
            {
                var output = default(TOutput);
                adminkaDbContextHandler.Handle((container, context) =>
                {
                    output = func(new Repository<TEntity>(container, noTracking));
                });
                return output;
            });
        }

        public void Handle(Action<IRepository<TEntity>, IStorage<TEntity>> action)
        {
            adminkaDbContextHandler.Handle((container, context, audit) => action(
                new Repository<TEntity>(container, false),
                new Storage<TEntity>(container, analyzeException, audit)
                ));
        }

        public TOutput Handle<TOutput>(Func<IRepository<TEntity>, IStorage<TEntity>, TOutput> func)
        {
            return adminkaDbContextHandler.Handle((container, context, audit) => func(
                new Repository<TEntity>(container, false),
                new Storage<TEntity>(container, analyzeException, audit)
                ));
        }

        public Task<TOutput> HandleAsync<TOutput>(Func<IRepository<TEntity>, IStorage<TEntity>, TOutput> func)
        {
            return Task.Run(() =>
            {
                var output = default(TOutput);
                adminkaDbContextHandler.Handle((container, context, audit) =>
                {
                    output = func(
                        new Repository<TEntity>(container, noTracking),
                        new Storage<TEntity>(container,  analyzeException, audit)
                        );
                });
                return output;
            });
        }

        

        public void Handle(Action<IRepository<TEntity>, IStorage<TEntity>, IModel<TEntity>> action)
        {
            adminkaDbContextHandler.Handle((container, context, audit) => action(
               new Repository<TEntity>(container, false),
               new Storage<TEntity>(container, analyzeException, audit),
               new Model<TEntity>(container)
               ));
        }

        public TOutput Handle<TOutput>(Func<IRepository<TEntity>, IStorage<TEntity>, IModel<TEntity>, TOutput> func)
        {
            return adminkaDbContextHandler.Handle((container, context, audit) => func(
                new Repository<TEntity>(container, false),
                new Storage<TEntity>(container, analyzeException, audit),
                new Model<TEntity>(container)
                ));
        }

        public Task<TOutput> HandleAsync<TOutput>(Func<IRepository<TEntity>, IStorage<TEntity>, IModel<TEntity>, TOutput> func)
        {
            return Task.Run(() =>
            {
                var output = default(TOutput);
                adminkaDbContextHandler.Handle((container, context, audit) =>
                {
                    output = func(
                        new Repository<TEntity>(container, noTracking),
                        new Storage<TEntity>(container, analyzeException, audit),
                        new Model<TEntity>(container)
                        );
                });
                return output;
            });
        }
    }
}
