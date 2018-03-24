using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using DashboardCode.Routines;

namespace DashboardCode.AdminkaV1.Injected.InMemory.Test
{
    [TestClass]
    public class NLogTest : RepositoryTestBase
    {
        [TestMethod]
        public virtual void TestNLogSuccess() // 161 ms
        {
            var logger = new List<string>();
            var routine = new AdminkaInMemoryTestRoutine(logger, new MemberTag(this), new { input = "Input text" }, readonlyDatabaseName);
            var x = routine.Handle(container =>
            {
                container.Verbose("Test message");
                return "Output text";
            });
        }

        [TestMethod]
        public void TestNLogFailure() // 149 ms
        {
            var logger = new List<string>();
            var routine = new AdminkaInMemoryTestRoutine(logger, new MemberTag(this), new { input = "Input text" }, readonlyDatabaseName);
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