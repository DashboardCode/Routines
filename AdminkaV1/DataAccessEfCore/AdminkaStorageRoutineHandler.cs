using System;
using DashboardCode.Routines;
using DashboardCode.Routines.Injected;
using DashboardCode.Routines.Storage;
using DashboardCode.Routines.Storage.EfCore;

namespace DashboardCode.AdminkaV1.DataAccessEfCore
{
    public class AdminkaStorageRoutineHandler : EfCoreStorageRoutineHandler<UserContext, AdminkaDbContext>
    {
        public AdminkaStorageRoutineHandler(
            AdminkaStorageConfiguration adminkaStorageConfiguration,
            IEntityMetaServiceContainer entityMetaServiceContainer,
            RoutineGuid routineGuid,
            UserContext userContext,
            IContainer container,
            IBasicLogging basicLogging,
            Func<Exception, Exception> transformException,
            object input) :
            base(userContext, entityMetaServiceContainer,
                closure => new AdminkaDbContextFactory(adminkaStorageConfiguration).Create(closure),
                closure => new ValueTuple<AdminkaDbContext, IAuditVisitor>(
                        new AdminkaDbContextFactory(adminkaStorageConfiguration).Create(closure),
                        new AuditVisitor(closure.UserContext)
                        ),
                basicLogging, 
                transformException,
                verbose => new RoutineClosure<UserContext>(userContext, routineGuid, verbose, container),
                //createRoutineClosure, 
                input)
        {
        }
    }
}
