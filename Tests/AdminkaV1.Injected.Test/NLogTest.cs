﻿using System;
using Vse.Routines;
#if NETCOREAPP1_1
    using Xunit;
#else
using Microsoft.VisualStudio.TestTools.UnitTesting;
#endif 

namespace Vse.AdminkaV1.Injected.Test
{
#if !NETCOREAPP1_1
    [TestClass]
#endif
    public class NLogTest
    {
#if NETCOREAPP1_1
        ConfigurationNETStandard Configuration = new ConfigurationNETStandard();
#else
        ConfigurationNETFramework Configuration = new ConfigurationNETFramework();
#endif

#if NETCOREAPP1_1
        [Fact]
#else
        [TestMethod]
#endif
        public virtual void TestNLogSuccess() // 161 ms
        {
            var routine = new AdminkaRoutine(new RoutineTag(this), Configuration, new { input="Input text" });
            var x = routine.Handle(container =>
            {
                container.Verbose("Test message");
                return "Output text";
            });
        }

#if NETCOREAPP1_1
        [Fact]
#else
        [TestMethod]
#endif
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
                    throw ex;
            }
        }
    }
}
