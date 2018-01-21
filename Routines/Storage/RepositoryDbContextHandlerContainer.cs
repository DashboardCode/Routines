using System;

namespace DashboardCode.Routines.Storage
{
    public class RepositoryDbContextHandlerContainer<TUserContext, TDbContext> where TDbContext : IDisposable
    {
        readonly RoutineClosure<TUserContext> closure;
        readonly Func<RoutineClosure<TUserContext>, TDbContext> adminkaDbContextContainer;

        public RepositoryDbContextHandlerContainer(
            RoutineClosure<TUserContext> closure,
            Func<RoutineClosure<TUserContext>, TDbContext> adminkaDbContextContainer
            )
        {
            this.closure = closure;
            this.adminkaDbContextContainer = adminkaDbContextContainer;
        }

        public RepositoryDbContextHandler<TUserContext, TDbContext> Resolve()
        {
            var dbContextHandler = new RepositoryDbContextHandler<TUserContext, TDbContext>(closure, adminkaDbContextContainer);
            return dbContextHandler;
        }
    }
}