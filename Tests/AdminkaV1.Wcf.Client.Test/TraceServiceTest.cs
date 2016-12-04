using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Vse.AdminkaV1.Wcf.Client;

namespace Vse.AdminkaV1.Wcf.Client.Test
{
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
