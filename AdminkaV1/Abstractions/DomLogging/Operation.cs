using System.Collections.Generic;

namespace DashboardCode.AdminkaV1.DomLogging
{
    public class Operation
    {
        public string Application { get; set; }
        public string OperationName { get; set; }
        public string Input { get; set; }
        public string Output { get; set; }
        public string StartActivityRecord { get; set; }
        public string FinishActivityRecord { get; set; }
        public IReadOnlyCollection<VerboseRecord> VerboseRecord { get; set; }
    }
}
