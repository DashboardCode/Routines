using System;
using System.Collections.Generic;
using System.Linq.Expressions;

using static System.Console;
namespace DashboardCode.Routines
{
    class Program
    {
        static void Main(string[] args)
        {
            //Example 1   
            //var myData = "Custom Data";
            //var myData2 = myData is string ? "String" : "Not a string";
            //var myData3 = myData is string a ? a : "Not a String";
            //WriteLine(myData2);
            //WriteLine(myData3);
            ////Example 2   
            //var x = 10;
            dynamic y = 0b1001;
            var sum = y is ValueType? $"Struct" : "Invalid data";
            //WriteLine(sum);
        }
    }

    public class Chain<TRootEntity> 
    {
        protected readonly IChainVisitor<TRootEntity> chainVisitor;
        public Chain(IChainVisitor<TRootEntity> chainVisitor)
        {
            this.chainVisitor = chainVisitor;
        }
        public ThenChain<TRootEntity, TEntity> Include<TEntity>(Expression<Func<TRootEntity, TEntity>> lambdaExpression)
        {
            chainVisitor.ParseRoot(lambdaExpression);
            return new ThenChain<TRootEntity, TEntity>(chainVisitor);
        }
        public ThenChain<TRootEntity, TEntity> IncludeAll<TEntity>(Expression<Func<TRootEntity, IEnumerable<TEntity>>> lambdaExpression)
        {
            chainVisitor.ParseRootEnumerable(lambdaExpression);
            return new ThenChain<TRootEntity, TEntity>(chainVisitor);
        }
    }

    public class ThenChain<TRootEntity, TThenEntity> : Chain<TRootEntity> 
    {
        public ThenChain(IChainVisitor<TRootEntity> chainVisitor):base(chainVisitor)
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
