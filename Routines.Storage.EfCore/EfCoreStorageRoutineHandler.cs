using System;
using Microsoft.EntityFrameworkCore;

namespace DashboardCode.Routines.Storage.EfCore
{
    public class EfCoreStorageRoutineHandler<TUserContext, TDbContext> : MetaStorageRoutineHandler<TUserContext, TDbContext>
        where TDbContext : DbContext
    {
        static readonly RepositoryContainer<TDbContext> repositoryContainer = new RepositoryContainer<TDbContext>();
        static readonly OrmContainer<TDbContext> ormContainer = new OrmContainer<TDbContext>();

        public EfCoreStorageRoutineHandler(
            IEntityMetaServiceContainer entityMetaServiceContainer, 
            Func<TDbContext> createDbContext, 
            Func<(TDbContext, IAuditVisitor)> createDbContextForStorage,
            IHandler<RoutineClosure<TUserContext>> routineHandler
            ) : 
                base(entityMetaServiceContainer, createDbContext, createDbContextForStorage, repositoryContainer, ormContainer,
                     routineHandler)
        {
        }
    }
}