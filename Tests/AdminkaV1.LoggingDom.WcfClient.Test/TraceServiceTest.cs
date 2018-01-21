using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading.Tasks;

namespace DashboardCode.AdminkaV1.LoggingDom.WcfClient.Test
{
    /// <summary>
    /// Start web service DashboardCode.AdminkaV1.Wcf before testing
    /// </summary>
    [TestClass]
    public class TraceServiceTest 
    {
        [TestMethod]
        public void GetTrace()
        {
            var traceService = new TraceServiceProxy();
            var guid = Guid.NewGuid();
            try
            {
                var trace = traceService.GetTrace(guid);
            }
            catch (AdminkaException ex)
            {
                if (ex.Code != "TEST")
                    throw;
            }
        }

        [TestMethod]
        public void GetTraceAsync()
        {
            var traceService = new TraceServiceAsyncProxy();
            var guid = Guid.NewGuid();
            try
            {
                var task = traceService.GetTrace(guid);
                var trace = task.Result;
            }
            catch(AggregateException ex)
            {
                if (ex.InnerExceptions.Count > 0 && ex.InnerExceptions[0] is AdminkaException userContextException)
                {
                    if (userContextException.Code != "TEST")
                        throw;
                }
                else
                    throw;
            }
        }

        [TestMethod]
        public void GetTraceAsyncQueued()
        {
            var traceService = new TraceServiceAsyncProxy();
            var guid = Guid.NewGuid();
            Task.Run(async () =>
            {
                try
                {
                    var trace = await traceService.GetTrace(guid);
                }
                catch (AdminkaException ex)
                {
                    if (ex.Code != "TEST")
                        throw;
                }

            }).GetAwaiter().GetResult();
        }
    }
}