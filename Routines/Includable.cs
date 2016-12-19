using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace Vse.Routines
{
    public class Includable<TRootEntity> 
    {
        protected readonly IIncluding<TRootEntity> includingProcess;
        public Includable(IIncluding<TRootEntity> includingProcess)
        {
            this.includingProcess = includingProcess;
        }
        public ThenIncludable<TRootEntity, TEntity> Include<TEntity>(Expression<Func<TRootEntity, TEntity>> navigationExpression)
        {
            includingProcess.Include(navigationExpression);
            return new ThenIncludable<TRootEntity, TEntity>(includingProcess);
        }
        public ThenIncludable<TRootEntity, TEntity> IncludeAll<TEntity>(Expression<Func<TRootEntity, IEnumerable<TEntity>>> navigationExpression)
        {
            includingProcess.IncludeAll(navigationExpression);
            return new ThenIncludable<TRootEntity, TEntity>(includingProcess);
        }
    }
    public class ThenIncludable<TRootEntity, TThenEntity> : Includable<TRootEntity> 
    {
        public ThenIncludable(IIncluding<TRootEntity> includingProcess):base(includingProcess)
        {
        }
        public ThenIncludable<TRootEntity, TEntity> ThenInclude<TEntity>(Expression<Func<TThenEntity, TEntity>> navigationExpression)
        {
            includingProcess.ThenInclude(navigationExpression);
            return new ThenIncludable<TRootEntity, TEntity>(includingProcess);
        }
        public ThenIncludable<TRootEntity, TEntity> ThenIncludeAll<TEntity>(Expression<Func<TThenEntity, IEnumerable<TEntity>>> navigationExpression)
        {
            includingProcess.ThenIncludeAll(navigationExpression);
            return new ThenIncludable<TRootEntity, TEntity>(includingProcess);
        }
    }
}
