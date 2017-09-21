using System;

namespace DashboardCode.AdminkaV1.LoggingDom
{
    public class ActivityRecord
    {
        public int ActivityRecordId {get;set;}
        public Guid CorrelationToken { get; set; }
        public string Application { get; set; }
        public string FullActionName { get; set; }
        public DateTime ActivityRecordLoggedAt { get; set; }
        public bool Successed { get; set; }
        public long DurationTicks { get; set; }
    }
}
