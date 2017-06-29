using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace Vse.Routines
{
    public class MemberExpressionChainParser<TRootEntity> : IChainingState<TRootEntity>
    {
        public readonly List<MemberExpressionNode> Root = new List<MemberExpressionNode>();
        private MemberExpressionNode CurrentNode;

        public void ParseHead<TEntity>(Expression<Func<TRootEntity, TEntity>> expression)
        {
            var name = MemberExpressionExtensions.GetMemberName(expression);
            var node = Root.FirstOrDefault(e => e.MemberName == name);
            if (node == null)
            {
                node = new MemberExpressionNode((MemberExpression)(expression.Body));
                Root.Add(node);
            }
            CurrentNode = node;
        }

        public void ParseHeadEnumerable<TEntity>(Expression<Func<TRootEntity, IEnumerable<TEntity>>> expression)
        {
            ParseHead(expression);
        }

        public void Parse<TThenEntity, TEntity>(Expression<Func<TThenEntity, TEntity>> expression)
        {
            var name = MemberExpressionExtensions.GetMemberName(expression);
            var node = CurrentNode.Children.FirstOrDefault(e => e.MemberName == name);
            if (node == null)
                node = new MemberExpressionNode((MemberExpression)(expression.Body));
            CurrentNode.Children.Add(node);
            CurrentNode = node;
        }
        public void ParseEnumerable<TThenEntity, TEntity>(Expression<Func<TThenEntity, IEnumerable<TEntity>>> expression)
        {
            Parse(expression);
        }

        //public void ParseRootNullable<TEntity>(Expression<Func<TRootEntity, TEntity?>> getterExpression) where TEntity : struct
        //{
        //    throw new NotImplementedException();
        //}

        //public void ParseNullable<TThenEntity, TEntity>(Expression<Func<TThenEntity, TEntity?>> getterExpression) where TEntity : struct
        //{
        //    throw new NotImplementedException();
        //}
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
