using System;
using System.Collections.Generic;

namespace DashboardCode.AdminkaV1.DomLogging
{
    public class Trace
    {
        Guid CorrelationToken { get; set; }
        public List<Operation> Operations { get; set; }
    }
}
