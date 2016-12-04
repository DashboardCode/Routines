using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace Vse.Routines.Configuration.Test
{
    [TestClass]
    public class ConfigurationUnitTest
    {
        [TestMethod]
        public void ReadConfiguration()
        {
            var container = new State("ConfigurationUnitTest", "ReadConfiguration");
            var t1 = container.Resolve<LoggingConfiguration>();
            var t2 = container.Resolve<LoggingPerformanceConfiguration>();
            if (!(t1.Output == false && t2.ThresholdSec==(decimal)0.1))
                throw new ApplicationException("Test fails");

            var containerS = new State("ConfigurationUnitTest", "ReadConfiguration", "superuser");
            var t1s = containerS.Resolve<LoggingConfiguration>();
            var t2s = containerS.Resolve<LoggingPerformanceConfiguration>();
            if (!(t1s.Output == true && t2s.ThresholdSec == (decimal)0.5))
                throw new ApplicationException("Test fails");
        }
    }
}
