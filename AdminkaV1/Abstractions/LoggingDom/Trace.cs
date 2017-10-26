using System;
using System.Collections.Generic;

namespace DashboardCode.AdminkaV1.LoggingDom
{
    public class Trace
    {
        public Guid CorrelationToken { get; set; }
        public List<Operation> Operations { get; set; }
    }
}
