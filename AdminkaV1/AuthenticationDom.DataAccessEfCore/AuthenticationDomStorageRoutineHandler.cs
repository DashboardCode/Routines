using System;
using DashboardCode.Routines;
using DashboardCode.Routines.Storage;
using DashboardCode.Routines.Storage.EfCore;

namespace DashboardCode.AdminkaV1.AuthenticationDom.DataAccessEfCore
{
    public class AuthenticationDomStorageRoutineHandler<TUserContext> : EfCoreStorageRoutineHandler<TUserContext, AuthenticationDomDbContext>
    {
        public AuthenticationDomStorageRoutineHandler(
            AdminkaStorageConfiguration adminkaStorageConfiguration,
            TUserContext userContext,

            Action<string> efDbContextVerbose,
            IHandler<RoutineClosure<TUserContext>> routineHandler,
            Func<TUserContext, string> getAudit) :
            this(
                AuthenticationDomDataAccessEfCoreManager.AuthenticationDomEntityMetaServiceContainer,
                userContext,
                () => AuthenticationDomDataAccessEfCoreManager.CreateAdminkaDbContext(adminkaStorageConfiguration, efDbContextVerbose),
                routineHandler, getAudit)
        {
        }

        private AuthenticationDomStorageRoutineHandler(
            IEntityMetaServiceContainer entityMetaServiceContainer,
            TUserContext userContext,
            Func<AuthenticationDomDbContext> createDbContext,
            IHandler<RoutineClosure<TUserContext>> routineHandler,
            Func<TUserContext, string> getAudit) :
            base(
                entityMetaServiceContainer,
                createDbContext,
                () => new ValueTuple<AuthenticationDomDbContext, IAuditVisitor>(
                    createDbContext(),
                    new AuditVisitor<IVersioned>(
                        (e)=> { e.RowVersionAt = DateTime.Now; e.RowVersionBy = getAudit(userContext); })
                ),
                routineHandler)
        {
        }
    }

    public class AuthenticationDomStorageRoutineHandlerAsync<TUserContext> : EfCoreStorageRoutineHandlerAsync<TUserContext, AuthenticationDomDbContext>
    {
        public AuthenticationDomStorageRoutineHandlerAsync(
            AdminkaStorageConfiguration adminkaStorageConfiguration,
            TUserContext userContext,

            Action<string> efDbContextVerbose,
            IHandlerAsync<RoutineClosure<TUserContext>> routineHandler,
            Func<TUserContext, string> getAudit) :
            this(
                AuthenticationDomDataAccessEfCoreManager.AuthenticationDomEntityMetaServiceContainer,
                userContext,
                () => AuthenticationDomDataAccessEfCoreManager.CreateAdminkaDbContext(adminkaStorageConfiguration, efDbContextVerbose),
                routineHandler, getAudit)
        {
        }

        private AuthenticationDomStorageRoutineHandlerAsync(
            IEntityMetaServiceContainer entityMetaServiceContainer,
            TUserContext userContext,
            Func<AuthenticationDomDbContext> createDbContext,
            IHandlerAsync<RoutineClosure<TUserContext>> routineHandler,
            Func<TUserContext, string> getAudit) :
            base(
                entityMetaServiceContainer,
                createDbContext,
                () => new ValueTuple<AuthenticationDomDbContext, IAuditVisitor>(
                    createDbContext(),
                    new AuditVisitor<IVersioned>(
                        (e) => { e.RowVersionAt = DateTime.Now; e.RowVersionBy = getAudit(userContext); })
                ),
                routineHandler)
        {
        }
    }
}