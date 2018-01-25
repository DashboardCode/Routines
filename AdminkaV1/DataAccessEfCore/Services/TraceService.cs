using System;

using DashboardCode.Routines.Storage;
using DashboardCode.AdminkaV1.LoggingDom;

namespace DashboardCode.AdminkaV1.DataAccessEfCore.Services
{
    public class TraceService : ITraceService
    {
        readonly IndependentDbContextHandler<UserContext, AdminkaDbContext> dbContextHandler;
        public TraceService(IndependentDbContextHandler<UserContext, AdminkaDbContext> dbContextHandler)
            => this.dbContextHandler = dbContextHandler;

        public Trace GetTrace(Guid correlationToken)
        {
            return dbContextHandler.Handle<Trace>(
                (dbContext,closure) =>
                {
                    throw new AdminkaDataAccessEfCoreException($"User exception from '{nameof(GetTrace)}'", closure,  "TEST");
                });
        }

        public void ResetTrace(Guid correlationToken)
        {
            dbContextHandler.Handle(
                (dbContext, closure) =>
                {
                    throw new AdminkaDataAccessEfCoreException($"User exception from '{nameof(ResetTrace)}'", closure, "TEST");
                });
        }
    }
}