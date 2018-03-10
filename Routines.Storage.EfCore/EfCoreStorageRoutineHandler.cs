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
            Func<TDbContext> createDbContext, 
            Func<(TDbContext, IAuditVisitor)> createDbContextForStorage,
            IRoutineHandler<RoutineClosure<TUserContext>> routineHandler
            ) : 
                base(userContext, entityMetaServiceContainer, createDbContext, createDbContextForStorage, repositoryContainer, ormContainer,
                     routineHandler)
        {
        }
    }
}