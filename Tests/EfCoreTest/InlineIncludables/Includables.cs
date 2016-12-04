using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;

namespace Vse.Includables2
{
    public interface IIncludable<TRootEntity> where TRootEntity : class
    {
        IMidIncludable<TRootEntity, TEntity> Include<TEntity>(Expression<Func<TRootEntity, TEntity>> navigationExpression);
        IMidIncludable<TRootEntity, TEntity> IncludeAll<TEntity>(Expression<Func<TRootEntity, IEnumerable<TEntity>>> navigationExpression);
    }
    public interface IMidIncludable<TRootEntity, TMidEntity> : IIncludable<TRootEntity> where TRootEntity : class
    {
        IMidIncludable<TRootEntity, TEntity> ThenInclude<TEntity>(Expression<Func<TMidEntity, TEntity>> navigationExpression);
        IMidIncludable<TRootEntity, TEntity> ThenIncludeAll<TEntity>(Expression<Func<TMidEntity, IEnumerable<TEntity>>> navigationExpression);
    }
    public static class EfCoreIncludablesExtensions
    {
        private static string GetName<T1, T2>(Expression<Func<T1, T2>> expression)
        {
            var memberExpression = (MemberExpression)expression.Body;
            return memberExpression.Member.Name;
        }

        public static IQueryable<T> Include<T>(this IQueryable<T> query, Action<IIncludable<T>> addIncludes) where T : class
        {
            var includable = new EfCoreQueryIncludable<T>(query);
            addIncludes?.Invoke(includable);
            return includable.ResultQueryable;
        }

        #region Detach
        public static void Detach<T>(this DbContext context, T entity, Action<IIncludable<T>> addIncludes) where T : class
        {
            context.Entry(entity).State = EntityState.Detached;
            var dbSet = context.Set<T>();
            Detach(entity, addIncludes);
        }

        private static void Detach<T>(object entity, Action<IIncludable<T>> addIncludes) where T : class
        {
            var includable = new DetachIncludable<T>();
            addIncludes?.Invoke(includable);
            var pathes = includable.Pathes;
            Detach(entity, pathes);
        }
        public static void Detach(object entity, List<string[]> allowedPaths)
        {
            var type = entity.GetType();
            if (entity is IEnumerable)
            {
                foreach (var value in (IEnumerable)entity)
                {
                    if (value != null)
                    {
                        Detach(value, allowedPaths);
                    }
                }
            }
            else
            {
                var properties = type.GetProperties();
                foreach (var propertyInfo in properties)
                {
                    if (propertyInfo.CanRead && propertyInfo.CanWrite && propertyInfo.GetIndexParameters().Length == 0)
                    {
                        if (!SystemTypes.Contains(propertyInfo.PropertyType))
                        {
                            string propertyName = propertyInfo.Name;
                            var value = propertyInfo.GetValue(entity, null);
                            if (value != null)
                            {
                                var selectedPaths = allowedPaths.Where(e => e[0] == propertyName);
                                if (selectedPaths.Count() > 0)
                                {
                                    var newPaths = new List<string[]>();
                                    foreach (var path in allowedPaths)
                                    {
                                        var root = path[0];
                                        if (root == propertyName)
                                        {
                                            if (path.Length > 1)
                                            {
                                                var newPath = new string[path.Length - 1];
                                                newPath = path.Skip(1).ToArray();
                                                newPaths.Add(newPath);
                                            }
                                        }
                                    }
                                    Detach(value, newPaths);
                                }
                                else
                                {
                                    propertyInfo.SetValue(entity, null);
                                }
                            }
                        }
                    }
                }
            }
        }

        private static readonly Type[] SystemTypes = new[]
        {
                typeof(bool),
                typeof(bool?),
                typeof(byte),
                typeof(byte?),
                typeof(char),
                typeof(char?),
                typeof(decimal),
                typeof(decimal?),
                typeof(double),
                typeof(double?),
                typeof(float),
                typeof(float?),
                typeof(int),
                typeof(int?),
                typeof(long),
                typeof(long?),
                typeof(sbyte),
                typeof(sbyte?),
                typeof(short),
                typeof(short?),
                typeof(uint),
                typeof(uint?),
                typeof(ulong),
                typeof(ulong?),
                typeof(ushort),
                typeof(ushort?),
                typeof(string),
                typeof(DateTime),
                typeof(DateTime?),
                typeof(DateTimeOffset),
                typeof(DateTimeOffset?),
                typeof(Guid),
                typeof(Guid?),
                typeof(TimeSpan),
                typeof(TimeSpan?)
            };
        #endregion

        #region DetachIncludable
        public class DetachIncludable<TRootEntity> : IIncludable<TRootEntity> where TRootEntity : class
        {
            public readonly List<string[]> Pathes = new List<string[]>();

            internal void AddPath(string[] path)
            {
                if (!Pathes.Any(e => e.SequenceEqual(path)))
                    Pathes.Add(path);
            }

            public IMidIncludable<TRootEntity, TEntity> Include<TEntity>(Expression<Func<TRootEntity, TEntity>> navigationExpression)
            {
                var name = GetName(navigationExpression);
                var path = new[] { name };
                AddPath(path);
                return new DetachMidIncludable<TRootEntity, TEntity>(path, AddPath);
            }

            public IMidIncludable<TRootEntity, TEntity> IncludeAll<TEntity>(Expression<Func<TRootEntity, IEnumerable<TEntity>>> navigationExpression)
            {
                var name = GetName(navigationExpression);
                var path = new[] { name };
                AddPath(path);
                return new DetachMidIncludable<TRootEntity, TEntity>(path, AddPath);
                //return new DetachCollectionMidIncludable<TRootEntity, TEntity>(path, AddPath);
            }
        }
        public class DetachMidIncludable<TRootEntity, TMidProperty> : IMidIncludable<TRootEntity, TMidProperty> where TRootEntity : class
        {
            private Action<string[]> addPath;
            private string[] path;
            public DetachMidIncludable(string[] path, Action<string[]> addPath)
            {
                this.path = path;
                this.addPath = addPath;
            }

            public IMidIncludable<TRootEntity, TEntity> IncludeAll<TEntity>(Expression<Func<TRootEntity, IEnumerable<TEntity>>> navigationExpression)
            {
                var name = GetName(navigationExpression);
                var path = new[] { name };
                addPath(path);
                return new DetachMidIncludable<TRootEntity, TEntity>(path, addPath);
                //return new DetachCollectionMidIncludable<TRootEntity, TEntity>(path, addPath);
            }

            public IMidIncludable<TRootEntity, TEntity> Include<TEntity>(Expression<Func<TRootEntity, TEntity>> navigationExpression)
            {
                var name = GetName(navigationExpression);
                var path = new[] { name };
                addPath(path);
                return new DetachMidIncludable<TRootEntity, TEntity>(path, addPath);
            }

            public IMidIncludable<TRootEntity, TEntity> ThenIncludeAll<TEntity>(Expression<Func<TMidProperty, IEnumerable<TEntity>>> navigationExpression)
            {
                var name = GetName(navigationExpression);
                var newPath = path.Concat(new[] { name }).ToArray();
                addPath(newPath);
                return new DetachMidIncludable<TRootEntity, TEntity>(newPath, addPath);
                //return new DetachCollectionMidIncludable<TRootEntity, TEntity>(newPath, addPath);
            }

            public IMidIncludable<TRootEntity, TEntity> ThenInclude<TEntity>(Expression<Func<TMidProperty, TEntity>> navigationExpression)
            {
                var name = GetName(navigationExpression);
                var newPath = path.Concat(new[] { name }).ToArray();
                addPath(newPath);
                return new DetachMidIncludable<TRootEntity, TEntity>(path, addPath);
            }
        }
        //public class DetachCollectionMidIncludable<TRootEntity, TMidEntity> : IMidIncludable<TRootEntity, TMidEntity> where TRootEntity : class
        //{
        //    Action<string[]> addPath;
        //    string[] path;
        //    public DetachCollectionMidIncludable(string[] path, Action<string[]> addPath)
        //    {
        //        this.path = path;
        //        this.addPath = addPath;
        //    }

        //    public IMidIncludable<TRootEntity, TEntity> Include<TEntity>(Expression<Func<TRootEntity, IEnumerable<TEntity>>> navigationExpression)
        //    {
        //        var name = GetName(navigationExpression);
        //        var path = new[] { name };
        //        addPath(path);
        //        return new DetachCollectionMidIncludable<TRootEntity, TEntity>(path, addPath);
        //    }

        //    public IMidIncludable<TRootEntity, TEntity> Include<TEntity>(Expression<Func<TRootEntity, TEntity>> navigationExpression)
        //    {
        //        var name = GetName(navigationExpression);
        //        var path = new[] { name };
        //        addPath(path);
        //        return new DetachMidIncludable<TRootEntity, TEntity>(path, addPath);
        //    }

        //    public IMidIncludable<TRootEntity, TEntity> ThenInclude<TEntity>(Expression<Func<TMidEntity, IEnumerable<TEntity>>> navigationExpression)
        //    {
        //        var name = GetName(navigationExpression);
        //        var newPath = path.Concat(new[] { name }).ToArray();
        //        addPath(newPath);
        //        return new DetachCollectionMidIncludable<TRootEntity, TEntity>(newPath, addPath);
        //    }

        //    public IMidIncludable<TRootEntity, TEntity> ThenInclude<TEntity>(Expression<Func<TMidEntity, TEntity>> navigationExpression)
        //    {
        //        var name = GetName(navigationExpression);
        //        var newPath = path.Concat(new[] { name }).ToArray();
        //        addPath(newPath);
        //        return new DetachMidIncludable<TRootEntity, TEntity>(newPath, addPath);
        //    }
        //}
        #endregion

        #region QueryIncludable<TEntity>
        public class EfCoreQueryIncludable<TEntity> : IIncludable<TEntity> where TEntity : class
        {
            public IQueryable<TEntity> ResultQueryable { get; private set; }

            public EfCoreQueryIncludable(IQueryable<TEntity> rootQuery)
            {
                ResultQueryable = rootQuery;
            }

            internal void SetResultQueryable(IQueryable<TEntity> query)
            {
                ResultQueryable = query;
            }

            public IMidIncludable<TEntity, TEntityProperty> IncludeAll<TEntityProperty>(Expression<Func<TEntity, IEnumerable<TEntityProperty>>> navigationExpression)
            {
                var furtherQuery = ResultQueryable.Include(navigationExpression);
                return new QueryCollectionMidIncludable<TEntity, TEntityProperty>(furtherQuery, SetResultQueryable);
            }

            public IMidIncludable<TEntity, TEntityProperty> Include<TEntityProperty>(Expression<Func<TEntity, TEntityProperty>> navigationExpression)
            {
                var furtherQuery = ResultQueryable.Include(navigationExpression);
                return new QueryMidIncludable<TEntity, TEntityProperty>(furtherQuery, SetResultQueryable);
            }

        }
        public class QueryMidIncludable<TRootEntity, TMidEntity> : IMidIncludable<TRootEntity, TMidEntity> where TRootEntity : class
        {
            private IIncludableQueryable<TRootEntity, TMidEntity> query;
            private Action<IQueryable<TRootEntity>> setResult;
            internal QueryMidIncludable(IIncludableQueryable<TRootEntity, TMidEntity> query, Action<IQueryable<TRootEntity>> setResult)
            {
                this.query = query;
                this.setResult = setResult;
                setResult(query);
            }
            public IMidIncludable<TRootEntity, TEntity> IncludeAll<TEntity>(Expression<Func<TRootEntity, IEnumerable<TEntity>>> navigationExpression)
            {
                var furtherQuery = query.Include(navigationExpression);
                return new QueryCollectionMidIncludable<TRootEntity, TEntity>(furtherQuery, setResult);
            }

            public IMidIncludable<TRootEntity, TEntity> Include<TEntity>(Expression<Func<TRootEntity, TEntity>> navigationExpression)
            {
                var furtherQuery = query.Include(navigationExpression);
                return new QueryMidIncludable<TRootEntity, TEntity>(furtherQuery, setResult);
            }

            public IMidIncludable<TRootEntity, TEntity> ThenIncludeAll<TEntity>(Expression<Func<TMidEntity, IEnumerable<TEntity>>> navigationExpression)
            {
                var furtherQuery = query.ThenInclude(navigationExpression);
                return new QueryCollectionMidIncludable<TRootEntity, TEntity>(furtherQuery, setResult);
            }

            public IMidIncludable<TRootEntity, TEntity> ThenInclude<TEntity>(Expression<Func<TMidEntity, TEntity>> navigationExpression)
            {
                var furtherQuery = query.ThenInclude(navigationExpression);
                return new QueryMidIncludable<TRootEntity, TEntity>(furtherQuery, setResult);
            }
        }
        public class QueryCollectionMidIncludable<TRootEntity, TMidEntity> : IMidIncludable<TRootEntity, TMidEntity> where TRootEntity : class
        {
            private IIncludableQueryable<TRootEntity, IEnumerable<TMidEntity>> query;
            private Action<IQueryable<TRootEntity>> setResult;
            internal QueryCollectionMidIncludable(IIncludableQueryable<TRootEntity, IEnumerable<TMidEntity>> query, Action<IQueryable<TRootEntity>> setResult)
            {
                this.query = query;
                this.setResult = setResult;
                setResult(query);
            }
            public IMidIncludable<TRootEntity, TEntity> IncludeAll<TEntity>(Expression<Func<TRootEntity, IEnumerable<TEntity>>> navigationExpression)
            {
                var furtherQuery = query.Include(navigationExpression);
                return new QueryCollectionMidIncludable<TRootEntity, TEntity>(furtherQuery, setResult);
            }

            public IMidIncludable<TRootEntity, TEntity> Include<TEntity>(Expression<Func<TRootEntity, TEntity>> navigationExpression)
            {
                var furtherQuery = query.Include(navigationExpression);
                return new QueryMidIncludable<TRootEntity, TEntity>(furtherQuery, setResult);
            }

            public IMidIncludable<TRootEntity, TEntity> ThenIncludeAll<TEntity>(Expression<Func<TMidEntity, IEnumerable<TEntity>>> navigationExpression)
            {
                var furtherQuery = query.ThenInclude(navigationExpression);
                return new QueryCollectionMidIncludable<TRootEntity, TEntity>(furtherQuery, setResult);
            }

            public IMidIncludable<TRootEntity, TEntity> ThenInclude<TEntity>(Expression<Func<TMidEntity, TEntity>> navigationExpression)
            {
                var furtherQuery = query.ThenInclude(navigationExpression);
                return new QueryMidIncludable<TRootEntity, TEntity>(furtherQuery, setResult);
            }
        }
        #endregion
    }
}
