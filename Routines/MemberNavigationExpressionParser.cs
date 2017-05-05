using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace Vse.Routines
{
    public class MemberNavigationExpressionParser<TRootEntity> : INavigationExpressionParser<TRootEntity>
    {
        public readonly List<MemberExpressionNode> Root = new List<MemberExpressionNode>();
        private MemberExpressionNode CurrentNode;

        public void ParseRoot<TEntity>(Expression<Func<TRootEntity, TEntity>> navigationExpression)
        {
            var name = MemberExpressionExtensions.GetMemberName(navigationExpression);
            var node = Root.FirstOrDefault(e => e.MemberName == name);
            if (node == null)
            {
                node = new MemberExpressionNode((MemberExpression)(navigationExpression.Body));
                Root.Add(node);
            }
            CurrentNode = node;
        }
        public void ParseRootEnumerable<TEntity>(Expression<Func<TRootEntity, IEnumerable<TEntity>>> navigationExpression)
        {
            ParseRoot(navigationExpression);
        }
        public void Parse<TThenEntity, TEntity>(Expression<Func<TThenEntity, TEntity>> navigationExpression)
        {
            var name = MemberExpressionExtensions.GetMemberName(navigationExpression);
            var node = CurrentNode.Children.FirstOrDefault(e => e.MemberName == name);
            if (node == null)
                node = new MemberExpressionNode((MemberExpression)(navigationExpression.Body));
            CurrentNode.Children.Add(node);
            CurrentNode = node;
        }
        public void ParseEnumerable<TThenEntity, TEntity>(Expression<Func<TThenEntity, IEnumerable<TEntity>>> navigationExpression)
        {
            Parse(navigationExpression);
        }
    }

    public class MemberExpressionNode
    {
        public readonly string MemberName;
        public readonly MemberExpression MemberExpression;
        public readonly List<MemberExpressionNode> Children = new List<MemberExpressionNode>();
        public MemberExpressionNode(MemberExpression memberExpression)
        {
            MemberName = memberExpression.Member.Name;
            MemberExpression = memberExpression;
        }
    }
}
