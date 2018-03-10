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
            Action<string> efDbContextVerbose,
            IRoutineHandler<RoutineClosure<UserContext>> routineHandler) :
            this(
                entityMetaServiceContainer,
                () => DataAccessEfCoreManager.CreateAdminkaDbContext(adminkaStorageConfiguration, efDbContextVerbose),
                userContext,
                routineHandler)
        {
        }

        private AdminkaStorageRoutineHandler(
            IEntityMetaServiceContainer entityMetaServiceContainer,
            Func<AdminkaDbContext> createDbContext,
            UserContext userContext,
            IRoutineHandler<RoutineClosure<UserContext>> routineHandler) :
            base(
                userContext,
                entityMetaServiceContainer,
                createDbContext,
                () => new ValueTuple<AdminkaDbContext, IAuditVisitor>(
                    createDbContext(),
                    new AuditVisitor(userContext)
                ),
                routineHandler)
        {
        }
    }
}