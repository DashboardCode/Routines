using System;

using DashboardCode.Routines;
using DashboardCode.Routines.Storage;

namespace DashboardCode.AdminkaV1.DataAccessEfCore
{
    /// Redefinitions to stop reference's propogation to AdminkaDbContext parents 
    /// (DashboardCode.AdminkaV1.DataAccessEfCore.VerboseDbContext, Microsoft.EntityFrameworkCore.DbContext)
    /// We can't use AdminkaDbContext in generic types definitions without referencing all parent's assemblies.

    public delegate void AdminkaDbContextUniAction(AdminkaDbContext adminkaDbContext);

    public delegate TOutput AdminkaDbContextUniFunc<TOutput>(AdminkaDbContext adminkaDbContext);

    public delegate void AdminkaDbContextOrmFactoryUniAction(ReliantOrmHandlerGFactory<UserContext, AdminkaDbContext> adminkaDbContextFactory);

    public delegate TOutput AdminkaDbContextOrmFactoryUniFunc<TOutput>(ReliantOrmHandlerGFactory<UserContext, AdminkaDbContext> adminkaDbContextFactory);

    public delegate void AdminkaDbContextRepositoryFactoryUniAction(ReliantRepositoryHandlerGFactory<UserContext, AdminkaDbContext> adminkaDbContextFactory);

    public delegate TOutput AdminkaDbContextRepositoryFactoryUniFunc<TOutput>(ReliantRepositoryHandlerGFactory<UserContext, AdminkaDbContext> adminkaDbContextFactory);
    // ---------

    public delegate void AdminkaDbContextAction(AdminkaDbContext adminkaDbContext, RoutineClosure<UserContext> closure);

    public delegate TOutput AdminkaDbContextFunc<TOutput>(AdminkaDbContext adminkaDbContext, RoutineClosure<UserContext> closure);

    public delegate void AdminkaDbContextOrmFactoryAction(ReliantOrmHandlerGFactory<UserContext, AdminkaDbContext> adminkaDbContextFactory, RoutineClosure<UserContext> closure);

    public delegate TOutput AdminkaDbContextOrmFactoryFunc<TOutput>(ReliantOrmHandlerGFactory<UserContext, AdminkaDbContext> adminkaDbContextFactory, RoutineClosure<UserContext> closure);

    public delegate void AdminkaDbContextRepositoryFactoryAction(ReliantRepositoryHandlerGFactory<UserContext, AdminkaDbContext> adminkaDbContextFactory, RoutineClosure<UserContext> closure);

    public delegate TOutput AdminkaDbContextRepositoryFactoryFunc<TOutput>(ReliantRepositoryHandlerGFactory<UserContext, AdminkaDbContext> adminkaDbContextFactory, RoutineClosure<UserContext> closure);

    public class AdminkaDataAccessFacade : DataAccessFacade<UserContext, AdminkaDbContext>
    {
        public AdminkaDataAccessFacade(
                IStorageMetaService storageMetaService,
                Func<RoutineClosure<UserContext>, AdminkaDbContext> dbContextFactory
            ) : base(
                        new RepositoryGFactory(),
                        new OrmGFactory(),
                        storageMetaService,
                        (closure) => new AuditVisitor(closure.UserContext),
                        dbContextFactory
            )
        {
        }

        public Action<RoutineClosure<UserContext>> ComposeAdminkaDbContextActionHandled(AdminkaDbContextAction action) =>
            ComposeDbContextActionHandled((dContext, closure) => action(dContext, closure));

        public Func<RoutineClosure<UserContext>, TOutput> ComposeAdminkaDbContextFuncHandled<TOutput>(AdminkaDbContextFunc<TOutput> func) =>
            ComposeDbContextFuncHandled((dContext, closure) => func(dContext, closure));

        public Action<RoutineClosure<UserContext>> ComposeAdminkaDbContextOrmFactoryActionHandled(AdminkaDbContextOrmFactoryAction action) =>
            ComposeOrmFactoryActionHandled((dContext, closure) => action(dContext, closure));

        public Func<RoutineClosure<UserContext>, TOutput> ComposeAdminkaDbContextOrmFactoryFuncHandled<TOutput>(AdminkaDbContextOrmFactoryFunc<TOutput> func) =>
            ComposeOrmFactoryActionHandled((dContext, closure) => func(dContext, closure));

        public Action<RoutineClosure<UserContext>> ComposeAdminkaDbContextRepositoryFactoryActionHandled(AdminkaDbContextRepositoryFactoryAction action) =>
            ComposeRepositoryFactoryActionHandled((dContext, closure) => action(dContext, closure));

        public Func<RoutineClosure<UserContext>, TOutput> ComposeAdminkaDbContextRepositoryFactoryFuncHandled<TOutput>(AdminkaDbContextRepositoryFactoryFunc<TOutput> func) =>
            ComposeRepositoryFactoryActionHandled((dContext, closure) => func(dContext, closure));



        public Action<RoutineClosure<UserContext>> ComposeAdminkaDbContextActionHandled(AdminkaDbContextUniAction action) =>
            ComposeDbContextActionHandled((dContext) => action(dContext));

        public Func<RoutineClosure<UserContext>, TOutput> ComposeAdminkaDbContextFuncHandled<TOutput>(AdminkaDbContextUniFunc<TOutput> func) =>
            ComposeDbContextFuncHandled((dContext) => func(dContext));

        public Action<RoutineClosure<UserContext>> ComposeAdminkaDbContextOrmFactoryActionHandled(AdminkaDbContextOrmFactoryUniAction action) =>
            ComposeOrmFactoryActionHandled((dContext) => action(dContext));

        public Func<RoutineClosure<UserContext>, TOutput> ComposeAdminkaDbContextOrmFactoryFuncHandled<TOutput>(AdminkaDbContextOrmFactoryUniFunc<TOutput> func) =>
            ComposeOrmFactoryActionHandled((dContext) => func(dContext));

        public Action<RoutineClosure<UserContext>> ComposeAdminkaDbContextRepositoryFactoryActionHandled(AdminkaDbContextRepositoryFactoryUniAction action) =>
            ComposeRepositoryFactoryActionHandled((dContext) => action(dContext));

        public Func<RoutineClosure<UserContext>, TOutput> ComposeAdminkaDbContextRepositoryFactoryFuncHandled<TOutput>(AdminkaDbContextRepositoryFactoryUniFunc<TOutput> func) =>
            ComposeRepositoryFactoryActionHandled((dContext) => func(dContext));
    }
}