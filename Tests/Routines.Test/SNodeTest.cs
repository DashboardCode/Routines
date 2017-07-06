using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;

namespace Vse.Routines.Test
{
    public class SNodeBase
    {
        public Dictionary<string, SNode> Children = new Dictionary<string, SNode>();
        public string tag;
        public SNodeBase(string tag)
        {
            this.tag = tag;
        }

        public SNode AddChild(string key, string tag, int val1)
        {
            var s = new SNode(key, tag, val1);
            Children.Add(key, s);
            return s;
        }
    }

    public class SNode : SNodeBase
    {
        public string key;
        public int val1;
        public SNode(string key, string tag, int val1) : base(tag)
        {
            this.key = key;
            this.val1 = val1;
        }
    }

    public class TestTree: STree<SNodeBase, SNode, string>{
        public TestTree(): base(
                (t) => t.Children.Values, 
                (t) => new SNodeBase(t.tag),
                (t, p) => { var t2 = new SNode(t.key, t.tag, t.val1); p.Children.Add(t.key, t2); return t2; }, 
                (t) => t.key,
                (t,n) => {
                    SNode c = null;
                    t.Children.TryGetValue(n, out c); return c;
                }
            )
        {

        }
    }

    [TestClass]
    public class SNodeTest
    {
        SNodeBase x1;
        SNodeBase x2;
        SNodeBase x3;
        TestTree testTree = new TestTree();
        public SNodeTest()
        {
            x1 = new SNodeBase("head");
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

            x2 = new SNodeBase("head");
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

            x3 = new SNodeBase("head");
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
            var x1cloned = testTree.Clone(x1);
            var b1 = testTree.IsSubsetOf(x1, x1cloned);
            var b2 = testTree.IsSupersetOf(x1, x1cloned);
            var b3 = testTree.IsEquals(x1, x1cloned);
            var b4 = testTree.IsEquals(x1cloned, x1);
            if (b1 == false || b2 == false || b3 == false || b4 == false)
                throw new Exception("cloned logical operation fails");
        }

        [TestMethod]
        public void SNodeLogicalTest()
        {
            var b1 = testTree.IsSupersetOf(x1, x2);
            var b2 = testTree.IsSubsetOf(x2, x1);

            var b3 = testTree.IsSubsetOf(x1, x2);
            var b4 = testTree.IsSupersetOf(x2, x1);

            var b5 = testTree.IsEquals(x1, x2);
            var b6 = testTree.IsEquals(x2, x1);

            if (b1 == false || b2 == false || b3 == true || b4 == true || b5 == true || b6 == true)
                throw new Exception("logical operation fails A");

            var b1b = testTree.IsSupersetOf(x1, x3);
            var b2b = testTree.IsSubsetOf(x3, x1);

            var b3b = testTree.IsSubsetOf(x1, x3);
            var b4b = testTree.IsSupersetOf(x3, x1);

            if (b1b == true || b2b == true || b3b == true || b4b == true)
                throw new Exception("logical operation fails B");

            var b1c = testTree.IsSupersetOf(x3, x2);
            var b2c = testTree.IsSubsetOf(x2, x3);

            var b3c = testTree.IsSubsetOf(x3, x2);
            var b4c = testTree.IsSupersetOf(x2, x3);

            if (b1c == false || b2c == false || b3c == true || b4c == true)
                throw new Exception("logical operation fails C");

        }

        [TestMethod]
        public void SNodeUnionTest()
        {
            var z1 = testTree.Union(x1, x2);
            var z2 = testTree.Union(x2, x1);
            var z3 = testTree.Union(z1, x3);

            var b1 = testTree.IsEquals(z1, z2);
            var b2 = testTree.IsEquals(z2, z1);
            if (b1 == false || b2 == false)
                throw new Exception("union equals fails");

            var b3 = testTree.IsEquals(z1, x1);

            var q1 = testTree.IsSupersetOf(z3, x1);
            var q2 = testTree.IsSupersetOf(z3, x2);
            var q3 = testTree.IsSupersetOf(z3,x3);
            if (q1 == false || q2 == false || q3 == false)
                throw new Exception("union superset fails");
        }

        [TestMethod]
        public void SNodeChainLists()
        {
            var nodes1 = testTree.ToChainPathes(x1);
            var nodes2 = testTree.ToChainPathes(x2);
            var nodes3 = testTree.ToChainPathes(x3);
            if (nodes1.Count != 5 || nodes2.Count != 4 || nodes3.Count != 5)
                throw new Exception("ToAncestorsAndSelfChains fails");
        }
    }
}

