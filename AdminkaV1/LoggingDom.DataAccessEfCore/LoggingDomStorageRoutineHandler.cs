using System;
using DashboardCode.Routines;
using DashboardCode.Routines.Storage;
using DashboardCode.Routines.Storage.EfCore;

namespace DashboardCode.AdminkaV1.LoggingDom.DataAccessEfCore
{
    public class LoggingDomStorageRoutineHandler<TUserContext> : EfCoreStorageRoutineHandler<TUserContext, LoggingDomDbContext>
    {
        public LoggingDomStorageRoutineHandler(
            AdminkaStorageConfiguration adminkaStorageConfiguration,
            TUserContext userContext,
            Action<string> efDbContextVerbose,
            IHandler<RoutineClosure<TUserContext>> routineHandler,
            Func<TUserContext, string> getAudit) :
            this(
                LoggingDomDataAccessEfCoreManager.LoggingDomEntityMetaServiceContainer,
                userContext,
                () => LoggingDomDataAccessEfCoreManager.CreateLoggingDomDbContext(adminkaStorageConfiguration, efDbContextVerbose),
                routineHandler, getAudit)
        {
        }

        private LoggingDomStorageRoutineHandler(
            IEntityMetaServiceContainer entityMetaServiceContainer,
            TUserContext userContext,
            Func<LoggingDomDbContext> createDbContext,
            IHandler<RoutineClosure<TUserContext>> routineHandler,
            Func<TUserContext, string> getAudit) :
            base(
                entityMetaServiceContainer,
                createDbContext,
                () => new ValueTuple<LoggingDomDbContext, IAuditVisitor>(
                    createDbContext(),
                    new AuditVisitor<IVersioned>(
                        (e)=> { e.RowVersionAt = DateTime.Now; e.RowVersionBy = getAudit(userContext); })
                ),
                routineHandler)
        {
        }
    }

    public class LoggingDomStorageRoutineHandlerAsync<TUserContext> : EfCoreStorageRoutineHandlerAsync<TUserContext, LoggingDomDbContext>
    {
        public LoggingDomStorageRoutineHandlerAsync(
            AdminkaStorageConfiguration adminkaStorageConfiguration,
            TUserContext userContext,
            Action<string> efDbContextVerbose,
            IHandlerAsync<RoutineClosure<TUserContext>> routineHandler,
            Func<TUserContext, string> getAudit) :
            this(
                LoggingDomDataAccessEfCoreManager.LoggingDomEntityMetaServiceContainer,
                userContext,
                () => LoggingDomDataAccessEfCoreManager.CreateLoggingDomDbContext(adminkaStorageConfiguration, efDbContextVerbose),
                routineHandler, getAudit)
        {
        }

        private LoggingDomStorageRoutineHandlerAsync(
            IEntityMetaServiceContainer entityMetaServiceContainer,
            TUserContext userContext,
            Func<LoggingDomDbContext> createDbContext,
            IHandlerAsync<RoutineClosure<TUserContext>> routineHandler,
            Func<TUserContext, string> getAudit) :
            base(
                entityMetaServiceContainer,
                createDbContext,
                () => new ValueTuple<LoggingDomDbContext, IAuditVisitor>(
                    createDbContext(),
                    new AuditVisitor<IVersioned>(
                        (e) => { e.RowVersionAt = DateTime.Now; e.RowVersionBy = getAudit(userContext); })
                ),
                routineHandler)
        {
        }
    }
}