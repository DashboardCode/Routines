using System;

using DashboardCode.Routines;
using DashboardCode.Routines.Storage.EfCore;
using DashboardCode.AdminkaV1.DataAccessEfCore.Services;

namespace DashboardCode.AdminkaV1.DataAccessEfCore
{
    public class AdminkaDbContextHandler
    {
        readonly AdminkaDbContextFactory dbContextFactory;
        readonly Routine<UserContext>    state;
        readonly Action<object>          setAuditProperties;
        readonly Func<object, bool> isAuditable;
        public AdminkaDbContextHandler(Routine<UserContext> state, Func<object, bool> isAuditable, Action<object> setAuditProperties, IDbContextOptionsBuilder optionsBuilder)
        {
            this.state = state;
            this.isAuditable= isAuditable;
            this.setAuditProperties = setAuditProperties;
            dbContextFactory = new AdminkaDbContextFactory(optionsBuilder, state);
        }
        public TOutput Handle<TOutput>(Func<AdminkaDbContext, Routine<UserContext>, Func<object,bool>, Action<object>, TOutput> func)
        {
            using (var context = dbContextFactory.CreateAdminkaDbContext())
            {
                return func(context, state, isAuditable, setAuditProperties);
            }
        }
        public void Handle(Action<AdminkaDbContext, Routine<UserContext>, Func<object, bool>, Action<object>> func)
        {
            using (var context = dbContextFactory.CreateAdminkaDbContext())
            {
                func(context, state, isAuditable, setAuditProperties);
            }
        }
        public TOutput Handle<TOutput>(Func<AdminkaDbContext, Func<object, bool>, Action<object>, TOutput> func)
        {
            using (var context = dbContextFactory.CreateAdminkaDbContext())
            {
                return func(context, isAuditable, setAuditProperties);
            }
        }
        public TOutput Handle<TOutput>(Func<AdminkaDbContext,  TOutput> func) 
        {
            using (var context = dbContextFactory.CreateAdminkaDbContext())
            {
                return func(context);
            }
        }
        public void Handle(Action<AdminkaDbContext, Func<object, bool>, Action<object>> func)
        {
            using (var context = dbContextFactory.CreateAdminkaDbContext())
            {
                func(context, isAuditable, setAuditProperties);
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