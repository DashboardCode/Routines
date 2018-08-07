//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Linq.Expressions;
//using System.Data.Entity;

//namespace DashboardCode.Routines.Storage.Ef6
//{
//    public class QueryableChainVisitor<TRootEntity> : IChainVisitor<TRootEntity> where TRootEntity : class
//    {
//        public IQueryable<TRootEntity> Queryable { get; private set; }
//        public string QueryableText { get; private set; } = "";
//        public bool isEnumerable;

//        public QueryableChainVisitor(IQueryable<TRootEntity> rootQueryable) =>
//            Queryable = rootQueryable ?? throw new ArgumentNullException(nameof(rootQueryable));

//        public void ParseRoot<TEntity>(Expression<Func<TRootEntity, TEntity>> expression, string memberName = null)
//        {
//            QueryableText = expression.GetMemberName();
//            Queryable = Queryable.Include(QueryableText);
//        }
//        public void ParseRootEnumerable<TEntity>(Expression<Func<TRootEntity, IEnumerable<TEntity>>> enumerableExpression, string memberName = null)
//        {
//            QueryableText = enumerableExpression.GetMemberName();
//            Queryable = Queryable.Include(QueryableText);
//        }
//        public void Parse<TMidEntity, TEntity>(Expression<Func<TMidEntity, TEntity>> expression, bool changeCurrentNode, string memberName)
//        {
//            string newQueryableText = QueryableText + "." + expression.GetMemberName();
//            if (changeCurrentNode)
//            {
//                QueryableText = newQueryableText;
//            }
//            Queryable = Queryable.Include(newQueryableText);
//        }
//        public void ParseEnumerable<TMidEntity, TEntity>(Expression<Func<TMidEntity, IEnumerable<TEntity>>> enumerableExpression, bool changeCurrentNode, string memberName = null)
//        {
//            string newQueryableText = QueryableText + "." + enumerableExpression.GetMemberName();
//            if (changeCurrentNode)
//            {
//                QueryableText = newQueryableText;
//            }
//            Queryable = Queryable.Include(newQueryableText);
//        }
//    }
//}