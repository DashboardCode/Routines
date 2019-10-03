using System;
using DashboardCode.Routines;
using DashboardCode.Routines.Storage;
using DashboardCode.Routines.Storage.Ef6;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DashboardCode.AdminkaV1.LoggingDom.DataAccessEf6
{
    public class LoggingDomStorageRoutineHandler<TUserContext> : Ef6StorageRoutineHandler<TUserContext, LoggingDomDbContext>
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
                        (e) => { e.RowVersionAt = DateTime.Now; e.RowVersionBy = getAudit(userContext); })
                ),
                routineHandler)
        {
        }
    }
}
