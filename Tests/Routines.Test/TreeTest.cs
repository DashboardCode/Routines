using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;

namespace Vse.Routines.Test
{
    public class XNodePrimal
    {
        public Dictionary<string, XNode> Children = new Dictionary<string, XNode>();
        public string tag;
        public XNodePrimal(string tag)
        {
            this.tag = tag;
        }

        public XNode AddChild(string key, string tag, int val1)
        {
            var s = new XNode(key, tag, val1);
            Children.Add(key, s);
            return s;
        }
    }

    public class XNode : XNodePrimal
    {
        public string key;
        public int val1;
        public XNode(string key, string tag, int val1) : base(tag)
        {
            this.key = key;
            this.val1 = val1;
        }
    }

    public class XTree: Tree<XNodePrimal, XNode, string>{

        public static readonly XTree Instance = new XTree();

        private XTree(): base(
                (t) => t.Children.Values,
                (t) => t.key,
                (t, n) => {
                    XNode c = null;
                    t.Children.TryGetValue(n, out c); return c;
                },
                (t) => new XNodePrimal(t.tag),
                (t, p) => { var t2 = new XNode(t.key, t.tag, t.val1); p.Children.Add(t.key, t2); return t2; }
            )
        {

        }
    }

    [TestClass]
    public class TreeTest
    {
        XNodePrimal x1;
        XNodePrimal x2;
        XNodePrimal x3;
        
        public TreeTest()
        {
            x1 = new XNodePrimal("head");
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

            x2 = new XNodePrimal("head");
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

            x3 = new XNodePrimal("head");
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
            var x1cloned = XTree.Instance.Clone(x1);
            var b1 = XTree.Instance.IsSubsetOf(x1, x1cloned);
            var b2 = XTree.Instance.IsSupersetOf(x1, x1cloned);
            var b3 = XTree.Instance.IsEqualTo(x1, x1cloned);
            var b4 = XTree.Instance.IsEqualTo(x1cloned, x1);
            if (b1 == false || b2 == false || b3 == false || b4 == false)
                throw new Exception("cloned logical operation fails");
        }

        [TestMethod]
        public void SNodeLogicalTest()
        {
            var b1 = XTree.Instance.IsSupersetOf(x1, x2);
            var b2 = XTree.Instance.IsSubsetOf(x2, x1);

            var b3 = XTree.Instance.IsSubsetOf(x1, x2);
            var b4 = XTree.Instance.IsSupersetOf(x2, x1);

            var b5 = XTree.Instance.IsEqualTo(x1, x2);
            var b6 = XTree.Instance.IsEqualTo(x2, x1);

            if (b1 == false || b2 == false || b3 == true || b4 == true || b5 == true || b6 == true)
                throw new Exception("logical operation fails A");

            var b1b = XTree.Instance.IsSupersetOf(x1, x3);
            var b2b = XTree.Instance.IsSubsetOf(x3, x1);

            var b3b = XTree.Instance.IsSubsetOf(x1, x3);
            var b4b = XTree.Instance.IsSupersetOf(x3, x1);

            if (b1b == true || b2b == true || b3b == true || b4b == true)
                throw new Exception("logical operation fails B");

            var b1c = XTree.Instance.IsSupersetOf(x3, x2);
            var b2c = XTree.Instance.IsSubsetOf(x2, x3);

            var b3c = XTree.Instance.IsSubsetOf(x3, x2);
            var b4c = XTree.Instance.IsSupersetOf(x2, x3);

            if (b1c == false || b2c == false || b3c == true || b4c == true)
                throw new Exception("logical operation fails C");

        }

        [TestMethod]
        public void SNodeUnionTest()
        {
            var z1 = XTree.Instance.Union(x1, x2);
            var z2 = XTree.Instance.Union(x2, x1);
            var z3 = XTree.Instance.Union(z1, x3);

            var b1 = XTree.Instance.IsEqualTo(z1, z2);
            var b2 = XTree.Instance.IsEqualTo(z2, z1);
            if (b1 == false || b2 == false)
                throw new Exception("union equals fails");

            var b3 = XTree.Instance.IsEqualTo(z1, x1);

            var q1 = XTree.Instance.IsSupersetOf(z3, x1);
            var q2 = XTree.Instance.IsSupersetOf(z3, x2);
            var q3 = XTree.Instance.IsSupersetOf(z3,x3);
            if (q1 == false || q2 == false || q3 == false)
                throw new Exception("union superset fails");
        }

        [TestMethod]
        public void SNodeChainLists()
        {
            var nodes1 = XTree.Instance.GetTreeAsListOfPaths(x1);
            var nodes2 = XTree.Instance.GetTreeAsListOfPaths(x2);
            var nodes3 = XTree.Instance.GetTreeAsListOfPaths(x3);
            if (nodes1.Count != 5 || nodes2.Count != 4 || nodes3.Count != 5)
                throw new Exception("ToAncestorsAndSelfChains fails");
        }
    }
}

