using System.Collections.Generic;
using System.Linq;
using System.Text;
using System;

namespace DashboardCode.Routines
{
    public class Tree<TNodePrimal, TNode, TKey> where TNode : TNodePrimal
    {
        Func<TNodePrimal, IEnumerable<TNode>> getChildren;
        Func<TNodePrimal, TNodePrimal> cloneRootPrimal;
        Func<TNode, TNodePrimal, TNode> cloneChild;
        Func<TNode, TKey> getKey;
        Func<TNodePrimal, TKey, TNode> getChild;
        Func<TNodePrimal, TNodePrimal, bool> rootPrimalEquals;
        /// <summary>
        /// 
        /// </summary>
        /// <param name="getChildren"></param>
        /// <param name="getKey">get not root node's Key</param>
        /// <param name="getChild">get child by key</param>
        /// <param name="cloneRootPrimal">param: node for clone; should return cloned node (with empty child list)</param>
        /// <param name="cloneChild">1st param: node for clone; 2nd param: new parent; should return cloned node (with empty child list)</param>
        /// <param name="rootPrimalEquals"></param>
        public Tree(
            Func<TNodePrimal, IEnumerable<TNode>> getChildren,
            Func<TNode, TKey> getKey,
            Func<TNodePrimal, TKey, TNode> getChild,
            Func<TNodePrimal, TNodePrimal> cloneRootPrimal,
            Func<TNode, TNodePrimal, TNode> cloneChild,
            Func<TNodePrimal, TNodePrimal, bool> rootPrimalEquals = null)
        {
            this.getChildren = getChildren;
            this.cloneRootPrimal = cloneRootPrimal;
            this.cloneChild = cloneChild;
            this.getKey = getKey;
            this.getChild = getChild;
            this.rootPrimalEquals = rootPrimalEquals ?? ((n1, n2) => true);
        }

        #region Modification: Clone, Union, PathOfNode
        public TNodePrimal Clone(TNodePrimal nodeHead) {
            var cloned = cloneRootPrimal(nodeHead); 
            var children = getChildren(nodeHead);

            foreach (var c in children)
                CloneRecursive(c, cloned);
            return cloned;
        }

        private void CloneRecursive(TNode n, TNodePrimal parent)
        {
            var cloned = cloneChild(n, parent);
            var children = getChildren(n);
            foreach (var c in children)
                CloneRecursive(c, cloned);
        }

        public TNodePrimal Union(TNodePrimal node1, TNodePrimal node2)
        {
            if (!rootPrimalEquals(node1, node2))
                throw new ArgumentException("Tree's root nodes are not equal. You can do union only trees when " +nameof(rootPrimalEquals) + "tree's argument return true");
            var cloned = Clone(node2);
            UnionRecursive(node1, cloned);
            return cloned;
        }

        private void UnionRecursive(TNodePrimal source, TNodePrimal cloned)
        {
            var sourceChildren = getChildren(source);
            foreach (var sourceChild in sourceChildren)
            {
                var key = getKey(sourceChild);
                var clonedChild = getChild(cloned, key);
                if (clonedChild == null)
                    CloneRecursive(sourceChild, cloned);
                else
                    UnionRecursive(sourceChild, clonedChild);
            }
        }

        public TNodePrimal GetPathOfNode(TNode node, Func<TNode, TNodePrimal> GetParent)
        {
            var ancestorsAndSelf = new List<TNodePrimal>();
            TNodePrimal parent = node;
            while (parent !=null)
            {
                ancestorsAndSelf.Add(parent);
                if (parent is TNode)
                    parent = GetParent((TNode)parent);
                else
                {
                    parent = default(TNodePrimal);
                    break;
                }
            }

            ancestorsAndSelf.Reverse();
            var root = ancestorsAndSelf.First();
            ancestorsAndSelf.RemoveAt(0);
            var @value = cloneRootPrimal(root);
            var p = @value;
            foreach (var a in ancestorsAndSelf)
            {
                p = cloneChild((TNode)a, p);
            }
            return @value;
        }

        public string GetXPathOfNode(TNode node, Func<TNode, TNodePrimal> GetParent)
        {
            TNodePrimal parent = node;
            var ancestorsAndSelf = new List<string>();
            var l = 0;  
            while (true)
            {
                if (parent is TNode)
                {
                    var child = (TNode)parent;
                    parent = GetParent(child);
                    if (parent == null)
                    {
                        break; // it is root
                    }
                    else
                    {
                        var nodeKey = getKey(child);
                        var keyText = nodeKey.ToString();
                        ancestorsAndSelf.Add(keyText);
                        l += keyText.Length + 1;
                    }
                    
                }
                else
                {
                    break;
                }
            }
            if (l == 0)
                return "/";
            ancestorsAndSelf.Reverse();
            //var root = ancestorsAndSelf.First();
            //ancestorsAndSelf.RemoveAt(0);

            var stringBuilder = new StringBuilder(l);
            foreach (var a in ancestorsAndSelf)
                stringBuilder.Append("/").Append(a);
            var text = stringBuilder.ToString();
            return text;
        }
        #endregion

        #region Logical: IsEqualTo, IsSubsetOf, IsSupersetOf

        public bool IsEqualTo(TNodePrimal node1, TNodePrimal node2)
        {
            if (!rootPrimalEquals(node1, node2))
                return false;
            var @value = IsEqualToRecursive(node1, node2);
            return @value;
        }

        private bool IsEqualToRecursive(TNodePrimal node1, TNodePrimal node2)
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
                var key = getKey(n2);
                var f = getChild(node1, key);
                if (f != null)
                    found = IsEqualToRecursive(n2, f);
                else
                    found = false;
                if (found == false)
                    break;
            }
            return found;
        }

        public bool IsSubsetOf(TNodePrimal node1, TNodePrimal node2) {
            if (!rootPrimalEquals(node1, node2))
                return false;
            var @value = IsSubsetRecursive(node1, node2);
            return @value;
        }

        private bool IsSubsetRecursive(TNodePrimal node1, TNodePrimal node2)
        {
            var children1 = getChildren(node1); 
            var children2 = getChildren(node2);
            if(children1.Count() == 0 && children2.Count() == 0)
                return true;
            var found = true;
            foreach (var n in children1)
            {
                var key = getKey(n);
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

        public bool IsSupersetOf(TNodePrimal node1, TNodePrimal node2) {
            var @value = IsSubsetRecursive(node2, node1);
            return @value;
        }
        #endregion

        #region GetTreeAs

        public List<TNodePrimal> GetTreeAsListOfPaths(TNodePrimal root, Func<TNodePrimal, bool> condition = null)
        {
            var @value = new List<TNodePrimal>();
            var children = getChildren(root);
            var basePath = new  List < Func<TNodePrimal, TNode> >();
            foreach (var c in children)
            {
                basePath.Add((p) => cloneChild(c, p));
                GetTreeAsListOfPathsRecursive(c, root, basePath, @value, condition);
            }
            return @value;
        }

        private void GetTreeAsListOfPathsRecursive(TNodePrimal node, TNodePrimal nodeRoot, List<Func<TNodePrimal, TNode>> basePath, List<TNodePrimal> lists, Func<TNodePrimal, bool> condition)
        {
            var children = getChildren(node);
            if (children.Count() == 0)
            {
                TNodePrimal newRoot = cloneRootPrimal(nodeRoot);
                TNodePrimal newNode = newRoot;
                foreach (var f in basePath)
                {
                    newNode = f(newNode);
                }
                if (condition==null || condition(newNode))
                    lists.Add(newNode);
            }
            else
                foreach (var c in children)
                {
                    basePath.Add((p) => cloneChild(c, p));
                    GetTreeAsListOfPathsRecursive(c, nodeRoot, basePath, lists, condition);
                }
        }

        public List<TKey[]> GetTreeAsListKeysArray(TNodePrimal node)
        {
            var pathes = new List<TKey[]>();
            var children = getChildren(node);
            foreach (var c in children)
                GetTreeAsListKeysArrayRecursive(c, new TKey[0], pathes);
            return pathes;
        }

        private void GetTreeAsListKeysArrayRecursive(TNode node, TKey[] basePath, List<TKey[]> pathes)
        {
            var children = getChildren(node);
            var key = getKey(node);
            var path = new TKey[basePath.Length+1];
            Array.Copy(basePath, path, basePath.Length);
            path[basePath.Length] = key;
            if (children.Count() == 0)
                pathes.Add(path);
            else
                foreach (var c in children)
                    GetTreeAsListKeysArrayRecursive(c, path, pathes);
        }

        public List<string> GetTreeAsListOfXPaths(TNodePrimal node)
        {
            var pathes = new List<string>();
            var children = getChildren(node);
            foreach (var c in children)
                GetTreeAsListOfXPathsRecursive(c, "", pathes);
            return pathes;
        }

        private void GetTreeAsListOfXPathsRecursive(TNode node, string basePath, List<string> pathes)
        {
            var children = getChildren(node);
            var key = getKey(node);
            var path = basePath + @"/" + key;
            if (children.Count() == 0)
                pathes.Add(path);
            else
                foreach (var c in children)
                    GetTreeAsListOfXPathsRecursive(c, path, pathes);
        }

        public string GetTreeAsXPathUnion(TNodePrimal node)
        {
            var sb = new StringBuilder();
            var pathes = GetTreeAsListOfXPaths(node);

            var e = pathes.GetEnumerator();
            bool moveNext = e.MoveNext();
            while (moveNext)
            {
                sb.Append(e.Current);
                moveNext = e.MoveNext();
                if (moveNext)
                    sb.Append(" | ");
            }

            var @value = (sb.Length == 0)? "/": sb.ToString();
            return @value;
        }
        #endregion
    }
}