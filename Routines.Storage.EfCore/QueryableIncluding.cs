using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;

namespace Vse.Routines.Storage.EfCore
{
    public class QueryableIncluding<TRootEntity> : IChainParser<TRootEntity> where TRootEntity : class
    {
        public IQueryable<TRootEntity> Queryable { get; private set; }
        public bool isEnumerable;

        public QueryableIncluding(IQueryable<TRootEntity> rootQueryable)
        {
            if (rootQueryable == null)
                throw new ArgumentNullException(nameof(rootQueryable));
            Queryable = rootQueryable;
        }
        public void ParseRoot<TEntity>(Expression<Func<TRootEntity, TEntity>> expression)
        {
            Queryable = EntityFrameworkQueryableExtensions.Include(Queryable, expression);
            isEnumerable = false;
        }
        public void ParseRootEnumerable<TEntity>(Expression<Func<TRootEntity, IEnumerable<TEntity>>> enumerableExpression)
        {
            Queryable = EntityFrameworkQueryableExtensions.Include(Queryable, enumerableExpression);
            isEnumerable = true;
        }
        public void Parse<TMidEntity, TEntity>(Expression<Func<TMidEntity, TEntity>> expression)
        {
            if(isEnumerable)
                Queryable = EntityFrameworkQueryableExtensions.ThenInclude(
                    (IIncludableQueryable<TRootEntity, IEnumerable<TMidEntity>>)Queryable, expression);
            else
                Queryable = EntityFrameworkQueryableExtensions.ThenInclude(
                    (IIncludableQueryable<TRootEntity, TMidEntity>)Queryable, expression);
            isEnumerable = false;
        }
        public void ParseEnumerable<TMidEntity, TEntity>(Expression<Func<TMidEntity, IEnumerable<TEntity>>> enumerableExpression)
        {
            if (isEnumerable)
                Queryable = EntityFrameworkQueryableExtensions.ThenInclude(
                    (IIncludableQueryable<TRootEntity, IEnumerable<TMidEntity>>)Queryable, enumerableExpression);
            else
                Queryable = EntityFrameworkQueryableExtensions.ThenInclude(
                    (IIncludableQueryable<TRootEntity, TMidEntity>)Queryable, enumerableExpression);
            isEnumerable = true;
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
}
