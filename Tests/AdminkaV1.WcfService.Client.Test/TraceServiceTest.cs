using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DashboardCode.AdminkaV1.WcfService.Client.Test
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
            var traceService = new TraceServiceClient();
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
