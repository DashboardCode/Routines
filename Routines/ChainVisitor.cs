using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;

namespace DashboardCode.Routines
{
    public class ChainVisitor<TRootEntity> : IChainVisitor<TRootEntity>
    {
        static ChainVisitor(){
            var isEnumerable = typeof(IEnumerable).GetTypeInfo().IsAssignableFrom(typeof(TRootEntity).GetTypeInfo());
            if (typeof(TRootEntity) != typeof(string) && typeof(TRootEntity) != typeof(byte[]) && isEnumerable)
                throw new NotSupportedException("Chain can't be defined as collection (except two types byte[] and string)");
        }

        public readonly ChainNode Root = new ChainNode(typeof(TRootEntity));

        private ChainMemberNode CurrentNode;

        private static ChainMemberNode AddIfAbsent(
            ChainNode parent,
            LambdaExpression lambdaExpression,
            string memberName,
            Type navigationType,
            Type navigationEnumerableType,
            bool isEnumerable
            )
        {
            var dictionary = parent.Children;
            if (memberName != null)
            {
                if (!dictionary.TryGetValue(memberName, out ChainMemberNode node))
                {
                    node = new ChainMemberNode(navigationType, lambdaExpression, memberName, isEnumerable, parent);
                    dictionary.Add(memberName, node);
                }
                return node;
            }
            else
            {
                if (lambdaExpression.Body is MemberExpression)
                {
                    var memberInfo = ((MemberExpression)lambdaExpression.Body).Member;
                    var name = memberInfo.Name;
                    if (!dictionary.TryGetValue(name, out ChainMemberNode node))
                    {
                        node = new ChainMemberNode(navigationType, lambdaExpression, name, isEnumerable, parent);
                        dictionary.Add(name, node);
                    }
                    return node;
                }
                else if (lambdaExpression.Body is MethodCallExpression)
                {
                    var methodInfo = ((MethodCallExpression)lambdaExpression.Body).Method;
                    var name = methodInfo.Name;
                    if (!dictionary.TryGetValue(name, out ChainMemberNode node))
                    {
                        node = new ChainMemberNode(navigationType, lambdaExpression, name, isEnumerable, parent);
                        dictionary.Add(name, node);
                    }
                    return node;
                }
                else
                    throw new NotSupportedException("Not supported type of expression: neither member, neither method call");
            }

        }

        public void ParseRoot<TEntity>(Expression<Func<TRootEntity, TEntity>> navigationExpression, string memberName)
        {
            var node = AddIfAbsent(Root, navigationExpression, memberName, typeof(TEntity), null, false);
            CurrentNode = node;
        }
        public void ParseRootEnumerable<TEntity>(Expression<Func<TRootEntity, IEnumerable<TEntity>>> navigationExpression, string memberName)
        {
            var node = AddIfAbsent(Root, navigationExpression, memberName,  typeof(TEntity), typeof(IEnumerable<TEntity>), true);
            CurrentNode = node;
        }
        public void Parse<TThenEntity, TEntity>(Expression<Func<TThenEntity, TEntity>> navigationExpression, bool changeCurrenNode, string memberName)
        {
            var node = AddIfAbsent(CurrentNode, navigationExpression, memberName, typeof(TEntity), null, false);
            if (changeCurrenNode)
                CurrentNode = node;
        }
        public void ParseEnumerable<TThenEntity, TEntity>(Expression<Func<TThenEntity, IEnumerable<TEntity>>> navigationExpression, bool changeCurrenNode, string memberName)
        {
            var node = AddIfAbsent(CurrentNode, navigationExpression, memberName, typeof(TEntity), typeof(IEnumerable<TEntity>), true);
            if (changeCurrenNode)
                CurrentNode = node;
        }
    }
}