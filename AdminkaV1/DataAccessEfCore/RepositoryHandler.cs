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
        readonly Func<Exception, List<FieldMessage>> analyzeException;
        readonly bool noTracking;

        public RepositoryHandler(
            AdminkaDbContextHandler adminkaDbContextHandler,
            Func<Exception, List<FieldMessage>> analyzeException,
            bool noTracking
            )
        {
            this.adminkaDbContextHandler = adminkaDbContextHandler;
            this.analyzeException = analyzeException;
            this.noTracking = noTracking;
        }

        public void Handle(Action<IRepository<TEntity>> action)
        {
            adminkaDbContextHandler.Handle((container) => action(new Repository<TEntity>(container, noTracking)));
        }

        public TOutput Handle<TOutput>(Func<IRepository<TEntity>, TOutput> func)
        {
            return adminkaDbContextHandler.Handle((container) => func(new Repository<TEntity>(container, noTracking)));
        }

        public Task<TOutput> HandleAsync<TOutput>(Func<IRepository<TEntity>, TOutput> func)
        {
            return Task.Run(() =>
            {
                var output = default(TOutput);
                adminkaDbContextHandler.Handle((container) =>
                {
                    output = func(new Repository<TEntity>(container, noTracking));
                });
                return output;
            });
        }

        public void Handle(Action<IRepository<TEntity>, IOrmStorage<TEntity>> action)
        {
            adminkaDbContextHandler.Handle((container, context, isAuditable, setAuditProperties) => action(
                new Repository<TEntity>(container, false),
                new OrmStorage<TEntity>(container, analyzeException, isAuditable, setAuditProperties)
                ));
        }

        public TOutput Handle<TOutput>(Func<IRepository<TEntity>, IOrmStorage<TEntity>, TOutput> func)
        {
            return adminkaDbContextHandler.Handle((container, context, isAuditable,setAuditProperties) => func(
                new Repository<TEntity>(container, false),
                new OrmStorage<TEntity>(container, analyzeException, isAuditable, setAuditProperties)
                ));
        }

        public Task<TOutput> HandleAsync<TOutput>(Func<IRepository<TEntity>, IOrmStorage<TEntity>, TOutput> func)
        {
            return Task.Run(() =>
            {
                var output = default(TOutput);
                adminkaDbContextHandler.Handle((container, context, isAuditable, setAuditProperties) =>
                {
                    output = func(
                        new Repository<TEntity>(container, noTracking),
                        new OrmStorage<TEntity>(container,  analyzeException, isAuditable, setAuditProperties)
                        );
                });
                return output;
            });
        }

        public void Handle(Action<IRepository<TEntity>, IOrmStorage<TEntity>, IModel<TEntity>> action)
        {
            adminkaDbContextHandler.Handle((container, context, isAuditable, setAuditProperties) => action(
               new Repository<TEntity>(container, false),
               new OrmStorage<TEntity>(container, analyzeException, isAuditable, setAuditProperties),
               new Model<TEntity>(container)
               ));
        }

        public TOutput Handle<TOutput>(Func<IRepository<TEntity>, IOrmStorage<TEntity>, IModel<TEntity>, TOutput> func)
        {
            return adminkaDbContextHandler.Handle((container, context, isAuditable, setAuditProperties) => func(
                new Repository<TEntity>(container, false),
                new OrmStorage<TEntity>(container, analyzeException, isAuditable, setAuditProperties),
                new Model<TEntity>(container)
                ));
        }

        public Task<TOutput> HandleAsync<TOutput>(Func<IRepository<TEntity>, IOrmStorage<TEntity>, IModel<TEntity>, TOutput> func)
        {
            return Task.Run(() =>
            {
                var output = default(TOutput);
                adminkaDbContextHandler.Handle((container, context, isAuditable, setAuditProperties) =>
                {
                    output = func(
                        new Repository<TEntity>(container, noTracking),
                        new OrmStorage<TEntity>(container, analyzeException, isAuditable, setAuditProperties),
                        new Model<TEntity>(container)
                        );
                });
                return output;
            });
        }
    }
}