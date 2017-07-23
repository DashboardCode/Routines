//using System;
//using System.Collections;
//using System.Collections.Generic;
//using System.Linq;
//using System.Linq.Expressions;
//using Microsoft.EntityFrameworkCore;
//using Microsoft.EntityFrameworkCore.Query;

//namespace DashboardCode.Includables2
//{
//    public interface IIncludable<TRootEntity> where TRootEntity : class
//    {
//        IThenIncludable<TRootEntity, TEntity> Include<TEntity>(Expression<Func<TRootEntity, TEntity>> navigationExpression);
//        IThenIncludable<TRootEntity, TEntity> IncludeAll<TEntity>(Expression<Func<TRootEntity, IEnumerable<TEntity>>> navigationExpression);
//    }
//    public interface IThenIncludable<TRootEntity, TThenEntity> : IIncludable<TRootEntity> where TRootEntity : class
//    {
//        IThenIncludable<TRootEntity, TEntity> ThenInclude<TEntity>(Expression<Func<TThenEntity, TEntity>> navigationExpression);
//        IThenIncludable<TRootEntity, TEntity> ThenIncludeAll<TEntity>(Expression<Func<TThenEntity, IEnumerable<TEntity>>> navigationExpression);
//    }
//    public static class EfCoreIncludablesExtensions
//    {
//        private static string GetName<T1, T2>(Expression<Func<T1, T2>> expression)
//        {
//            var memberExpression = (MemberExpression)expression.Body;
//            return memberExpression.Member.Name;
//        }

//        public static IQueryable<T> Include<T>(this IQueryable<T> query, Action<IIncludable<T>> addIncludes) where T : class
//        {
//            var includable = new EfCoreQueryIncludable<T>(query);
//            addIncludes?.Invoke(includable);
//            return includable.ResultQueryable;
//        }

//        #region Detach
//        public static void Detach<T>(this DbContext context, T entity, Action<IIncludable<T>> addIncludes) where T : class
//        {
//            context.Entry(entity).State = EntityState.Detached;
//            var dbSet = context.Set<T>();
//            Detach(entity, addIncludes);
//        }

//        private static void Detach<T>(object entity, Action<IIncludable<T>> addIncludes) where T : class
//        {
//            var includable = new DetachIncludable<T>();
//            addIncludes?.Invoke(includable);
//            var pathes = includable.Pathes;
//            Detach(entity, pathes);
//        }
//        public static void Detach(object entity, List<string[]> allowedPaths)
//        {
//            var type = entity.GetType();
//            if (entity is IEnumerable)
//            {
//                foreach (var value in (IEnumerable)entity)
//                {
//                    if (value != null)
//                    {
//                        Detach(value, allowedPaths);
//                    }
//                }
//            }
//            else
//            {
//                var properties = type.GetProperties();
//                foreach (var propertyInfo in properties)
//                {
//                    if (propertyInfo.CanRead && propertyInfo.CanWrite && propertyInfo.GetIndexParameters().Length == 0)
//                    {
//                        if (!SystemTypes.Contains(propertyInfo.PropertyType))
//                        {
//                            string propertyName = propertyInfo.Name;
//                            var value = propertyInfo.GetValue(entity, null);
//                            if (value != null)
//                            {
//                                var selectedPaths = allowedPaths.Where(e => e[0] == propertyName);
//                                if (selectedPaths.Count() > 0)
//                                {
//                                    var newPaths = new List<string[]>();
//                                    foreach (var path in allowedPaths)
//                                    {
//                                        var root = path[0];
//                                        if (root == propertyName)
//                                        {
//                                            if (path.Length > 1)
//                                            {
//                                                var newPath = new string[path.Length - 1];
//                                                newPath = path.Skip(1).ToArray();
//                                                newPaths.Add(newPath);
//                                            }
//                                        }
//                                    }
//                                    Detach(value, newPaths);
//                                }
//                                else
//                                {
//                                    propertyInfo.SetValue(entity, null);
//                                }
//                            }
//                        }
//                    }
//                }
//            }
//        }

//        private static readonly Type[] SystemTypes = new[]
//        {
//                typeof(bool),
//                typeof(bool?),
//                typeof(byte),
//                typeof(byte?),
//                typeof(char),
//                typeof(char?),
//                typeof(decimal),
//                typeof(decimal?),
//                typeof(double),
//                typeof(double?),
//                typeof(float),
//                typeof(float?),
//                typeof(int),
//                typeof(int?),
//                typeof(long),
//                typeof(long?),
//                typeof(sbyte),
//                typeof(sbyte?),
//                typeof(short),
//                typeof(short?),
//                typeof(uint),
//                typeof(uint?),
//                typeof(ulong),
//                typeof(ulong?),
//                typeof(ushort),
//                typeof(ushort?),
//                typeof(string),
//                typeof(DateTime),
//                typeof(DateTime?),
//                typeof(DateTimeOffset),
//                typeof(DateTimeOffset?),
//                typeof(Guid),
//                typeof(Guid?),
//                typeof(TimeSpan),
//                typeof(TimeSpan?)
//            };
//        #endregion

//        #region DetachIncludable
//        public class DetachIncludable<TRootEntity> : IIncludable<TRootEntity> where TRootEntity : class
//        {
//            public readonly List<string[]> Pathes = new List<string[]>();

//            internal void AddPath(string[] path)
//            {
//                if (!Pathes.Any(e => e.SequenceEqual(path)))
//                    Pathes.Add(path);
//            }

//            public IThenIncludable<TRootEntity, TEntity> Include<TEntity>(Expression<Func<TRootEntity, TEntity>> navigationExpression)
//            {
//                var name = GetName(navigationExpression);
//                var path = new[] { name };
//                AddPath(path);
//                return new DetachThenIncludable<TRootEntity, TEntity>(path, AddPath);
//            }

//            public IThenIncludable<TRootEntity, TEntity> IncludeAll<TEntity>(Expression<Func<TRootEntity, IEnumerable<TEntity>>> navigationExpression)
//            {
//                var name = GetName(navigationExpression);
//                var path = new[] { name };
//                AddPath(path);
//                return new DetachThenIncludable<TRootEntity, TEntity>(path, AddPath);
//            }
//        }
//        public class DetachThenIncludable<TRootEntity, TThenEntity> : IThenIncludable<TRootEntity, TThenEntity> where TRootEntity : class
//        {
//            private Action<string[]> addPath;
//            private string[] path;
//            public DetachThenIncludable(string[] path, Action<string[]> addPath)
//            {
//                this.path = path;
//                this.addPath = addPath;
//            }

//            public IThenIncludable<TRootEntity, TEntity> IncludeAll<TEntity>(Expression<Func<TRootEntity, IEnumerable<TEntity>>> navigationExpression)
//            {
//                var name = GetName(navigationExpression);
//                var path = new[] { name };
//                addPath(path);
//                return new DetachThenIncludable<TRootEntity, TEntity>(path, addPath);
//            }

//            public IThenIncludable<TRootEntity, TEntity> Include<TEntity>(Expression<Func<TRootEntity, TEntity>> navigationExpression)
//            {
//                var name = GetName(navigationExpression);
//                var path = new[] { name };
//                addPath(path);
//                return new DetachThenIncludable<TRootEntity, TEntity>(path, addPath);
//            }

//            public IThenIncludable<TRootEntity, TEntity> ThenIncludeAll<TEntity>(Expression<Func<TThenEntity, IEnumerable<TEntity>>> navigationExpression)
//            {
//                var name = GetName(navigationExpression);
//                var newPath = path.Concat(new[] { name }).ToArray();
//                addPath(newPath);
//                return new DetachThenIncludable<TRootEntity, TEntity>(newPath, addPath);
//            }

//            public IThenIncludable<TRootEntity, TEntity> ThenInclude<TEntity>(Expression<Func<TThenEntity, TEntity>> navigationExpression)
//            {
//                var name = GetName(navigationExpression);
//                var newPath = path.Concat(new[] { name }).ToArray();
//                addPath(newPath);
//                return new DetachThenIncludable<TRootEntity, TEntity>(path, addPath);
//            }
//        }
//        #endregion

//        #region QueryIncludable<TEntity>
//        public class EfCoreQueryIncludable<TEntity> : IIncludable<TEntity> where TEntity : class
//        {
//            public IQueryable<TEntity> ResultQueryable { get; private set; }

//            public EfCoreQueryIncludable(IQueryable<TEntity> rootQuery)
//            {
//                ResultQueryable = rootQuery;
//            }

//            internal void SetResultQueryable(IQueryable<TEntity> query)
//            {
//                ResultQueryable = query;
//            }

//            public IThenIncludable<TEntity, TEntityProperty> IncludeAll<TEntityProperty>(Expression<Func<TEntity, IEnumerable<TEntityProperty>>> navigationExpression)
//            {
//                var furtherQuery = ResultQueryable.Include(navigationExpression);
//                return new QueryCollectionThenIncludable<TEntity, TEntityProperty>(furtherQuery, SetResultQueryable);
//            }

//            public IThenIncludable<TEntity, TEntityProperty> Include<TEntityProperty>(Expression<Func<TEntity, TEntityProperty>> navigationExpression)
//            {
//                var furtherQuery = ResultQueryable.Include(navigationExpression);
//                return new QueryThenIncludable<TEntity, TEntityProperty>(furtherQuery, SetResultQueryable);
//            }

//        }
//        public class QueryThenIncludable<TRootEntity, TMidEntity> : IThenIncludable<TRootEntity, TMidEntity> where TRootEntity : class
//        {
//            private IIncludableQueryable<TRootEntity, TMidEntity> query;
//            private Action<IQueryable<TRootEntity>> setResult;
//            internal QueryThenIncludable(IIncludableQueryable<TRootEntity, TMidEntity> query, Action<IQueryable<TRootEntity>> setResult)
//            {
//                this.query = query;
//                this.setResult = setResult;
//                setResult(query);
//            }
//            public IThenIncludable<TRootEntity, TEntity> IncludeAll<TEntity>(Expression<Func<TRootEntity, IEnumerable<TEntity>>> navigationExpression)
//            {
//                var furtherQuery = query.Include(navigationExpression);
//                return new QueryCollectionThenIncludable<TRootEntity, TEntity>(furtherQuery, setResult);
//            }

//            public IThenIncludable<TRootEntity, TEntity> Include<TEntity>(Expression<Func<TRootEntity, TEntity>> navigationExpression)
//            {
//                var furtherQuery = query.Include(navigationExpression);
//                return new QueryThenIncludable<TRootEntity, TEntity>(furtherQuery, setResult);
//            }

//            public IThenIncludable<TRootEntity, TEntity> ThenIncludeAll<TEntity>(Expression<Func<TMidEntity, IEnumerable<TEntity>>> navigationExpression)
//            {
//                var furtherQuery = query.ThenInclude(navigationExpression);
//                return new QueryCollectionThenIncludable<TRootEntity, TEntity>(furtherQuery, setResult);
//            }

//            public IThenIncludable<TRootEntity, TEntity> ThenInclude<TEntity>(Expression<Func<TMidEntity, TEntity>> navigationExpression)
//            {
//                var furtherQuery = query.ThenInclude(navigationExpression);
//                return new QueryThenIncludable<TRootEntity, TEntity>(furtherQuery, setResult);
//            }
//        }
//        public class QueryCollectionThenIncludable<TRootEntity, TMidEntity> : IThenIncludable<TRootEntity, TMidEntity> where TRootEntity : class
//        {
//            private IIncludableQueryable<TRootEntity, IEnumerable<TMidEntity>> query;
//            private Action<IQueryable<TRootEntity>> setResult;
//            internal QueryCollectionThenIncludable(IIncludableQueryable<TRootEntity, IEnumerable<TMidEntity>> query, Action<IQueryable<TRootEntity>> setResult)
//            {
//                this.query = query;
//                this.setResult = setResult;
//                setResult(query);
//            }
//            public IThenIncludable<TRootEntity, TEntity> IncludeAll<TEntity>(Expression<Func<TRootEntity, IEnumerable<TEntity>>> navigationExpression)
//            {
//                var furtherQuery = query.Include(navigationExpression);
//                return new QueryCollectionThenIncludable<TRootEntity, TEntity>(furtherQuery, setResult);
//            }

//            public IThenIncludable<TRootEntity, TEntity> Include<TEntity>(Expression<Func<TRootEntity, TEntity>> navigationExpression)
//            {
//                var furtherQuery = query.Include(navigationExpression);
//                return new QueryThenIncludable<TRootEntity, TEntity>(furtherQuery, setResult);
//            }

//            public IThenIncludable<TRootEntity, TEntity> ThenIncludeAll<TEntity>(Expression<Func<TMidEntity, IEnumerable<TEntity>>> navigationExpression)
//            {
//                var furtherQuery = query.ThenInclude(navigationExpression);
//                return new QueryCollectionThenIncludable<TRootEntity, TEntity>(furtherQuery, setResult);
//            }

//            public IThenIncludable<TRootEntity, TEntity> ThenInclude<TEntity>(Expression<Func<TMidEntity, TEntity>> navigationExpression)
//            {
//                var furtherQuery = query.ThenInclude(navigationExpression);
//                return new QueryThenIncludable<TRootEntity, TEntity>(furtherQuery, setResult);
//            }
//        }
//        #endregion
//    }
//}
