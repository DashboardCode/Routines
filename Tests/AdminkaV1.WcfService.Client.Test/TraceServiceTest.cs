using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using DashboardCode.AdminkaV1.WcfClient;

namespace DashboardCode.AdminkaV1.WcfApp.Messaging.Client.Test
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
            var traceService = new TraceServiceWcfClient();
            var guid = Guid.NewGuid();
            try
            {
                var x = traceService.GetTrace(guid);
            }
            catch (UserContextException)
            {

            }
        }
    }
}
