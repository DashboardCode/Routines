using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace Vse.Routines
{
    public class Includable<TRootEntity> 
    {
        protected readonly INExpParser<TRootEntity> navigationExpressionParser;
        public Includable(INExpParser<TRootEntity> navigationExpressionParser)
        {
            this.navigationExpressionParser = navigationExpressionParser;
        }
        public ThenIncludable<TRootEntity, TEntity> Include<TEntity>(Expression<Func<TRootEntity, TEntity>> navigationExpression)
        {
            navigationExpressionParser.ParseRoot(navigationExpression);
            return new ThenIncludable<TRootEntity, TEntity>(navigationExpressionParser);
        }
        public ThenIncludable<TRootEntity, TEntity> IncludeAll<TEntity>(Expression<Func<TRootEntity, IEnumerable<TEntity>>> navigationExpression)
        {
            navigationExpressionParser.ParseRootEnumerable(navigationExpression);
            return new ThenIncludable<TRootEntity, TEntity>(navigationExpressionParser);
        }
    }

    public class ThenIncludable<TRootEntity, TThenEntity> : Includable<TRootEntity> 
    {
        public ThenIncludable(INExpParser<TRootEntity> navigationExpressionParser):base(navigationExpressionParser)
        {
        }
        public ThenIncludable<TRootEntity, TEntity> ThenInclude<TEntity>(Expression<Func<TThenEntity, TEntity>> navigationExpression)
        {
            navigationExpressionParser.Parse(navigationExpression);
            return new ThenIncludable<TRootEntity, TEntity>(navigationExpressionParser);
        }
        public ThenIncludable<TRootEntity, TEntity> ThenIncludeAll<TEntity>(Expression<Func<TThenEntity, IEnumerable<TEntity>>> navigationExpression)
        {
            navigationExpressionParser.ParseEnumerable(navigationExpression);
            return new ThenIncludable<TRootEntity, TEntity>(navigationExpressionParser);
        }
    }
}
