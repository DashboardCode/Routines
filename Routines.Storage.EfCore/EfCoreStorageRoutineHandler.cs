using System;
using Microsoft.EntityFrameworkCore;

using DashboardCode.Routines.Injected;

namespace DashboardCode.Routines.Storage.EfCore
{
    public class EfCoreStorageRoutineHandler<TUserContext, TDbContext> : StorageRoutineHandler<TUserContext, TDbContext>
        where TDbContext : DbContext
    {
        static readonly EfCoreEntityOrmStorageGFactory<TDbContext> gFactory = new EfCoreEntityOrmStorageGFactory<TDbContext>();

        public EfCoreStorageRoutineHandler(
            TUserContext userContext, 
            IStorageMetaService storageMetaService, 
            Func<RoutineClosure<TUserContext>, TDbContext> createDbContext, 
            Func<RoutineClosure<TUserContext>, (TDbContext, IAuditVisitor)> createDbContextForStorage,
            IBasicLogging basicLogging, 
            Func<Exception, Exception> transformException, 
            Func<Action<DateTime, string>, 
            RoutineClosure<TUserContext>> createRoutineState, object input) : 
            
            base(userContext, storageMetaService, createDbContext, createDbContextForStorage, gFactory, gFactory, basicLogging, transformException, createRoutineState, input)
        {
        }
    }
}