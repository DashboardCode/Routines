using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Vse.Routines;

namespace Vse.AdminkaV1.Injected.Test
{
    [TestClass]
    public class NLogTest
    {
        [TestMethod]
        public void TestNLogSuccess() // 161 ms
        {
            var routine = new AdminkaRoutine(new RoutineTag(this), new { input="Input text" });
            var x = routine.Handle(container =>
            {
                container.Verbose("Test message");
                return "Output text";
            });
        }

        [TestMethod]
        public void TestNLogFailure() // 149 ms
        {
            var routine = new AdminkaRoutine(new RoutineTag(this), new { input = "Input text" });
            try
            { 
                var x = routine.Handle<string>(container =>
                {
                    container.Verbose("Test message");
                    throw new ApplicationException("Test exception");
                });
            }
            catch (ApplicationException)
            {

            }
        }
    }
}
