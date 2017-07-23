using System;
using DashboardCode.AdminkaV1.DataAccessEfCore.Services;
using DashboardCode.Routines;

namespace DashboardCode.AdminkaV1.DataAccessEfCore
{
    public class AdminkaDbContextHandler
    {
        readonly DbContextFactory dbContextFactory;
        readonly RoutineState<UserContext> state;
        readonly Action<object> setAudit;
        public AdminkaDbContextHandler(RoutineState<UserContext> state, Action<object> setAudit, IAdminkaOptionsFactory optionsFactory)
        {
            this.state = state;
            this.setAudit = setAudit;
            dbContextFactory = new DbContextFactory(optionsFactory, state);
        }
        public TOutput Handle<TOutput>(Func<AdminkaDbContext, RoutineState<UserContext>, Action<object>, TOutput> func)
        {
            using (var context = dbContextFactory.CreateDbContext())
            {
                return func(context, state, setAudit);
            }
        }
        public void Handle(Action<AdminkaDbContext, RoutineState<UserContext>, Action<object>> func)
        {
            using (var context = dbContextFactory.CreateDbContext())
            {
                func(context, state, setAudit);
            }
        }
        public TOutput Handle<TOutput>(Func<AdminkaDbContext, Action<object>, TOutput> func)
        {
            using (var context = dbContextFactory.CreateDbContext())
            {
                return func(context, setAudit);
            }
        }
        public TOutput Handle<TOutput>(Func<AdminkaDbContext,  TOutput> func)
        {
            using (var context = dbContextFactory.CreateDbContext())
            {
                return func(context);
            }
        }
        public void Handle(Action<AdminkaDbContext, Action<object>> func)
        {
            using (var context = dbContextFactory.CreateDbContext())
            {
                func(context, setAudit);
            }
        }
        public void Handle(Action<AdminkaDbContext> func)
        {
            using (var context = dbContextFactory.CreateDbContext())
            {
                func(context);
            }
        }
    }
}
