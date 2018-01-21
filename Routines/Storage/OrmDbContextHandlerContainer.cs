using System;

namespace DashboardCode.Routines.Storage
{
    public class OrmDbContextHandlerContainer<TUserContext, TDbContext> where TDbContext : IDisposable
    {
        readonly RoutineClosure<TUserContext> closure;
        readonly Func<RoutineClosure<TUserContext>, TDbContext> createDbContext;
        readonly Func<object, bool> hasAuditProperties;
        readonly Action<object> setAuditProperties;

        public OrmDbContextHandlerContainer(
            RoutineClosure<TUserContext> closure,
            Func<RoutineClosure<TUserContext>, TDbContext> createDbContext,
            Func<object, bool> hasAuditProperties,
            Action<object> setAuditProperties
            )
        {
            this.closure = closure;
            this.createDbContext = createDbContext;
            this.hasAuditProperties = hasAuditProperties;
            this.setAuditProperties = setAuditProperties;
        }

        public OrmDbContextHandler<TUserContext, TDbContext> Resolve()
        {
            var dbContextHandler = new OrmDbContextHandler<TUserContext, TDbContext>(closure, createDbContext, hasAuditProperties, setAuditProperties);
            return dbContextHandler;
        }
    }
}
