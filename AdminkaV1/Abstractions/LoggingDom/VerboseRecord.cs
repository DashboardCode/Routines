using System;

namespace DashboardCode.AdminkaV1.LoggingDom
{
    public class VerboseRecord
    {
        public int ActivityRecordId { get; set; }
        public Guid CorrelationToken { get; set; }
        public string Application { get; set; }
        public string FullActionName { get; set; }
        public DateTime VerboseRecordLoggedAt { get; set; }
        public string VerboseRecordTypeId { get; set; }
        public string VerboseRecordMessage { get; set; }
    }
}
