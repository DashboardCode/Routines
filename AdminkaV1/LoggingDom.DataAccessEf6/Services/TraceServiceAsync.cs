using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Data.Entity;

namespace DashboardCode.AdminkaV1.LoggingDom.DataAccessEf6.Services
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

    public class TraceServiceAsync<TUserContext> : ITraceServiceAsync
    {
        readonly LoggingDomStorageRoutineHandlerAsync<TUserContext> storageRoutineHandler;
        public TraceServiceAsync(LoggingDomStorageRoutineHandlerAsync<TUserContext> storageRoutineHandler)
            => this.storageRoutineHandler = storageRoutineHandler;

        public async Task<List<VerboseRecord>> GetTraceAsync(Guid correlationToken)
        {
            return await storageRoutineHandler.HandleRepositoryAsync<List<VerboseRecord>, VerboseRecord>(async rep =>
            {
                return await rep.Query().Where(e => e.CorrelationToken == correlationToken).ToListAsync();
            });
        }
    }
}