using System;
using DashboardCode.AdminkaV1.DataAccessEfCore.Services;
using DashboardCode.Routines;

namespace DashboardCode.AdminkaV1.DataAccessEfCore
{
    public class AdminkaDbContextHandler
    {
        readonly AdminkaDbContextFactory dbContextFactory;
        readonly Routine<UserContext>    state;
        readonly Action<object>          setAuditProperties;
        public AdminkaDbContextHandler(Routine<UserContext> state, Action<object> setAuditProperties, IAdminkaOptionsFactory optionsFactory)
        {
            this.state = state;
            this.setAuditProperties = setAuditProperties;
            dbContextFactory = new AdminkaDbContextFactory(optionsFactory, state);
        }
        public TOutput Handle<TOutput>(Func<AdminkaDbContext, Routine<UserContext>, Action<object>, TOutput> func)
        {
            using (var context = dbContextFactory.CreateAdminkaDbContext())
            {
                return func(context, state, setAuditProperties);
            }
        }
        public void Handle(Action<AdminkaDbContext, Routine<UserContext>, Action<object>> func)
        {
            using (var context = dbContextFactory.CreateAdminkaDbContext())
            {
                func(context, state, setAuditProperties);
            }
        }
        public TOutput Handle<TOutput>(Func<AdminkaDbContext, Action<object>, TOutput> func)
        {
            using (var context = dbContextFactory.CreateAdminkaDbContext())
            {
                return func(context, setAuditProperties);
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
                func(context, setAuditProperties);
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