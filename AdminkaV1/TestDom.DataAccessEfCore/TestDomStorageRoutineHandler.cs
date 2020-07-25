using System;
using DashboardCode.Routines;
using DashboardCode.Routines.Storage;
using DashboardCode.Routines.Storage.EfCore;

namespace DashboardCode.AdminkaV1.TestDom.DataAccessEfCore
{
    public class TestDomStorageRoutineHandler<TUserContext> : EfCoreStorageRoutineHandler<TUserContext, TestDomDbContext>
    {
        public TestDomStorageRoutineHandler(
            AdminkaStorageConfiguration adminkaStorageConfiguration,
            TUserContext userContext,

            Action<string> efDbContextVerbose,
            IHandler<RoutineClosure<TUserContext>> routineHandler,
            Func<TUserContext, string> getAudit) :
            this(
                TestDomDataAccessEfCoreManager.TestDomEntityMetaServiceContainer,
                userContext,
                () => TestDomDataAccessEfCoreManager.CreateAdminkaDbContext(adminkaStorageConfiguration, efDbContextVerbose),
                routineHandler, getAudit)
        {
        }

        private TestDomStorageRoutineHandler(
            IEntityMetaServiceContainer entityMetaServiceContainer,
            TUserContext userContext,
            Func<TestDomDbContext> createDbContext,
            IHandler<RoutineClosure<TUserContext>> routineHandler,
            Func<TUserContext, string> getAudit) :
            base(
                entityMetaServiceContainer,
                createDbContext,
                () => new ValueTuple<TestDomDbContext, IAuditVisitor>(
                    createDbContext(),
                    new AuditVisitor<IVersioned>(
                        (e)=> { e.RowVersionAt = DateTime.Now; e.RowVersionBy = getAudit(userContext); })
                ),
                routineHandler)
        {
        }
    }

    public class TestDomStorageRoutineHandlerAsync<TUserContext> : EfCoreStorageRoutineHandlerAsync<TUserContext, TestDomDbContext>
    {
        public TestDomStorageRoutineHandlerAsync(
            AdminkaStorageConfiguration adminkaStorageConfiguration,
            TUserContext userContext,

            Action<string> efDbContextVerbose,
            IHandlerAsync<RoutineClosure<TUserContext>> routineHandler,
            Func<TUserContext, string> getAudit) :
            this(
                TestDomDataAccessEfCoreManager.TestDomEntityMetaServiceContainer,
                userContext,
                () => TestDomDataAccessEfCoreManager.CreateAdminkaDbContext(adminkaStorageConfiguration, efDbContextVerbose),
                routineHandler, getAudit)
        {
        }

        private TestDomStorageRoutineHandlerAsync(
            IEntityMetaServiceContainer entityMetaServiceContainer,
            TUserContext userContext,
            Func<TestDomDbContext> createDbContext,
            IHandlerAsync<RoutineClosure<TUserContext>> routineHandler,
            Func<TUserContext, string> getAudit) :
            base(
                entityMetaServiceContainer,
                createDbContext,
                () => new ValueTuple<TestDomDbContext, IAuditVisitor>(
                    createDbContext(),
                    new AuditVisitor<IVersioned>(
                        (e) => { e.RowVersionAt = DateTime.Now; e.RowVersionBy = getAudit(userContext); })
                ),
                routineHandler)
        {
        }
    }
}