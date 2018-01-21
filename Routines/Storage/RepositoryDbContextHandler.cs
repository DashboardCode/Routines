using System;

namespace DashboardCode.Routines.Storage
{
    public class RepositoryDbContextHandler<TUserContext, TDbContext> where TDbContext : IDisposable
    {
        readonly RoutineClosure<TUserContext> closure;
        readonly Func<RoutineClosure<TUserContext>, TDbContext> createDbContextFactoryMethod;

        public RepositoryDbContextHandler(
                RoutineClosure<TUserContext> closure,
                Func<RoutineClosure<TUserContext>, TDbContext> createDbContextFactoryMethod
            )
        {
            this.closure = closure;
            this.createDbContextFactoryMethod = createDbContextFactoryMethod;
        }

        public TOutput Handle<TOutput>(Func<TDbContext, RoutineClosure<TUserContext>, TOutput> func)
        {
            using (var context = createDbContextFactoryMethod(closure))
                return func(context, closure);
        }
        public void Handle(Action<TDbContext, RoutineClosure<TUserContext>> func)
        {
            using (var context = createDbContextFactoryMethod(closure))
                func(context, closure);
        }

        public TOutput Handle<TOutput>(Func<TDbContext,  TOutput> func)
        {
            using (var context = createDbContextFactoryMethod(closure))
                return func(context);
        }
        public void Handle(Action<TDbContext> func)
        {
            using (var context = createDbContextFactoryMethod(closure))
                func(context);
        }
    }
}