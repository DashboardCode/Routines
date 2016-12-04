using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace Vse.Routines
{
    public class Includable<TRootEntity> where TRootEntity : class
    {
        protected readonly IIncluding<TRootEntity> includingProcess;
        public Includable(IIncluding<TRootEntity> includingProcess)
        {
            this.includingProcess = includingProcess;
        }
        public MidIncludable<TRootEntity, TEntity> Include<TEntity>(Expression<Func<TRootEntity, TEntity>> navigationExpression)
        {
            includingProcess.Include(navigationExpression);
            return new MidIncludable<TRootEntity, TEntity>(includingProcess);
        }
        public MidIncludable<TRootEntity, TEntity> IncludeAll<TEntity>(Expression<Func<TRootEntity, IEnumerable<TEntity>>> navigationExpression)
        {
            includingProcess.IncludeAll(navigationExpression);
            return new MidIncludable<TRootEntity, TEntity>(includingProcess);
        }
    }
    public class MidIncludable<TRootEntity, TMidEntity> : Includable<TRootEntity> where TRootEntity : class
    {
        public MidIncludable(IIncluding<TRootEntity> includingProcess):base(includingProcess)
        {
        }
        public MidIncludable<TRootEntity, TEntity> ThenInclude<TEntity>(Expression<Func<TMidEntity, TEntity>> navigationExpression)
        {
            includingProcess.ThenInclude(navigationExpression);
            return new MidIncludable<TRootEntity, TEntity>(includingProcess);
        }
        public MidIncludable<TRootEntity, TEntity> ThenIncludeAll<TEntity>(Expression<Func<TMidEntity, IEnumerable<TEntity>>> navigationExpression)
        {
            includingProcess.ThenIncludeAll(navigationExpression);
            return new MidIncludable<TRootEntity, TEntity>(includingProcess);
        }
    }
}
