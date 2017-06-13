using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace Vse.Routines
{
    public interface IChainParser<TRootEntity>
    {
        void ParseRoot<TEntity>(Expression<Func<TRootEntity, TEntity>> getterExpression);
        void ParseRootEnumerable<TEntity>(Expression<Func<TRootEntity, IEnumerable<TEntity>>> getterExpression);
        void Parse<TThenEntity, TEntity>(Expression<Func<TThenEntity, TEntity>> getterExpression);
        void ParseEnumerable<TThenEntity, TEntity>(Expression<Func<TThenEntity, IEnumerable<TEntity>>> getterExpression);

        // TODO: Add navigation chain support to Nullable
        //void ParseRootNullable<TEntity>(Expression<Func<TRootEntity, TEntity?>> getterExpression) where TEntity : struct;
        //void ParseRootEnumerableNullable<TEntity>(Expression<Func<TRootEntity, IEnumerable<TEntity>>> getterExpression);
        //void ParseNullable<TThenEntity, TEntity>(Expression<Func<TThenEntity, TEntity?>> getterExpression) where TEntity : struct;
        //void ParseEnumerableNullable<TThenEntity, TEntity>(Expression<Func<TThenEntity, IEnumerable<TEntity>>> getterExpression);
    }
}
