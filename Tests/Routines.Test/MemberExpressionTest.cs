using System;
using System.Linq.Expressions;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DashboardCode.Routines.Test
{
    [TestClass]
    public class MemberExpressionTest
    {
        private class TestClass
        {
            public int Property { get; set; }
        }

        [TestMethod]
        public void CompileSettterWithConverter()
        {
            Expression<Func<TestClass, int>> expression = (e) => e.Property;
            var memberExpression = (MemberExpression)expression.Body;
            Func<string, int> converter = (s) => int.Parse(s);
            Func<TestClass, Action<string>> func = memberExpression.CompileSettterWithConverter<TestClass, string, int>(converter);

            var t = new TestClass();
            var action = func(t);
            action("10");

            if (t.Property != 10)
                throw new Exception(nameof(CompileSettterWithConverter));
        }
    }
}
