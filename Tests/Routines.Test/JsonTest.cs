using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vse.Routines.Text;

namespace Vse.Routines.Test
{
    [TestClass]
    public class UnitTest
    {
        [TestMethod]
        public void JsonSerializeTest()
        {
            var t = TestTool.CreateTestModel();
            var includes = TestTool.CreateInclude();
            //var cloned = MemberExpressionExtensions.Clone(t, includes);
            var json = RoutineSerializer.ToJson(t, includes);
        }
    }
}
