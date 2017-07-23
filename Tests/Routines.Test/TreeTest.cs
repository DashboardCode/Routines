using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Linq;
using System.Collections.Generic;

namespace DashboardCode.Routines.Test
{
    public class XNode
    {
        public Dictionary<string, XNode> Children = new Dictionary<string, XNode>();
        public string key;
        public XNode Parent;
        public XNode(string key)
        {
            this.key = key;
        }

        public XNode AddChild(string key)
        {
            var n = new XNode(key);
            n.Parent = this;
            Children.Add(key, n);
            return n;
        }
    }

    public class SNodePrimal
    {
        public Dictionary<string, SNode> Children = new Dictionary<string, SNode>();
        public string tag;
        public SNodePrimal(string tag)
        {
            this.tag = tag;
        }

        public SNode AddChild(string key, string tag, int val1)
        {
            var s = new SNode(key, tag, val1);
            s.Parent = this;
            Children.Add(key, s);
            return s;
        }
    }

    public class SNode : SNodePrimal
    {
        public string key;
        public int val1;

        public SNodePrimal Parent;

        public SNode(string key, string tag, int val1) : base(tag)
        {
            this.key = key;
            this.val1 = val1;
        }
    }

    public class STree: Tree<SNodePrimal, SNode, string>{

        public static readonly STree Instance = new STree();

        private STree(): base(
                (t) => t.Children.Values,
                (t) => t.key,
                (t, n) => {
                    SNode c = null;
                    t.Children.TryGetValue(n, out c); return c;
                },
                (t) => new SNodePrimal(t.tag),
                (t, p) => { var t2 = new SNode(t.key, t.tag, t.val1); p.Children.Add(t.key, t2); return t2; }
            )
        {

        }
    }

    public class XTree : Tree<XNode, XNode, string>
    {
        public static readonly XTree Instance = new XTree();

        private XTree() : base(
                (t) => t.Children.Values,
                (t) => t.key,
                (t, n) => {
                    XNode c = null;
                    t.Children.TryGetValue(n, out c); return c;
                },
                (t) => new XNode(null),
                (t, p) => { var t2 = new XNode(t.key); p.Children.Add(t.key, t2); return t2; }
            )
        {

        }
    }

    [TestClass]
    public class TreeTest
    {
        SNodePrimal s1;
        SNodePrimal s2;
        SNodePrimal s3;

        XNode x1;

        public TreeTest()
        {
            x1 = new XNode(null);
            {
                var a1 = x1.AddChild("a1");
                var b1 = a1.AddChild("b1");
                var c1 = b1.AddChild("c1");
                var b2 = a1.AddChild("b2");
                var b3 = a1.AddChild("b3");
                var b4 = a1.AddChild("b4");
                var a2 = x1.AddChild("a2");
            }

            s1 = new SNodePrimal("head");
            {
                var a1 = s1.AddChild("a1", "child", 1);
                var b1 = a1.AddChild("b1", "child", 1);
                var c1 = b1.AddChild("c1", "child", 1);
                var b2 = a1.AddChild("b2", "child", 2);
                var b3 = a1.AddChild("b3", "child", 3);
                var b4 = a1.AddChild("b4", "child", 4);
                var a2 = s1.AddChild("a2", "child", 2);
                //var a3 = x1.AddChild("a3", "child", 3);
            }

            s2 = new SNodePrimal("head");
            {
                var a1 = s2.AddChild("a1", "child", 1);
                var b1 = a1.AddChild("b1", "child", 1);
                //var c1 = b1.AddChild("c1", "child", 1);
                //var b2 = a1.AddChild("b2", "child", 2);
                var b3 = a1.AddChild("b3", "child", 3);
                var b4 = a1.AddChild("b4", "child", 4);
                var a2 = s2.AddChild("a2", "child", 2);
                //var a3 = x2.AddChild("a3", "child", 3);
            }

            s3 = new SNodePrimal("head");
            {
                var a1 = s3.AddChild("a1", "child", 1);
                var b1 = a1.AddChild("b1", "child", 1);
                //var c1 = b1.AddChild("c1", "child", 1);
                //var b2 = a1.AddChild("b2", "child", 2);
                var b3 = a1.AddChild("b3", "child", 3);
                var b4 = a1.AddChild("b4", "child", 4);
                var a2 = s3.AddChild("a2", "child", 2);
                var a3 = s3.AddChild("a3", "child", 3);
            }
        }

        [TestMethod]
        public void SNodeCloneTest()
        {
            var x1cloned = STree.Instance.Clone(s1);
            var b1 = STree.Instance.IsSubsetOf(s1, x1cloned);
            var b2 = STree.Instance.IsSupersetOf(s1, x1cloned);
            var b3 = STree.Instance.IsEqualTo(s1, x1cloned);
            var b4 = STree.Instance.IsEqualTo(x1cloned, s1);
            if (b1 == false || b2 == false || b3 == false || b4 == false)
                throw new Exception("cloned logical operation fails");
        }

        [TestMethod]
        public void SNodeLogicalTest()
        {
            var b1 = STree.Instance.IsSupersetOf(s1, s2);
            var b2 = STree.Instance.IsSubsetOf(s2, s1);

            var b3 = STree.Instance.IsSubsetOf(s1, s2);
            var b4 = STree.Instance.IsSupersetOf(s2, s1);

            var b5 = STree.Instance.IsEqualTo(s1, s2);
            var b6 = STree.Instance.IsEqualTo(s2, s1);

            if (b1 == false || b2 == false || b3 == true || b4 == true || b5 == true || b6 == true)
                throw new Exception("logical operation fails A");

            var b1b = STree.Instance.IsSupersetOf(s1, s3);
            var b2b = STree.Instance.IsSubsetOf(s3, s1);

            var b3b = STree.Instance.IsSubsetOf(s1, s3);
            var b4b = STree.Instance.IsSupersetOf(s3, s1);

            if (b1b == true || b2b == true || b3b == true || b4b == true)
                throw new Exception("logical operation fails B");

            var b1c = STree.Instance.IsSupersetOf(s3, s2);
            var b2c = STree.Instance.IsSubsetOf(s2, s3);

            var b3c = STree.Instance.IsSubsetOf(s3, s2);
            var b4c = STree.Instance.IsSupersetOf(s2, s3);

            if (b1c == false || b2c == false || b3c == true || b4c == true)
                throw new Exception("logical operation fails C");

        }

        [TestMethod]
        public void SNodeUnionTest()
        {
            var z1 = STree.Instance.Union(s1, s2);
            var z2 = STree.Instance.Union(s2, s1);
            var z3 = STree.Instance.Union(z1, s3);

            var b1 = STree.Instance.IsEqualTo(z1, z2);
            var b2 = STree.Instance.IsEqualTo(z2, z1);
            if (b1 == false || b2 == false)
                throw new Exception("union equals fails");

            var b3 = STree.Instance.IsEqualTo(z1, s1);

            var q1 = STree.Instance.IsSupersetOf(z3, s1);
            var q2 = STree.Instance.IsSupersetOf(z3, s2);
            var q3 = STree.Instance.IsSupersetOf(z3,s3);
            if (q1 == false || q2 == false || q3 == false)
                throw new Exception("union superset fails");
        }

        [TestMethod]
        public void SNodeChainLists()
        {
            var nodes1 = STree.Instance.GetTreeAsListOfPaths(s1);
            var nodes2 = STree.Instance.GetTreeAsListOfPaths(s2);
            var nodes3 = STree.Instance.GetTreeAsListOfPaths(s3);
            if (nodes1.Count != 5 || nodes2.Count != 4 || nodes3.Count != 5)
                throw new Exception("ToAncestorsAndSelfChains fails");
        }

        [TestMethod]
        public void SNodeChainPaths()
        {
            var n = s1.Children.Values.First()
                      .Children.Values.First()
                      .Children.Values.First();

            var p1 = STree.Instance.GetXPathOfNode(n, i=>i.Parent);

            if (p1 != "/a1/b1/c1")
                throw new Exception("SNodeChainPaths 1");

            var p2 = STree.Instance.GetPathOfNode(n, i => i.Parent);
            var p3 = STree.Instance.GetTreeAsXPathUnion(p2);

            if (p3 != "/a1/b1/c1")
                throw new Exception("SNodeChainPaths 2");
        }

        [TestMethod]
        public void XNodeChainPaths()
        {
            var n = x1.Children.Values.First()
                      .Children.Values.First()
                      .Children.Values.First();

            var p1 = XTree.Instance.GetXPathOfNode(n, i => i.Parent);

            if (p1 != "/a1/b1/c1")
                throw new Exception("SNodeChainPaths 1");

            var p2 = XTree.Instance.GetPathOfNode(n, i => i.Parent);
            var p3 = XTree.Instance.GetTreeAsXPathUnion(p2);

            if (p3 != "/a1/b1/c1")
                throw new Exception("SNodeChainPaths 2");

            var p4 = XTree.Instance.GetXPathOfNode(x1, i => i.Parent);
            if (p4 != "/")
                throw new Exception("SNodeChainPaths 3");

            var p5 = XTree.Instance.GetPathOfNode(x1, i => i.Parent);

            var p6 = XTree.Instance.GetTreeAsXPathUnion(p5);
            if (p4 != "/")
                throw new Exception("SNodeChainPaths 4");
        }
    }
}

