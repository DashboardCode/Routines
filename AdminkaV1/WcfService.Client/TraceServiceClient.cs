﻿using System;
using System.ServiceModel;
using DashboardCode.AdminkaV1.DomLogging;
using DashboardCode.AdminkaV1.WcfService.Client.TraceServiceReference;

namespace DashboardCode.AdminkaV1.WcfService.Client
{
    // TODO: this code can be generated with T4
    public class TraceServiceClient : ITraceService
    {
        public DomLogging.Trace GetTrace(Guid correlationToken)
        {
            var client = new TraceServiceReference.TraceServiceClient();
            try
            {
                return client.GetTrace(correlationToken);
            }
            catch (FaultException<RoutineError> ex)
            {
                var baseException = (Exception)ex;
                if (ex.Detail.UserContextExceptionCode != null)
                    baseException = new UserContextException(ex.Message, ex, ex.Detail.UserContextExceptionCode);
                baseException.Data["RemoteUserContextExceptionCode"] = ex.Detail.UserContextExceptionCode;
                baseException.Data["RemoteCorrelationToken"] = ex.Detail.RoutineTag.CorrelationToken;
                baseException.Data["RemoteNamespace"] = ex.Detail.RoutineTag.Namespace;
                baseException.Data["RemoteType"] = ex.Detail.RoutineTag.Type;
                baseException.Data["RemoteMember"] = ex.Detail.RoutineTag.Member;
                baseException.Data["RemoteDetails"] = ex.Detail.Details;
                if (baseException != ex)
                    throw baseException;
                throw;
            }
        }
    }
}
