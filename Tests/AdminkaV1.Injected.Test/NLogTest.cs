using System;
using DashboardCode.Routines;
using Microsoft.VisualStudio.TestTools.UnitTesting;
#if NETCOREAPP1_1
    using DashboardCode.AdminkaV1.Injected.NETStandard.Test;
#else
    using DashboardCode.AdminkaV1.Injected.NETFramework.Test;
#endif 

namespace DashboardCode.AdminkaV1.Injected.Test
{
    [TestClass]
    public class NLogTest
    {
#if NETCOREAPP1_1
        ConfigurationNETStandard Configuration = new ConfigurationNETStandard();
#else
        ConfigurationNETFramework Configuration = new ConfigurationNETFramework();
#endif

        [TestMethod]
        public virtual void TestNLogSuccess() // 161 ms
        {
            var routine = new AdminkaRoutine(new RoutineTag(this), Configuration, new { input="Input text" });
            var x = routine.Handle(container =>
            {
                container.Verbose("Test message");
                return "Output text";
            });
        }

        [TestMethod]
        public void TestNLogFailure() // 149 ms
        {
            var routine = new AdminkaRoutine(new RoutineTag(this), Configuration, new { input = "Input text" });
            try
            { 
                var x = routine.Handle<string>(container =>
                {
                    container.Verbose("Test message");
                    throw new Exception("Test exception");
                });
            }
            catch (Exception ex)
            {
                if (ex.Message != "Test exception")
                    throw;
            }
        }
    }
}
