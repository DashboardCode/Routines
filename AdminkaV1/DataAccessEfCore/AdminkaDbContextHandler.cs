using System;
using DashboardCode.AdminkaV1.DataAccessEfCore.Services;
using DashboardCode.Routines;

namespace DashboardCode.AdminkaV1.DataAccessEfCore
{
    public class AdminkaDbContextHandler
    {
        readonly AdminkaDbContextFactory dbContextFactory;
        readonly Routine<UserContext> state;
        readonly Action<object> setAudit;
        public AdminkaDbContextHandler(Routine<UserContext> state, Action<object> setAudit, IAdminkaOptionsFactory optionsFactory)
        {
            this.state = state;
            this.setAudit = setAudit;
            dbContextFactory = new AdminkaDbContextFactory(optionsFactory, state);
        }
        public TOutput Handle<TOutput>(Func<AdminkaDbContext, Routine<UserContext>, Action<object>, TOutput> func)
        {
            using (var context = dbContextFactory.CreateAdminkaDbContext())
            {
                return func(context, state, setAudit);
            }
        }
        public void Handle(Action<AdminkaDbContext, Routine<UserContext>, Action<object>> func)
        {
            using (var context = dbContextFactory.CreateAdminkaDbContext())
            {
                func(context, state, setAudit);
            }
        }
        public TOutput Handle<TOutput>(Func<AdminkaDbContext, Action<object>, TOutput> func)
        {
            using (var context = dbContextFactory.CreateAdminkaDbContext())
            {
                return func(context, setAudit);
            }
        }
        public TOutput Handle<TOutput>(Func<AdminkaDbContext,  TOutput> func) 
        {
            using (var context = dbContextFactory.CreateAdminkaDbContext())
            {
                return func(context);
            }
        }
        public void Handle(Action<AdminkaDbContext, Action<object>> func)
        {
            using (var context = dbContextFactory.CreateAdminkaDbContext())
            {
                func(context, setAudit);
            }
        }
        public void Handle(Action<AdminkaDbContext> func)
        {
            using (var context = dbContextFactory.CreateAdminkaDbContext())
            {
                func(context);
            }
        }
    }
}
