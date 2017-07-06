﻿using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace Vse.Routines
{
    public class Chain<TRootEntity> 
    {
        protected readonly IChainVisitor<TRootEntity> chainVisitor;
        public Chain(IChainVisitor<TRootEntity> chainVisitor)
        {
            this.chainVisitor = chainVisitor;
        }
        public ThenChain<TRootEntity, TEntity> Include<TEntity>(Expression<Func<TRootEntity, TEntity>> lambdaExpression)
        {
            chainVisitor.ParseHead(lambdaExpression);
            return new ThenChain<TRootEntity, TEntity>(chainVisitor);
        }

        public ThenChain<TRootEntity, TEntity> IncludeAll<TEntity>(Expression<Func<TRootEntity, IEnumerable<TEntity>>> lambdaExpression)
        {
            chainVisitor.ParseHeadEnumerable(lambdaExpression);
            return new ThenChain<TRootEntity, TEntity>(chainVisitor);
        }
    }

    public class ThenChain<TRootEntity, TThenEntity> : Chain<TRootEntity> 
    {
        public ThenChain(IChainVisitor<TRootEntity> chainingState):base(chainingState)
        {
        }
        public ThenChain<TRootEntity, TEntity> ThenInclude<TEntity>(Expression<Func<TThenEntity, TEntity>> lambdaExpression)
        {
            chainVisitor.Parse(lambdaExpression);
            return new ThenChain<TRootEntity, TEntity>(chainVisitor);
        }
        public ThenChain<TRootEntity, TEntity> ThenIncludeAll<TEntity>(Expression<Func<TThenEntity, IEnumerable<TEntity>>> lambdaExpression)
        {
            chainVisitor.ParseEnumerable(lambdaExpression);
            return new ThenChain<TRootEntity, TEntity>(chainVisitor);
        }
    }
}