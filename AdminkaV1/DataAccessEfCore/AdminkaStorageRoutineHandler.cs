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
            UserContext userContext,
            IExceptionHandler exceptionHandler,
            IRoutineLogging routineLogging,
            RoutineClosure<UserContext> closure, 
            object input) :
            this(
                userContext,
                entityMetaServiceContainer,
                () => DataAccessEfCoreManager.CreateAdminkaDbContext(adminkaStorageConfiguration, closure),
                exceptionHandler, routineLogging, closure,
                input)
        {
        }

        private AdminkaStorageRoutineHandler(
            UserContext userContext,
            IEntityMetaServiceContainer entityMetaServiceContainer,
            Func<AdminkaDbContext> createDbContext,
            IExceptionHandler exceptionHandler,
            IRoutineLogging routineLogging,
            RoutineClosure<UserContext> closure,
            object input) :
            base(
                userContext,
                entityMetaServiceContainer,
                createDbContext,
                () => new ValueTuple<AdminkaDbContext, IAuditVisitor>(
                    createDbContext(),
                    new AuditVisitor(closure.UserContext)
                ),
                exceptionHandler, routineLogging, closure,
                input)
        {
        }
    }
}