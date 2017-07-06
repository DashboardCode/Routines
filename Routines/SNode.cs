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

    public class STree<TSNodePrimal, TSNode, TName> where TSNode : TSNodePrimal
    {
        Func<TSNodePrimal, IEnumerable<TSNode>> getChildren;
        Func<TSNodePrimal, TSNodePrimal> cloneRoot;
        Func<TSNode, TSNodePrimal, TSNode> cloneChild;
        Func<TSNode, TName> getName;
        Func<TSNodePrimal, TName, TSNode> getChild;
        public STree(
            Func<TSNodePrimal, IEnumerable<TSNode>> getChildren,
            Func<TSNodePrimal, TSNodePrimal> cloneHead,
            Func<TSNode, TSNodePrimal, TSNode> cloneChild,
            Func<TSNode, TName> getName,
            Func<TSNodePrimal, TName, TSNode> getChild)
        {
            this.getChildren = getChildren;
            this.cloneRoot = cloneHead;
            this.cloneChild = cloneChild;
            this.getName = getName;
            this.getChild = getChild;
        }
        
        public TSNodePrimal Clone(TSNodePrimal nodeHead) {
            var cloned = cloneRoot(nodeHead); 
            var children = getChildren(nodeHead);

            foreach (var c in children)
                CloneRecursive(c, cloned);
            return cloned;
        }

        private void CloneRecursive(TSNode n, TSNodePrimal parent)
        {
            var cloned = cloneChild(n, parent);// .AddChild(n.Key, n.HeadValue, n.Value);
            var children = getChildren(n);
            foreach (var c in children)
                CloneRecursive(c, cloned);
        }

        public TSNodePrimal Union(TSNodePrimal node1, TSNodePrimal node2)
        {
            var cloned = Clone(node2);
            UnionRecursive(node1, cloned);
            return cloned;
        }

        internal void UnionRecursive(TSNodePrimal source, TSNodePrimal cloned)
        {
            var sourceChildren = getChildren(source);
            foreach (var sourceChild in sourceChildren)
            {
                var key = getName(sourceChild);
                var clonedChild = getChild(cloned, key);
                if (clonedChild == null)
                    CloneRecursive(sourceChild, cloned);
                else
                    UnionRecursive(sourceChild, clonedChild);
            }
        }

        public bool IsEquals(TSNodePrimal node1, TSNodePrimal node2)
        {
            var @value = IsEqualsRecursive(node1, node2);
            return @value;
        }

        private bool IsEqualsRecursive(TSNodePrimal node1, TSNodePrimal node2)
        {
            var children1 = getChildren(node1); 
            var children2 = getChildren(node2); 
            if (children1.Count() != children2.Count())
                return false;
            if (children1.Count() == 0 && children2.Count() == 0)
                return true;
            var found = true;
            foreach (var n2 in children2)
            {
                var key = getName(n2);
                var f = getChild(node1, key);
                if (f != null)
                    found = IsEqualsRecursive(n2, f);
                else
                    found = false;
                if (found == false)
                    break;
            }
            return found;
        }

        public bool IsSubsetOf(TSNodePrimal node1, TSNodePrimal node2) {
            var @value = IsSubsetRecursive(node1, node2);
            return @value;
        }

        private bool IsSubsetRecursive(TSNodePrimal node1, TSNodePrimal node2)
        {
            var children1 = getChildren(node1); 
            var children2 = getChildren(node2);
            if(children1.Count() == 0 && children2.Count() == 0)
                return true;
            var found = true;
            foreach (var n in children1)
            {
                var key = getName(n);
                var f = getChild(node2, key);
                if (f != null)
                    found = IsSubsetRecursive(n, f);
                else
                    found = false;
                if (found == false)
                    break;
            }
            return found;
        }

        public bool IsSupersetOf(TSNodePrimal node1, TSNodePrimal node2) {
            var @value = IsSubsetRecursive(node2, node1);
            return @value;
        }

        public List<string> ToXPathes(TSNodePrimal node)
        {
            var pathes = new List<string>();
            var children = getChildren(node);
            foreach (var c in children)
                ToXPathesRecursive(c, "", pathes);
            return pathes;
        }

        private void ToXPathesRecursive(TSNode node, string basePath, List<string> pathes)
        {
            var children = getChildren(node);
            var key = getName(node); //node)
            var path = basePath + @"/" + key;
            if (children.Count() == 0)
                pathes.Add(path);
            else
                foreach (var c in children)
                    ToXPathesRecursive(c, path, pathes);
        }

        private void ToChainPathesRecursive(TSNodePrimal node, TSNodePrimal nodeRoot, List<Func<TSNodePrimal, TSNode>> basePath, List<TSNodePrimal> lists)
        {
            var children = getChildren(node);
            if (children.Count() == 0)
            {
                TSNodePrimal newRoot = cloneRoot(nodeRoot);
                TSNodePrimal newNode = newRoot;
                foreach (var f in basePath)
                {
                    newNode = f(newNode);
                }
                lists.Add(newNode);
            }
            else
                foreach (var c in children)
                {
                    basePath.Add((p) => cloneChild(c, p));
                    ToChainPathesRecursive(c, nodeRoot, basePath, lists);
                }
        }

        public List<TSNodePrimal> ToChainPathes(TSNodePrimal root)
        {
            var pathes = new List<TSNodePrimal>();
            var children = getChildren(root);
            var basePath = new  List < Func<TSNodePrimal, TSNode> >();
            foreach (var c in children)
            {
                basePath.Add((p) => cloneChild(c, p));
                ToChainPathesRecursive(c, root, basePath, pathes);
            }
            return pathes;
        }

        public string ToXPathUnion(TSNodePrimal node)
        {
            var sb = new StringBuilder();
            var pathes = ToXPathes(node);

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

        public TSNodePrimal GetChainPath(TSNode node, Func<TSNode, TSNodePrimal> GetParent)
        {
            var ancestorsAndSelf = new List<TSNode>();
            ancestorsAndSelf.Add(node);
            var root = GetParent(node);
            while (root is TSNode)
            {
                ancestorsAndSelf.Add(node);
                root = GetParent((TSNode)root);
            }
            ancestorsAndSelf.Reverse();

            var @value = cloneRoot(node); // new SNodeBase<THeadValue, TValue>(HeadValue);
            var p = @value;
            foreach (var a in ancestorsAndSelf)
            {
                p = cloneChild(a, p);
                    //p.AddChild(a.Key, a.HeadValue, a.Value);
            }
            return @value;
        }

        //public TSNodePrimal[] ChildrenAsChainPathes()
        //{
        //    return null;
        //}

        //public TSNodePrimal ParentAsChainPath(TSNode node)
        //{
        //    return default(TSNodePrimal);
        //}

    }

    //public class SNodeBase<TBaseValue, TValue>
    //{
    //    protected readonly Dictionary<string, SNode<TBaseValue, TValue>> Children = new Dictionary<string, SNode<TBaseValue, TValue>>();
    //    public readonly TBaseValue HeadValue;

    //    public SNodeBase(TBaseValue headValue)
    //    {
    //        HeadValue = headValue;
    //    }

    //    #region Dictionary emulation
    //    public SNode<TBaseValue, TValue> FindChild(string key)
    //    {
    //        Children.TryGetValue(key, out SNode<TBaseValue, TValue> node);
    //        return node;
    //    }

    //    public SNode<TBaseValue, TValue>[] GetChildrenArray()
    //    {
    //        return Children.Values.ToArray();
    //    }

    //    public SNode<TBaseValue, TValue> AddChild(string key, TBaseValue headValue, TValue value)
    //    {
    //        var node = new SNode<TBaseValue, TValue>(headValue, value, key, this);
    //        this.Children.Add(key ,node);
    //        return node;
    //    }

    //    #endregion

    //    #region Modify Tree
    //    public SNodeBase<TBaseValue, TValue> Clone()
    //    {
    //        var cloned = new SNodeBase<TBaseValue, TValue>(HeadValue);
    //        var children = GetChildrenArray();
    //        foreach (var c in children)
    //            c.CloneTo(cloned);
    //        return cloned;
    //    }

    //    public SNodeBase<TBaseValue, TValue> Union(SNodeBase<TBaseValue, TValue> node)
    //    {
    //        var cloned = node.Clone();
    //        UnionRecursive(this, cloned);
    //        return cloned;
    //    }

    //    private static void UnionRecursive(SNodeBase<TBaseValue, TValue> source, SNodeBase<TBaseValue, TValue> cloned)
    //    {
    //        var sourceChildren = source.GetChildrenArray();
    //        foreach (var sourceChild in sourceChildren)
    //        {
    //            var clonedChild = cloned.FindChild(sourceChild.Key);
    //            if (clonedChild == null)
    //                sourceChild.CloneTo(cloned);
    //            else
    //                UnionRecursive(sourceChild, clonedChild);
    //        }
    //    }
    //    #endregion

    //    #region logical operations
    //    public bool IsEquals(SNodeBase<TBaseValue, TValue> node)
    //    {
    //        var @value = IsEqualsRecursive(this, node);
    //        return @value;
    //    }

    //    private static bool IsEqualsRecursive(SNodeBase<TBaseValue, TValue> node1, SNodeBase<TBaseValue, TValue> node2)
    //    {
    //        var children1 = node1.GetChildrenArray();
    //        var children2 = node2.GetChildrenArray();
    //        if (children1.Length != children2.Length)
    //            return false;
    //        if (children1.Length == 0 && children2.Length == 0)
    //            return true;
    //        var found = true;
    //        foreach (var n2 in children2)
    //        {
    //            var f = node1.FindChild(n2.Key);
    //            if (f != null)
    //                found = IsEqualsRecursive(n2, f);
    //            else
    //                found = false;
    //            if (found == false)
    //                break;
    //        }
    //        return found;
    //    }

    //    public bool IsSupersetOf(SNodeBase<TBaseValue, TValue> node)
    //    {
    //        var @value = IsSubsetRecursive(node, this);
    //        return @value;
    //    }

    //    public bool IsSubsetOf(SNodeBase<TBaseValue, TValue> node)
    //    {
    //        var @value = IsSubsetRecursive(this, node);
    //        return @value;
    //    }

    //    private static bool IsSubsetRecursive(SNodeBase<TBaseValue, TValue> node1, SNodeBase<TBaseValue, TValue> node2)
    //    {
    //        var children1 = node1.GetChildrenArray();
    //        var children2 = node2.GetChildrenArray();
    //        if (children1.Length == 0 && children2.Length == 0)
    //            return true;
    //        var found = true;
    //        foreach (var n in children1)
    //        {
    //            var f = node2.FindChild(n.Key);
    //            if (f != null)
    //                found = IsSubsetRecursive(n, f);
    //            else
    //                found = false;
    //            if (found == false)
    //                break;
    //        }
    //        return found;
    //    }
    //    #endregion

    //    //public List<SNodeBase<TBaseValue, TValue>> ToChainPathes()
    //    //{
    //    //    var pathes = new List<SNodeBase<TBaseValue, TValue>>();
    //    //    var children = GetChildrenArray();
    //    //    foreach (var c in children)
    //    //    {
    //    //        c.ToChainPathesRecursive(pathes);
    //    //    }
    //    //    return pathes;
    //    //}

    //    //public List<string> ToXPathes()
    //    //{
    //    //    var pathes = new List<string>();
    //    //    var children = GetChildrenArray();
    //    //    foreach (var c in children)
    //    //        c.ToXPathesRecursive("", pathes);
    //    //    return pathes;
    //    //}

    //    //public override string ToString()
    //    //{
    //    //    var sb = new StringBuilder();
    //    //    var pathes = ToXPathes();

    //    //    var e = pathes.GetEnumerator();
    //    //    bool moveNext = e.MoveNext();
    //    //    while (moveNext)
    //    //    {
    //    //        sb.Append(e.Current);
    //    //        moveNext = e.MoveNext();
    //    //        if (moveNext)
    //    //            sb.Append(" | ");
    //    //    }
    //    //    string @value = sb.ToString();
    //    //    return @value;
    //    //}
    //}

    //public class SNode<THeadValue, TValue> : SNodeBase<THeadValue, TValue>
    //{
    //    public readonly SNodeBase<THeadValue, TValue> Parent;
    //    public readonly string Key;
    //    public readonly TValue Value;

    //    internal SNode(THeadValue headValue, TValue value, string key, SNodeBase<THeadValue, TValue> parent)
    //        : base(headValue)
    //    {
    //        Value = value;
    //        Key = key;
    //        Parent = parent;
    //    }

    //    internal SNode<THeadValue, TValue> CloneTo(SNodeBase<THeadValue, TValue> parent)
    //    {
    //        var cloned = parent.AddChild(Key, HeadValue, Value);
    //        var children = GetChildrenArray();
    //        foreach (var c in children)
    //            c.CloneTo(cloned);
    //        return cloned;
    //    }

    //    public SNodeBase<THeadValue, TValue> GetChainPath()
    //    {
    //        var ancestorsAndSelf = new List<SNode<THeadValue, TValue>>();
    //        ancestorsAndSelf.Add(this);
    //        var root = this.Parent;
    //        while (root is SNode<THeadValue, TValue>)
    //        {
    //            ancestorsAndSelf.Add(this);
    //            root = ((SNode<THeadValue, TValue>)root).Parent;
    //        }
    //        ancestorsAndSelf.Reverse();

    //        var @value = new SNodeBase<THeadValue, TValue>(HeadValue);
    //        var p = @value;
    //        foreach (var a in ancestorsAndSelf)
    //        {
    //            p = p.AddChild(a.Key, a.HeadValue, a.Value);
    //        }
    //        return @value;
    //    }

    //    //public void ToXPathesRecursive(string basePath, List<string> pathes)
    //    //{
    //    //    var children = GetChildrenArray();
    //    //    var path = basePath + @"/" + Key;
    //    //    if (children.Length == 0)
    //    //        pathes.Add(path);
    //    //    else
    //    //        foreach (var c in children)
    //    //            c.ToXPathesRecursive(path, pathes);
    //    //}

    //    //public void ToChainPathesRecursive(List<SNodeBase<THeadValue, TValue>> lists)
    //    //{
    //    //    var children = GetChildrenArray();
    //    //    if (children.Length == 0)
    //    //        lists.Add(GetChainPath());
    //    //    else
    //    //        foreach (var c in children)
    //    //            c.ToChainPathesRecursive(lists);
    //    //}
    //}
}