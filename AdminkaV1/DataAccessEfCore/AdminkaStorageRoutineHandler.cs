using System;
using DashboardCode.Routines;
using DashboardCode.Routines.Logging;
using DashboardCode.Routines.Storage;
using DashboardCode.Routines.Storage.EfCore;

namespace DashboardCode.AdminkaV1.DataAccessEfCore
{
    public class AdminkaStorageRoutineHandler : EfCoreStorageRoutineHandler<UserContext, AdminkaDbContext>
    {
        public AdminkaStorageRoutineHandler(
            AdminkaStorageConfiguration adminkaStorageConfiguration,
            UserContext userContext,

            IEntityMetaServiceContainer entityMetaServiceContainer,
            Action<string> efDbContextVerbose,
            IHandler<RoutineClosure<UserContext>> routineHandler) :
            this(
                entityMetaServiceContainer,
                userContext,
                () => DataAccessEfCoreManager.CreateAdminkaDbContext(adminkaStorageConfiguration, efDbContextVerbose),
                routineHandler)
        {
        }

        private AdminkaStorageRoutineHandler(
            IEntityMetaServiceContainer entityMetaServiceContainer,
            UserContext userContext,
            Func<AdminkaDbContext> createDbContext,
            IHandler<RoutineClosure<UserContext>> routineHandler) :
            base(
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