﻿using System;
using System.Collections.Generic;

using DashboardCode.AdminkaV1.LoggingDom;

namespace DashboardCode.AdminkaV1.Injected.WcfApp
{
    // TODO: Generate using T4
    public class TraceService : ITraceService
    {
        public List<VerboseRecord> GetTrace(Guid searchForCorrelationToken)
        {
            var routine = new WcfRoutineAsync(new Routines.MemberTag(this), RoutineErrorDataContractConstants.FaultCodeNamespace, new { searchForCorrelationToken });
            return routine.HandleAsync(async (container, closure) => await container.ResolveTraceServiceAsync().GetTraceAsync(searchForCorrelationToken)).Result; // TODO: block on unclear context?
        }
    }
}