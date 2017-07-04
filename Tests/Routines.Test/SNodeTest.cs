using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.IO;

namespace Vse.Routines.Test
{
    [TestClass]
    public class SNodeTest
    {
        SNodeBase<string, int> x1;
        SNodeBase<string, int> x2;
        SNodeBase<string, int> x3;
        public SNodeTest()
        {
            x1 = new SNodeBase<string, int>("head");
            {
                var a1 = x1.AddChild("a1", "child", 1);
                var b1 = a1.AddChild("b1", "child", 1);
                var c1 = b1.AddChild("c1", "child", 1);
                var b2 = a1.AddChild("b2", "child", 2);
                var b3 = a1.AddChild("b3", "child", 3);
                var b4 = a1.AddChild("b4", "child", 4);
                var a2 = x1.AddChild("a2", "child", 2);
                //var a3 = x1.AddChild("a3", "child", 3);
            }

            x2 = new SNodeBase<string, int>("head");
            {
                var a1 = x2.AddChild("a1", "child", 1);
                var b1 = a1.AddChild("b1", "child", 1);
                //var c1 = b1.AddChild("c1", "child", 1);
                //var b2 = a1.AddChild("b2", "child", 2);
                var b3 = a1.AddChild("b3", "child", 3);
                var b4 = a1.AddChild("b4", "child", 4);
                var a2 = x2.AddChild("a2", "child", 2);
                //var a3 = x2.AddChild("a3", "child", 3);
            }

            x3 = new SNodeBase<string, int>("head");
            {
                var a1 = x3.AddChild("a1", "child", 1);
                var b1 = a1.AddChild("b1", "child", 1);
                //var c1 = b1.AddChild("c1", "child", 1);
                //var b2 = a1.AddChild("b2", "child", 2);
                var b3 = a1.AddChild("b3", "child", 3);
                var b4 = a1.AddChild("b4", "child", 4);
                var a2 = x3.AddChild("a2", "child", 2);
                var a3 = x3.AddChild("a3", "child", 3);
            }
        }

        [TestMethod]
        public void SNodeCloneTest()
        {
            var x1cloned = x1.Clone();
            var b1 = x1.IsSubsetOf(x1cloned);
            var b2 = x1.IsSupersetOf(x1cloned);
            var b3 = x1.IsEquals(x1cloned);
            var b4 = x1cloned.IsEquals(x1);
            if (b1 == false || b2 == false || b3 == false || b4 == false)
                throw new Exception("cloned logical operation fails");
        }

        [TestMethod]
        public void SNodeLogicalTest()
        {
            var b1 = x1.IsSupersetOf(x2);
            var b2 = x2.IsSubsetOf(x1);

            var b3 = x1.IsSubsetOf(x2);
            var b4 = x2.IsSupersetOf(x1);

            var b5 = x1.IsEquals(x2);
            var b6 = x2.IsEquals(x1);

            if (b1 == false || b2 == false || b3 == true || b4 == true || b5 == true || b6 == true)
                throw new Exception("logical operation fails A");

            var b1b = x1.IsSupersetOf(x3);
            var b2b = x3.IsSubsetOf(x1);

            var b3b = x1.IsSubsetOf(x3);
            var b4b = x3.IsSupersetOf(x1);

            if (b1b == true || b2b == true || b3b == true || b4b == true)
                throw new Exception("logical operation fails B");

            var b1c = x3.IsSupersetOf(x2);
            var b2c = x2.IsSubsetOf(x3);

            var b3c = x3.IsSubsetOf(x2);
            var b4c = x2.IsSupersetOf(x3);

            if (b1c == false || b2c == false || b3c == true || b4c == true)
                throw new Exception("logical operation fails C");

        }

        [TestMethod]
        public void SNodeUnionTest()
        {
            var z1 = x1.Union(x2);
            var z2 = x2.Union(x1);
            var z3 = z1.Union(x3);

            var b1 = z1.IsEquals(z2);
            var b2 = z2.IsEquals(z1);
            if (b1 == false || b2 == false)
                throw new Exception("union equals fails");

            var b3 = z1.IsEquals(x1);

            var q1 = z3.IsSupersetOf(x1);
            var q2 = z3.IsSupersetOf(x2);
            var q3 = z3.IsSupersetOf(x3);
            if (q1 == false || q2 == false || q3 == false)
                throw new Exception("union superset fails");
        }

        [TestMethod]
        public void SNodeChainLists()
        {
            var nodes1 = x1.ToChainPathes();
            var nodes2 = x2.ToChainPathes();
            var nodes3 = x3.ToChainPathes();
            if (nodes1.Count != 5 || nodes2.Count != 4 || nodes3.Count != 5)
                throw new Exception("ToAncestorsAndSelfChains fails");
        }
    }
}

