using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace Vse.Routines
{
    public interface INavigationExpressionParser<TRootEntity>
    {
        void ParseRoot<TEntity>(Expression<Func<TRootEntity, TEntity>> navigationExpression);
        void ParseRootEnumerable<TEntity>(Expression<Func<TRootEntity, IEnumerable<TEntity>>> navigationExpression);
        void Parse<TThenEntity, TEntity>(Expression<Func<TThenEntity, TEntity>> navigationExpression);
        void ParseEnumerable<TThenEntity, TEntity>(Expression<Func<TThenEntity, IEnumerable<TEntity>>> navigationExpression);
    }
}
