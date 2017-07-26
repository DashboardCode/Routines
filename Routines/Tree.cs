using System.Collections.Generic;
using System.Linq;
using System.Text;
using System;

namespace DashboardCode.Routines
{
    public class Tree<TNodePrimal, TNode, TKey>
        where TNode : TNodePrimal
    {
        public readonly Func<TNodePrimal, IEnumerable<TNode>> GetChildren;
        public readonly Func<TNodePrimal, TNodePrimal> CloneRootPrimal;
        public readonly Func<TNode, TNodePrimal, TNode> CloneChild;
        public readonly Func<TNode, TKey> GetKey;
        public readonly Func<TNodePrimal, TKey, TNode> GetChild;
        public readonly Func<TNodePrimal, TNodePrimal, bool> RootPrimalEquals;

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
            this.GetChildren = getChildren;
            this.CloneRootPrimal = cloneRootPrimal;
            this.CloneChild = cloneChild;
            this.GetKey = getKey;
            this.GetChild = getChild;
            this.RootPrimalEquals = rootPrimalEquals ?? ((n1, n2) => true);
        }
    }

    public class LinkedTree<TNodePrimal, TNode, TKey> : Tree<TNodePrimal, TNode, TKey>
        where TNode : TNodePrimal
    {
        public readonly Func<TNode, TNodePrimal> LinkToParent;


        /// <summary>
        /// 
        /// </summary>
        /// <param name="getChildren"></param>
        /// <param name="getKey">get not root node's Key</param>
        /// <param name="getChild">get child by key</param>
        /// <param name="duplicateRoot">param: node for clone; should return cloned node (with empty child list)</param>
        /// <param name="duplicateChild">1st param: node for clone; 2nd param: new parent; should return cloned node (with empty child list)</param>
        /// <param name="rootEquals"></param>
        public LinkedTree(
            Func<TNodePrimal, IEnumerable<TNode>> getChildren,
            Func<TNode, TKey> getKey,
            Func<TNodePrimal, TKey, TNode> getChild,
            Func<TNodePrimal, TNodePrimal> duplicateRoot,
            Func<TNode, TNodePrimal, TNode> duplicateChild,
            Func<TNode, TNodePrimal> linktToParent,
            Func<TNodePrimal, TNodePrimal, bool> rootEquals = null):base(
                    getChildren, getKey, getChild, duplicateRoot, duplicateChild, rootEquals
                )
        {
            this.LinkToParent=linktToParent;
        }
    }

    public static class TreeExtensions
    {
        #region Modification: Clone, Union, PathOfNode
        public static TNodePrimal Clone<TNodePrimal, TNode, TKey>(this Tree<TNodePrimal, TNode, TKey>  tree, TNodePrimal nodeHead)
            where TNode : TNodePrimal
        {
            var cloned = tree.CloneRootPrimal(nodeHead); 
            var children = tree.GetChildren(nodeHead);

            foreach (var c in children)
                tree.cloneRecursive(c, cloned);
            return cloned;
        }

        private static void cloneRecursive<TNodePrimal, TNode, TKey>(this Tree<TNodePrimal, TNode, TKey> tree, TNode n, TNodePrimal parent)
            where TNode : TNodePrimal
        {
            var cloned = tree.CloneChild(n, parent);
            var children = tree.GetChildren(n);
            foreach (var c in children)
                tree.cloneRecursive(c, cloned);
        }

        public static TNodePrimal Merge<TNodePrimal, TNode, TKey>(this Tree<TNodePrimal, TNode, TKey> tree, TNodePrimal node1, TNodePrimal node2)
            where TNode : TNodePrimal
        {
            if (!tree.RootPrimalEquals(node1, node2))
                throw new ArgumentException("Tree's root nodes are not equal. You can do union only trees when " +nameof(Tree<TNodePrimal,TNode,TKey>.RootPrimalEquals) + "tree's argument return true");
            var cloned = tree.Clone(node2);
            tree.mergeRecursive(node1, cloned);
            return cloned;
        }

        private static void mergeRecursive<TNodePrimal, TNode, TKey>(this Tree<TNodePrimal, TNode, TKey> tree, TNodePrimal source, TNodePrimal cloned)
            where TNode : TNodePrimal
        {
            var sourceChildren = tree.GetChildren(source);
            foreach (var sourceChild in sourceChildren)
            {
                var key = tree.GetKey(sourceChild);
                var clonedChild = tree.GetChild(cloned, key);
                if (clonedChild == null)
                    tree.cloneRecursive(sourceChild, cloned);
                else
                    tree.mergeRecursive(sourceChild, clonedChild);
            }
        }

        public static TNodePrimal FindLinkedRootPath<TNodePrimal, TNode, TKey>(this LinkedTree<TNodePrimal, TNode, TKey> tree, TNode node)
            where TNode : TNodePrimal
        {
            var ancestorsAndSelf = new List<TNodePrimal>();
            TNodePrimal parent = node;
            while (parent !=null)
            {
                ancestorsAndSelf.Add(parent);
                if (parent is TNode alsoParent)
                    parent = tree.LinkToParent(alsoParent);
                else
                {
                    parent = default(TNodePrimal);
                    break;
                }
            }

            ancestorsAndSelf.Reverse();
            var root = ancestorsAndSelf.First();
            ancestorsAndSelf.RemoveAt(0);
            var @value = tree.CloneRootPrimal(root);
            var p = @value;
            foreach (var a in ancestorsAndSelf)
            {
                p = tree.CloneChild((TNode)a, p);
            }
            return @value;
        }

        public static string FindLinkedRootXPath<TNodePrimal, TNode, TKey>(this LinkedTree<TNodePrimal, TNode, TKey> tree, TNode node)
            where TNode : TNodePrimal
        {
            TNodePrimal parent = node;
            var ancestorsAndSelf = new List<string>();
            var l = 0;  
            while (true)
            {
                if (parent is TNode child)
                {
                    parent = tree.LinkToParent(child);
                    if (parent == null)
                    {
                        break; // it is root
                    }
                    else
                    {
                        var nodeKey = tree.GetKey(child);
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

        public static bool IsEqualTo<TNodePrimal, TNode, TKey>(this Tree<TNodePrimal, TNode, TKey> tree, TNodePrimal node1, TNodePrimal node2)
            where TNode : TNodePrimal
        {
            if (!tree.RootPrimalEquals(node1, node2))
                return false;
            var @value = tree.isEqualToRecursive(node1, node2);
            return @value;
        }

        private static bool isEqualToRecursive<TNodePrimal, TNode, TKey>(this Tree<TNodePrimal, TNode, TKey> tree, TNodePrimal node1, TNodePrimal node2)
            where TNode : TNodePrimal
        {
            var children1 = tree.GetChildren(node1); 
            var children2 = tree.GetChildren(node2); 
            if (children1.Count() != children2.Count())
                return false;
            if (children1.Count() == 0 && children2.Count() == 0)
                return true;
            var found = true;
            foreach (var n2 in children2)
            {
                var key = tree.GetKey(n2);
                var f = tree.GetChild(node1, key);
                if (f != null)
                    found = tree.isEqualToRecursive(n2, f);
                else
                    found = false;
                if (found == false)
                    break;
            }
            return found;
        }

        public static bool IsSubsetOf<TNodePrimal, TNode, TKey>(this Tree<TNodePrimal, TNode, TKey> tree, TNodePrimal node1, TNodePrimal node2)
            where TNode : TNodePrimal
        {
            if (!tree.RootPrimalEquals(node1, node2))
                return false;
            var @value = tree.isSubsetRecursive(node1, node2);
            return @value;
        }

        private static bool isSubsetRecursive<TNodePrimal, TNode, TKey>(this Tree<TNodePrimal, TNode, TKey> tree, TNodePrimal node1, TNodePrimal node2)
            where TNode : TNodePrimal
        {
            var children1 = tree.GetChildren(node1); 
            var children2 = tree.GetChildren(node2);
            if(children1.Count() == 0 && children2.Count() == 0)
                return true;
            var found = true;
            foreach (var n in children1)
            {
                var key = tree.GetKey(n);
                var f = tree.GetChild(node2, key);
                if (f != null)
                    found = tree.isSubsetRecursive(n, f);
                else
                    found = false;
                if (found == false)
                    break;
            }
            return found;
        }

        public static bool IsSupersetOf<TNodePrimal, TNode, TKey>(this Tree<TNodePrimal, TNode, TKey> tree, TNodePrimal node1, TNodePrimal node2)
            where TNode : TNodePrimal
        {
            var @value = tree.isSubsetRecursive(node2, node1);
            return @value;
        }
        #endregion

        #region GetTreeAs

        public static List<TNodePrimal> ListLeafPaths<TNodePrimal, TNode, TKey>(this Tree<TNodePrimal, TNode, TKey> tree, TNodePrimal root, Func<TNodePrimal, bool> condition = null)
            where TNode : TNodePrimal
        {
            var @value = new List<TNodePrimal>();
            var children = tree.GetChildren(root);
            var basePath = new  List < Func<TNodePrimal, TNode> >();
            foreach (var c in children)
            {
                basePath.Add((p) => tree.CloneChild(c, p));
                tree.listLeafPathsRecursive(c, root, basePath, @value, condition);
            }
            return @value;
        }

        private static void listLeafPathsRecursive<TNodePrimal, TNode, TKey>(this Tree<TNodePrimal, TNode, TKey> tree, TNodePrimal node, TNodePrimal nodeRoot, List<Func<TNodePrimal, TNode>> basePath, List<TNodePrimal> lists, Func<TNodePrimal, bool> condition)
            where TNode : TNodePrimal
        {
            var children = tree.GetChildren(node);
            if (children.Count() == 0)
            {
                TNodePrimal newRoot = tree.CloneRootPrimal(nodeRoot);
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
                    basePath.Add((p) => tree.CloneChild(c, p));
                    tree.listLeafPathsRecursive(c, nodeRoot, basePath, lists, condition);
                }
        }

        public static List<TKey[]> ListLeafKeyPaths<TNodePrimal, TNode, TKey>(this Tree<TNodePrimal, TNode, TKey> tree, TNodePrimal node)
            where TNode : TNodePrimal
        {
            var pathes = new List<TKey[]>();
            var children = tree.GetChildren(node);
            foreach (var c in children)
                tree.listLeafKeyPathsRecursive(c, new TKey[0], pathes);
            return pathes;
        }

        private static void listLeafKeyPathsRecursive<TNodePrimal, TNode, TKey>(this Tree<TNodePrimal, TNode, TKey> tree, TNode node, TKey[] basePath, List<TKey[]> pathes)
            where TNode : TNodePrimal
        {
            var children = tree.GetChildren(node);
            var key = tree.GetKey(node);
            var path = new TKey[basePath.Length+1];
            Array.Copy(basePath, path, basePath.Length);
            path[basePath.Length] = key;
            if (children.Count() == 0)
                pathes.Add(path);
            else
                foreach (var c in children)
                    tree.listLeafKeyPathsRecursive(c, path, pathes);
        }

        public static List<string> ListLeafXPaths<TNodePrimal, TNode, TKey>(this Tree<TNodePrimal, TNode, TKey> tree, TNodePrimal node)
            where TNode : TNodePrimal
        {
            var pathes = new List<string>();
            var children = tree.GetChildren(node);
            foreach (var c in children)
                tree.listLeafXPaths(c, "", pathes);
            return pathes;
        }

        private static void listLeafXPaths<TNodePrimal, TNode, TKey>(this Tree<TNodePrimal, TNode, TKey> tree, TNode node, string basePath, List<string> pathes)
            where TNode : TNodePrimal
        {
            var children = tree.GetChildren(node);
            var key = tree.GetKey(node);
            var path = basePath + @"/" + key;
            if (children.Count() == 0)
                pathes.Add(path);
            else
                foreach (var c in children)
                    tree.listLeafXPaths(c, path, pathes);
        }

        public static string CollectLeafsToXPathUnion<TNodePrimal, TNode, TKey>(this Tree<TNodePrimal, TNode, TKey> tree, TNodePrimal node)
            where TNode : TNodePrimal
        {
            var sb = new StringBuilder();
            var pathes = tree.ListLeafXPaths(node);

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