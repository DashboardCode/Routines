using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Vse.AdminkaV1.Wcf.Client.Test
{
    /// <summary>
    /// Start web service Vse.AdminkaV1.Wcf before testing
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
