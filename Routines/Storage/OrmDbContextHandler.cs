using System;

namespace DashboardCode.Routines.Storage
{
    public class OrmDbContextHandler<TUserContext, TDbContext> where TDbContext : IDisposable
    {
        readonly Func<RoutineClosure<TUserContext>, TDbContext> dbContextFactory;
        readonly RoutineClosure<TUserContext>    closure;
        readonly Func<object, bool>             getIsAuditable;
        readonly Action<object>                 setAuditProperties;

        public OrmDbContextHandler(
                RoutineClosure<TUserContext> closure,
                Func<RoutineClosure<TUserContext>, TDbContext> dbContextFactory, 
                Func<object, bool> getIsAuditable, 
                Action<object> setAuditProperties
            )
        {
            this.closure = closure;
            this.dbContextFactory = dbContextFactory;
            this.getIsAuditable = getIsAuditable;
            this.setAuditProperties = setAuditProperties;
        }

        public TOutput Handle<TOutput>(Func<TDbContext, RoutineClosure<TUserContext>, Func<object,bool>, Action<object>, TOutput> func)
        {
            using (var context = dbContextFactory(closure))
                return func(context, closure, getIsAuditable , setAuditProperties);
        }
        public void Handle(Action<TDbContext, RoutineClosure<TUserContext>, Func<object, bool>, Action<object>> func)
        {
            using (var context = dbContextFactory(closure))
                func(context, closure, getIsAuditable , setAuditProperties);
        }
        public TOutput Handle<TOutput>(Func<TDbContext, Func<object, bool>, Action<object>, TOutput> func)
        {
            using (var context = dbContextFactory(closure))
                return func(context, getIsAuditable , setAuditProperties);
        }
        public void Handle(Action<TDbContext, Func<object, bool>, Action<object>> func)
        {
            using (var context = dbContextFactory(closure))
                func(context, getIsAuditable , setAuditProperties);
        }
    }
}