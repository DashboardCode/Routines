using System.Collections.Generic;
using System.Linq;
using System.Text;
using System;

namespace Vse.Routines
{
    public class SSNodeRoot
    {
        public List<SSNode> SSNodes { get; set; } 
    }

    public class SSNode: SSNodeRoot
    {
        public string Key { get; set; }
    }

    // var getKey = lnode=>lnode.Key;
    // var getChilds  = lnode=>LNodes;
    public class LNode
    {
        public List<LNode> LNodes { get; set; }
        public string Key { get; set; }
    }

    public class Tree<TNode, TName>
    {
        Dictionary<TNode, TNode> dictionary = new Dictionary<TNode /*child*/, TNode /* parent*/>();
        public Tree(Func<TNode, IEnumerable<TNode>> getChildren, Func<TNode, TName> getKey)
        {

        }
        
        public TNode Clone() { return default(TNode); }
        public TNode Union(TNode lnode2) { return default(TNode); }
        public bool IsEquals(TNode lnode2) { return true; }
        public bool IsSubsetOf(TNode lnode2) { return true; }
        public bool IsSupersetOf(TNode lnode2) { return true; }
        public TNode[] ChildrenAsChainPathes() { return null; }
        public TNode ParentAsChainPath(TNode child) { return default(TNode); }
        //public void Add(TNode parent, TNode newChild) { };
    }

    public class STree<TSNodeHead, TSNode, TName> where TSNode : TSNodeHead
    {
        Dictionary<SSNode, SSNodeRoot> dictionary = new Dictionary<SSNode /*child*/, SSNodeRoot /* parent*/>();
        public STree(Func<TSNodeHead, IEnumerable<TSNode>> getChildren, Func<TSNode, TName> getName)
        {

        }
        
        public TSNodeHead Clone() { return default(TSNodeHead); }
        public TSNodeHead Union(TSNodeHead lnode2) { return default(TSNodeHead); }
        public bool IsEquals(TSNodeHead lnode2) { return true; }
        public bool IsSubsetOf(TSNodeHead lnode2) { return true; }
        public bool IsSupersetOf(TSNodeHead lnode2) { return true; }
        public TSNodeHead[] ChildrenAsChainPathes() { return null;  }
        public TSNodeHead ParentAsChainPath(TSNode node) { return default(TSNodeHead); }
        //public void Add(TNode parent, TNode newChild) { };
    }

    public class SNodeBase<TBaseValue, TValue>
    {
        protected readonly Dictionary<string, SNode<TBaseValue, TValue>> Children = new Dictionary<string, SNode<TBaseValue, TValue>>();
        public readonly TBaseValue HeadValue;

        public SNodeBase(TBaseValue headValue)
        {
            HeadValue = headValue;
        }

        #region Dictionary emulation
        public SNode<TBaseValue, TValue> FindChild(string key)
        {
            Children.TryGetValue(key, out SNode<TBaseValue, TValue> node);
            return node;
        }

        public SNode<TBaseValue, TValue>[] GetChildrenArray()
        {
            return Children.Values.ToArray();
        }

        public SNode<TBaseValue, TValue> AddChild(string key, TBaseValue headValue, TValue value)
        {
            var node = new SNode<TBaseValue, TValue>(headValue, value, key, this);
            this.Children.Add(key,node);
            return node;
        }

        #endregion

        #region Modify Tree
        public SNodeBase<TBaseValue, TValue> Clone()
        {
            var cloned = new SNodeBase<TBaseValue, TValue>(HeadValue);
            var children = GetChildrenArray();
            foreach (var c in children)
                c.CloneTo(cloned);
            return cloned;
        }

        public SNodeBase<TBaseValue, TValue> Union(SNodeBase<TBaseValue, TValue> node)
        {
            var cloned = node.Clone();
            UnionRecursive(this, cloned);
            return cloned;
        }

        private static void UnionRecursive(SNodeBase<TBaseValue, TValue> source, SNodeBase<TBaseValue, TValue> cloned)
        {
            var sourceChildren = source.GetChildrenArray();
            foreach (var sourceChild in sourceChildren)
            {
                var clonedChild = cloned.FindChild(sourceChild.Key);
                if (clonedChild == null)
                    sourceChild.CloneTo(cloned);
                else
                    UnionRecursive(sourceChild, clonedChild);
            }
        }
        #endregion

        #region logical operations
        public bool IsEquals(SNodeBase<TBaseValue, TValue> node)
        {
            var @value = IsEqualsRecursive(this, node);
            return @value;
        }

        private static bool IsEqualsRecursive(SNodeBase<TBaseValue, TValue> node1, SNodeBase<TBaseValue, TValue> node2)
        {
            var children1 = node1.GetChildrenArray();
            var children2 = node2.GetChildrenArray();
            if (children1.Length != children2.Length)
                return false;
            if (children1.Length == 0 && children2.Length == 0)
                return true;
            var found = true;
            foreach (var n2 in children2)
            {
                var f = node1.FindChild(n2.Key);
                if (f != null)
                    found = IsEqualsRecursive(n2, f);
                else
                    found = false;
                if (found == false)
                    break;
            }
            return found;
        }

        public bool IsSupersetOf(SNodeBase<TBaseValue, TValue> node)
        {
            var @value = IsSubsetRecursive(node, this);
            return @value;
        }

        public bool IsSubsetOf(SNodeBase<TBaseValue, TValue> node)
        {
            var @value = IsSubsetRecursive(this, node);
            return @value;
        }

        private static bool IsSubsetRecursive(SNodeBase<TBaseValue, TValue> node1, SNodeBase<TBaseValue, TValue> node2)
        {
            var children1 = node1.GetChildrenArray();
            var children2 = node2.GetChildrenArray();
            if (children1.Length == 0 && children2.Length == 0)
                return true;
            var found = true;
            foreach (var n in children1)
            {
                var f = node2.FindChild(n.Key);
                if (f != null)
                    found = IsSubsetRecursive(n, f);
                else
                    found = false;
                if (found == false)
                    break;
            }
            return found;
        }
        #endregion

        public List<SNodeBase<TBaseValue, TValue>> ToChainPathes()
        {
            var pathes = new List<SNodeBase<TBaseValue, TValue>>();
            var children = GetChildrenArray();
            foreach (var c in children)
            {
                c.ToChainPathesRecursive(pathes);
            }
            return pathes;
        }

        public List<string> ToXPathes()
        {
            var pathes = new List<string>();
            var children = GetChildrenArray();
            foreach (var c in children)
                c.ToXPathesRecursive("", pathes);
            return pathes;
        }

        public override string ToString()
        {
            var sb = new StringBuilder();
            var pathes = ToXPathes();

            var e = pathes.GetEnumerator();
            bool moveNext = e.MoveNext();
            while (moveNext)
            {
                sb.Append(e.Current);
                moveNext = e.MoveNext();
                if (moveNext)
                    sb.Append(" | ");
            }
            string @value = sb.ToString();
            return @value;
        }
    }

    public class SNode<THeadValue, TValue> : SNodeBase<THeadValue, TValue>
    {
        public readonly SNodeBase<THeadValue, TValue> Parent;
        public readonly string Key;
        public readonly TValue Value;

        internal SNode(THeadValue headValue, TValue value, string key, SNodeBase<THeadValue, TValue> parent)
            : base(headValue)
        {
            Value = value;
            Key = key;
            Parent = parent;
        }

        internal SNode<THeadValue, TValue> CloneTo(SNodeBase<THeadValue, TValue> parent)
        {
            var cloned = parent.AddChild(Key, HeadValue, Value);
            var children = GetChildrenArray();
            foreach (var c in children)
                c.CloneTo(cloned);
            return cloned;
        }

        public SNodeBase<THeadValue, TValue> GetChainPath()
        {
            var ancestorsAndSelf = new List<SNode<THeadValue, TValue>>();
            ancestorsAndSelf.Add(this);
            var root = this.Parent;
            while (root is SNode<THeadValue, TValue>)
            {
                ancestorsAndSelf.Add(this);
                root = ((SNode<THeadValue, TValue>)root).Parent;
            }
            ancestorsAndSelf.Reverse();

            var @value = new SNodeBase<THeadValue, TValue>(HeadValue);
            var p = @value;
            foreach (var a in ancestorsAndSelf)
            {
                p = p.AddChild(a.Key, a.HeadValue, a.Value);
            }
            return @value;
        }

        public void ToXPathesRecursive(string basePath, List<string> pathes)
        {
            var children = GetChildrenArray();
            var path = basePath + @"/" + Key;
            if (children.Length == 0)
                pathes.Add(path);
            else
                foreach (var c in children)
                    c.ToXPathesRecursive(path, pathes);
        }

        public void ToChainPathesRecursive(List<SNodeBase<THeadValue, TValue>> lists)
        {
            var children = GetChildrenArray();
            if (children.Length == 0)
                lists.Add(GetChainPath());
            else
                foreach (var c in children)
                    c.ToChainPathesRecursive(lists);
        }
    }
}