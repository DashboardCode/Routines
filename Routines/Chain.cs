using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace Vse.Routines
{
    public class Chain<TRootEntity> 
    {
        protected readonly IChainingState<TRootEntity> navigationExpressionParser;
        public Chain(IChainingState<TRootEntity> navigationExpressionParser)
        {
            this.navigationExpressionParser = navigationExpressionParser;
        }
        public ThenChain<TRootEntity, TEntity> Include<TEntity>(Expression<Func<TRootEntity, TEntity>> navigationExpression)
        {
            navigationExpressionParser.ParseHead(navigationExpression);
            return new ThenChain<TRootEntity, TEntity>(navigationExpressionParser);
        }

        public ThenChain<TRootEntity, TEntity> IncludeAll<TEntity>(Expression<Func<TRootEntity, IEnumerable<TEntity>>> navigationExpression)
        {
            navigationExpressionParser.ParseHeadEnumerable(navigationExpression);
            return new ThenChain<TRootEntity, TEntity>(navigationExpressionParser);
        }

        //public ThenIncludable<TRootEntity, TEntity> IncludeNullable<TEntity>(Expression<Func<TRootEntity, TEntity?>> navigationExpression) where TEntity:struct
        //{
        //    navigationExpressionParser.ParseRootNullable(navigationExpression);
        //    return new ThenIncludable<TRootEntity, TEntity>(navigationExpressionParser);
        //}

    }

    public class ThenChain<TRootEntity, TThenEntity> : Chain<TRootEntity> 
    {
        public ThenChain(IChainingState<TRootEntity> navigationExpressionParser):base(navigationExpressionParser)
        {
        }
        public ThenChain<TRootEntity, TEntity> ThenInclude<TEntity>(Expression<Func<TThenEntity, TEntity>> navigationExpression)
        {
            navigationExpressionParser.Parse(navigationExpression);
            return new ThenChain<TRootEntity, TEntity>(navigationExpressionParser);
        }
        public ThenChain<TRootEntity, TEntity> ThenIncludeAll<TEntity>(Expression<Func<TThenEntity, IEnumerable<TEntity>>> navigationExpression)
        {
            navigationExpressionParser.ParseEnumerable(navigationExpression);
            return new ThenChain<TRootEntity, TEntity>(navigationExpressionParser);
        }
    }
}
