using System;

using DashboardCode.Routines;

namespace DashboardCode.AdminkaV1.LoggingDom.DataAccessEfCore.Services
{
    public class TraceService : ITraceService
    {
        readonly RoutineDisposeHandler<LoggingDomDbContext, UserContext> dbContextHandler;
        public TraceService(RoutineDisposeHandler<LoggingDomDbContext, UserContext> dbContextHandler)
            => this.dbContextHandler = dbContextHandler;

        public Trace GetTrace(Guid correlationToken)
        {
            return dbContextHandler.Handle<Trace>(
                (dbContext,closure) =>
                    throw new LoggingDomDataAccessEfCoreException($"User exception from '{nameof(GetTrace)}'", closure,  "TEST")
                );
        }

        public void ResetTrace(Guid correlationToken)
        {
            dbContextHandler.Handle(
                (dbContext, closure) =>
                    throw new LoggingDomDataAccessEfCoreException($"User exception from '{nameof(ResetTrace)}', guid '{correlationToken}'", closure, "TEST")
                );
        }
    }
}