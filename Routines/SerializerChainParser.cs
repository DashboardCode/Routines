using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;

namespace Vse.Routines
{
    public class SerializerChainParser<TRootEntity> : IChainingState<TRootEntity>
    {
        static SerializerChainParser(){
            var isEnumerable = typeof(IEnumerable).GetTypeInfo().IsAssignableFrom(typeof(TRootEntity).GetTypeInfo());
            if (typeof(TRootEntity) != typeof(string) && typeof(TRootEntity) != typeof(byte[]) && isEnumerable)
                throw new NotSupportedException("Navigation expression root type can't be defined as collection (except two types byte[] and string)");
        }

        public readonly SerializerNode Root = new SerializerNode(typeof(TRootEntity), null);

        private SerializerPropertyNode CurrentNode;

        private static SerializerPropertyNode AddIfAbsent(
            SerializerNode parent,
            Expression expression,
            PropertyInfo propertyInfo,
            Type navigationType,
            Type navigationEnumerableType,
            bool isEnumerable,
            Func<Func<object, object>> getGeneralized)
        {
            var dictionary = parent.Children;
            var name = propertyInfo.Name;
            if (!dictionary.TryGetValue(name, out SerializerPropertyNode node))
            {
                var generalized = getGeneralized();
                if (isEnumerable)
                {
                    node = new SerializerEnumerablePropertyNode(navigationType, expression, parent,  name, navigationEnumerableType);
                }
                else
                {
                    node = new SerializerPropertyNode(navigationType, expression, parent,  name);
                }
                dictionary.Add(name, node);
            }
            return node;
        }

        public void ParseHead<TEntity>(Expression<Func<TRootEntity, TEntity>> navigationExpression)
        {
            var propertyInfo = ((MemberExpression)navigationExpression.Body).Member as PropertyInfo;
            var node = AddIfAbsent(Root, navigationExpression, propertyInfo,  typeof(TEntity), null, false, () =>
            {
                //TODO: it is interesing how to convert Func<TRootEntity, TEntity> to Func<object, object> before compile
                //Also it is interesting if my prognose about performance true: current straight-forward casting is the best.

                //var param = Expression.Parameter(typeof(object));
                //var casted = Expression.Convert(navigationExpression, typeof(object));
                //Expression<Func<object, object>> getterExpression =
                //    Expression.Lambda<Func<object, object>>(
                //        navigationExpression.Body
                //         casted
                //         , Expression.Parameter(typeof(object)));
                //Func<object, object> generalized = getterExpression.Compile();

                Func<TRootEntity, TEntity> func = navigationExpression.Compile();
                Func<object, object> generalized = (o) => { return func((TRootEntity)o); };
                return generalized;
            });
            CurrentNode = node;
        }
        public void ParseHeadEnumerable<TEntity>(Expression<Func<TRootEntity, IEnumerable<TEntity>>> navigationExpression)
        {
            var propertyInfo = ((MemberExpression)navigationExpression.Body).Member as PropertyInfo;
            var node = AddIfAbsent(Root, navigationExpression, propertyInfo, typeof(TEntity), typeof(IEnumerable<TEntity>), true, () =>
            {
                Func<TRootEntity, IEnumerable<TEntity>> func = navigationExpression.Compile();
                Func<object, object> generalized = (o) => { return func((TRootEntity)o); };
                return generalized;
            });
            CurrentNode = node;
        }
        public void Parse<TThenEntity, TEntity>(Expression<Func<TThenEntity, TEntity>> navigationExpression)
        {
            var propertyInfo = ((MemberExpression)navigationExpression.Body).Member as PropertyInfo;
            var node = AddIfAbsent(CurrentNode, navigationExpression, propertyInfo, typeof(TEntity), null, false, () =>
            {
                Func<TThenEntity, TEntity> func = navigationExpression.Compile();
                Func<object, object> generalized = (o) => { return func((TThenEntity)o); };
                return generalized;
            });
            CurrentNode = node;
        }
        public void ParseEnumerable<TThenEntity, TEntity>(Expression<Func<TThenEntity, IEnumerable<TEntity>>> navigationExpression)
        {
            var propertyInfo = ((MemberExpression)navigationExpression.Body).Member as PropertyInfo;
            var node = AddIfAbsent(CurrentNode, navigationExpression, propertyInfo, typeof(TEntity), typeof(IEnumerable<TEntity>), true, () =>
            {
                Func<TThenEntity, IEnumerable<TEntity>> func = navigationExpression.Compile();
                Func<object, object> generalized = (o) => { return func((TThenEntity)o); };
                return generalized;
            });
            CurrentNode = node;
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
