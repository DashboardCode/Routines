using System;
using Microsoft.EntityFrameworkCore;

using DashboardCode.Routines.Injected;

namespace DashboardCode.Routines.Storage.EfCore
{
    public class EfCoreStorageRoutineHandler<TUserContext, TDbContext> : StorageRoutineHandler<TUserContext, TDbContext>
        where TDbContext : DbContext
    {
        static readonly RepositoryContainer<TDbContext> repositoryContainer = new RepositoryContainer<TDbContext>();
        static readonly OrmContainer<TDbContext> ormContainer = new OrmContainer<TDbContext>();

        public EfCoreStorageRoutineHandler(
            TUserContext userContext,
            IEntityMetaServiceContainer entityMetaServiceContainer, 
            Func<RoutineClosure<TUserContext>, TDbContext> createDbContext, 
            Func<RoutineClosure<TUserContext>, (TDbContext, IAuditVisitor)> createDbContextForStorage,
            IBasicLogging basicLogging, 
            Func<Exception, Exception> transformException, 
            Func<Action<DateTime, string>, 
            RoutineClosure<TUserContext>> createRoutineState, object input) : 
            
            base(userContext, entityMetaServiceContainer, createDbContext, createDbContextForStorage, repositoryContainer, ormContainer, basicLogging, transformException, createRoutineState, input)
        {
        }
    }
}