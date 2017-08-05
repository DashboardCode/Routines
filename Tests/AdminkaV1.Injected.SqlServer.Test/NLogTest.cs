using System;
using DashboardCode.Routines;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DashboardCode.AdminkaV1.Injected.SqlServer.Test
{
    [TestClass]
    public class NLogTest
    {

        [TestMethod]
        public virtual void TestNLogSuccess() // 161 ms
        {
            var routine = new AdminkaRoutine(new MemberTag(this), ZoningSharedSourceManager.GetConfiguration(), new { input="Input text" });
            var x = routine.Handle(container =>
            {
                container.Verbose("Test message");
                return "Output text";
            });
        }

        [TestMethod]
        public void TestNLogFailure() // 149 ms
        {
            var routine = new AdminkaRoutine(new MemberTag(this), ZoningSharedSourceManager.GetConfiguration(), new { input = "Input text" });
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
