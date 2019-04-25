using System;

namespace DashboardCode.Routines.AspNetCore
{
    public class AspRoutineFeature
    {
        public Guid CorrelationToken { get; set; }
        public string AspRequestId { get; set; }
        public TraceDocument TraceDocument { get; set; }
    }
}