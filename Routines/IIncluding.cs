using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace Vse.Routines
{
    public interface IIncluding<TRootEntity>
    {
        void Include<TEntity>(Expression<Func<TRootEntity, TEntity>> navigationExpression);
        void IncludeAll<TEntity>(Expression<Func<TRootEntity, IEnumerable<TEntity>>> navigationExpression);
        void ThenInclude<TThenEntity, TEntity>(Expression<Func<TThenEntity, TEntity>> navigationExpression);
        void ThenIncludeAll<TThenEntity, TEntity>(Expression<Func<TThenEntity, IEnumerable<TEntity>>> navigationExpression);
    }
}
