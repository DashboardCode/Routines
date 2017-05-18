using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace Vse.Routines
{
    public interface INExpParser<TRootEntity>
    {
        void ParseRoot<TEntity>(Expression<Func<TRootEntity, TEntity>> getterExpression);
        void ParseRootEnumerable<TEntity>(Expression<Func<TRootEntity, IEnumerable<TEntity>>> getterExpression);
        void Parse<TThenEntity, TEntity>(Expression<Func<TThenEntity, TEntity>> getterExpression);
        void ParseEnumerable<TThenEntity, TEntity>(Expression<Func<TThenEntity, IEnumerable<TEntity>>> getterExpression);
    }
}
