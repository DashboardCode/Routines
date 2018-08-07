using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
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

    public class QueryableChainVisitor<TRootEntity> : IChainVisitor<TRootEntity>
    {
        public IQueryable<TRootEntity> Queryable { get; private set; }
        public string QueryableText { get; private set; } = "";
        public readonly Func<IQueryable<TRootEntity>, string, IQueryable<TRootEntity>> include;

        public QueryableChainVisitor(IQueryable<TRootEntity> rootQueryable, Func<IQueryable<TRootEntity>, string, IQueryable<TRootEntity>> include)
        {
            this.include = include;
            Queryable = rootQueryable ?? throw new ArgumentNullException(nameof(rootQueryable));
        }

        public void ParseRoot<TEntity>(Expression<Func<TRootEntity, TEntity>> expression, string memberName = null)
        {
            QueryableText = expression.GetMemberName();
            Queryable = include(Queryable, QueryableText);
        }

        public void ParseRootEnumerable<TEntity>(Expression<Func<TRootEntity, IEnumerable<TEntity>>> enumerableExpression, string memberName = null)
        {
            QueryableText = enumerableExpression.GetMemberName();
            Queryable = include(Queryable, QueryableText);
        }

        public void Parse<TMidEntity, TEntity>(Expression<Func<TMidEntity, TEntity>> expression, bool changeCurrentNode, string memberName)
        {
            string newQueryableText = QueryableText + "." + expression.GetMemberName();
            if (changeCurrentNode)
            {
                QueryableText = newQueryableText;
            }
            Queryable = include(Queryable, newQueryableText);
        }

        public void ParseEnumerable<TMidEntity, TEntity>(Expression<Func<TMidEntity, IEnumerable<TEntity>>> enumerableExpression, bool changeCurrentNode, string memberName = null)
        {
            string newQueryableText = QueryableText + "." + enumerableExpression.GetMemberName();
            if (changeCurrentNode)
            {
                QueryableText = newQueryableText;
            }
            Queryable = include(Queryable, newQueryableText);
        }
    }
}