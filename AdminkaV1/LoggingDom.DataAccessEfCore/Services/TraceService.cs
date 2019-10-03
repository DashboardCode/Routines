using System;
using System.Linq;
using System.Collections.Generic;

namespace DashboardCode.AdminkaV1.LoggingDom.DataAccessEfCore.Services
{
    public class TraceService<TUserContext> : ITraceService
    {
        readonly LoggingDomStorageRoutineHandler<TUserContext> storageRoutineHandler;
        public TraceService(LoggingDomStorageRoutineHandler<TUserContext> storageRoutineHandler)
            => this.storageRoutineHandler = storageRoutineHandler;

        public List<VerboseRecord> GetTrace(Guid correlationToken)
        {
            return storageRoutineHandler.HandleRepository<List<VerboseRecord>, VerboseRecord>(rep =>
            {
                return rep.Query().Where(e => e.CorrelationToken == correlationToken).ToList();
            });
        }
    }
}