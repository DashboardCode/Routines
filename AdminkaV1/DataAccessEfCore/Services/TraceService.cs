using System;
using DashboardCode.AdminkaV1.DomLogging;

namespace DashboardCode.AdminkaV1.DataAccessEfCore.Services
{
    public class TraceService : ITraceService
    {
        readonly AdminkaDbContextHandler dbContextHandler;
        public TraceService(AdminkaDbContextHandler dbContextHandler)
        {
            this.dbContextHandler = dbContextHandler;
        }

        public Trace GetTrace(Guid correlationToken)
        {
            return dbContextHandler.Handle<Trace>(
                dbContext =>
                {
                    throw new UserContextException("");
                    //return new Trace(); 
                });
        }

        public void ResetTrace(Guid correlationToken)
        {
            dbContextHandler.Handle(
                dbContext =>
                {
                    throw new UserContextException("");
                    
                });
        }
    }
}
