using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace Vse.Routines
{
    public interface IIncluding<TRootEntity> where TRootEntity : class
    {
        void Include<TEntity>(Expression<Func<TRootEntity, TEntity>> navigationExpression);
        void IncludeAll<TEntity>(Expression<Func<TRootEntity, IEnumerable<TEntity>>> navigationExpression);
        void ThenInclude<TMidProperty, TEntity>(Expression<Func<TMidProperty, TEntity>> navigationExpression);
        void ThenIncludeAll<TMidProperty, TEntity>(Expression<Func<TMidProperty, IEnumerable<TEntity>>> navigationExpression);
    }
}
