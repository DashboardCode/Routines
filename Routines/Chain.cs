using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace DashboardCode.Routines
{
    public class Chain<TRootEntity> 
    {
        protected readonly IChainVisitor<TRootEntity> chainVisitor;
        public Chain(IChainVisitor<TRootEntity> chainVisitor) => 
            this.chainVisitor = chainVisitor;
        
        public ThenChain<TRootEntity, TEntity> Include<TEntity>(Expression<Func<TRootEntity, TEntity>> lambdaExpression, string name=null)
        {
            chainVisitor.ParseRoot(lambdaExpression, name);
            return new ThenChain<TRootEntity, TEntity>(chainVisitor);
        }
        public ThenChain<TRootEntity, TEntity> IncludeAll<TEntity>(Expression<Func<TRootEntity, IEnumerable<TEntity>>> lambdaExpression, string name = null)
        {
            chainVisitor.ParseRootEnumerable(lambdaExpression, name);
            return new ThenChain<TRootEntity, TEntity>(chainVisitor);
        }
    }

    public class ThenChain<TRootEntity, TThenEntity> : Chain<TRootEntity> 
    {
        public ThenChain(IChainVisitor<TRootEntity> chainVisitor):base(chainVisitor)
        {
        }
        public ThenChain<TRootEntity, TEntity> ThenInclude<TEntity>(Expression<Func<TThenEntity, TEntity>> lambdaExpression, string name = null)
        {
            chainVisitor.Parse(lambdaExpression, true, name);
            return new ThenChain<TRootEntity, TEntity>(chainVisitor);
        }
        public ThenChain<TRootEntity, TThenEntity> ThenIncluding<TEntity>(Expression<Func<TThenEntity, TEntity>> lambdaExpression, string name = null)
        {
            chainVisitor.Parse(lambdaExpression, false, name);
            return new ThenChain<TRootEntity, TThenEntity>(chainVisitor);
        }
        public ThenChain<TRootEntity, TEntity> ThenIncludeAll<TEntity>(Expression<Func<TThenEntity, IEnumerable<TEntity>>> lambdaExpression, string name = null)
        {
            chainVisitor.ParseEnumerable(lambdaExpression, true, name);
            return new ThenChain<TRootEntity, TEntity>(chainVisitor);
        }
        public ThenChain<TRootEntity, TThenEntity> ThenIncludingAll<TEntity>(Expression<Func<TThenEntity, IEnumerable<TEntity>>> lambdaExpression, string name = null)
        {
            chainVisitor.ParseEnumerable(lambdaExpression, false,  name);
            return new ThenChain<TRootEntity, TThenEntity>(chainVisitor);
        }
    }
}
