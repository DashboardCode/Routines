using System;
using DashboardCode.Routines;
using DashboardCode.Routines.Storage;
using DashboardCode.Routines.Storage.EfCore;

namespace DashboardCode.AdminkaV1.DataAccessEfCore
{
    public class AdminkaStorageRoutineHandler<TUserContext> : EfCoreStorageRoutineHandler<TUserContext, AdminkaDbContext>
    {
        public AdminkaStorageRoutineHandler(
            AdminkaStorageConfiguration adminkaStorageConfiguration,
            TUserContext userContext,

            IEntityMetaServiceContainer entityMetaServiceContainer,
            Action<string> efDbContextVerbose,
            IHandler<RoutineClosure<TUserContext>> routineHandler,
            Func<TUserContext, string> getAudit) :
            this(
                entityMetaServiceContainer,
                userContext,
                () => DataAccessEfCoreManager.CreateAdminkaDbContext(adminkaStorageConfiguration, efDbContextVerbose),
                routineHandler, getAudit)
        {
        }

        private AdminkaStorageRoutineHandler(
            IEntityMetaServiceContainer entityMetaServiceContainer,
            TUserContext userContext,
            Func<AdminkaDbContext> createDbContext,
            IHandler<RoutineClosure<TUserContext>> routineHandler,
            Func<TUserContext, string> getAudit) :
            base(
                entityMetaServiceContainer,
                createDbContext,
                () => new ValueTuple<AdminkaDbContext, IAuditVisitor>(
                    createDbContext(),
                    new AuditVisitor<IVersioned>(
                        (e)=> { e.RowVersionAt = DateTime.Now; e.RowVersionBy = getAudit(userContext); })
                ),
                routineHandler)
        {
        }
    }
}