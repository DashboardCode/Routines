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

        private ChainPropertyNode CurrentNode;

        private static ChainPropertyNode AddIfAbsent(
            ChainNode parent,
            LambdaExpression expression,
            Type navigationType,
            Type navigationEnumerableType,
            bool isEnumerable
            )
        {
            var dictionary = parent.Children;

            var propertyInfo = ((MemberExpression)expression.Body).Member as PropertyInfo;
            var name = propertyInfo.Name;
            if (!dictionary.TryGetValue(name, out ChainPropertyNode node))
            {
                node = new ChainPropertyNode(navigationType, expression, /*propertyInfo,*/ name, isEnumerable, parent);
                dictionary.Add(name, node);
            }
            return node;
        }

        public void ParseRoot<TEntity>(Expression<Func<TRootEntity, TEntity>> navigationExpression)
        {
            var node = AddIfAbsent(Root, navigationExpression,  typeof(TEntity), null, false);
            CurrentNode = node;
        }
        public void ParseRootEnumerable<TEntity>(Expression<Func<TRootEntity, IEnumerable<TEntity>>> navigationExpression)
        {
            var node = AddIfAbsent(Root, navigationExpression,  typeof(TEntity), typeof(IEnumerable<TEntity>), true);
            CurrentNode = node;
        }
        public void Parse<TThenEntity, TEntity>(Expression<Func<TThenEntity, TEntity>> navigationExpression)
        {
            var node = AddIfAbsent(CurrentNode, navigationExpression, typeof(TEntity), null, false);
            CurrentNode = node;
        }
        public void ParseEnumerable<TThenEntity, TEntity>(Expression<Func<TThenEntity, IEnumerable<TEntity>>> navigationExpression)
        {
            var node = AddIfAbsent(CurrentNode, navigationExpression, typeof(TEntity), typeof(IEnumerable<TEntity>), true);
            CurrentNode = node;
        }
    }
}