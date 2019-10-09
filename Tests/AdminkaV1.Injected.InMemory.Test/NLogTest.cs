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
            var routine = new AdminkaInMemoryTestRoutine(
                logger,
                hasVerboseLoggingPrivilege: true,
                new MemberTag(this), 

                new { input = "Input text" }, readonlyDatabaseName);
            routine.Handle((container, closure) => container.ResolveTestDomDbContextHandler().HandleOrmFactory(ormHandlersFactory =>
            {
                closure.Verbose("Test message");
                return "Output text";
            }));
        }

        [TestMethod]
        public void TestNLogFailure() // 149 ms
        {
            var logger = new List<string>();
            var routine = new AdminkaInMemoryTestRoutine(logger, hasVerboseLoggingPrivilege: true, new MemberTag(this), new { input = "Input text" }, readonlyDatabaseName);
            try
            {
                var x = routine.Handle((container, closure) => container.ResolveTestDomDbContextHandler().HandleOrmFactory(ormHandlersFactory =>
                {
                    closure.Verbose("Test message");
                    throw new Exception("Test exception");
#pragma warning disable CS0162 // Unreachable code detected
                    return 0;
#pragma warning restore CS0162 // Unreachable code detected
                }));
            }
            catch (Exception ex)
            {
                if (ex.Message != "Test exception")
                    throw;
            }
        }
    }
}