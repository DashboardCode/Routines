using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace Vse.Routines
{
    public static class MemberExpressionExtensions
    {
        public static string GetMemberName<T1, T2>(Expression<Func<T1, T2>> expression)
        {
            var memberExpression = (MemberExpression)expression.Body;
            return memberExpression.Member.Name;
        }
        public static object GetMemberValue(this MemberExpression memberExpression, object entity)
        {
            var popertyName = memberExpression.Member.Name;
            var type = entity.GetType();
            var typeInfo = type.GetTypeInfo();
            var propertyInfo = typeInfo.GetDeclaredProperty(popertyName);
            Debug.Assert(propertyInfo!=null, "propertyInfo is null");
            Debug.Assert(propertyInfo.CanRead && propertyInfo.GetIndexParameters().Length == 0, "propertyInfo can't be read");
            var value = propertyInfo.GetValue(entity);
            return value;
        }

        public static object GetMemberValueCompiled(this MemberExpression memberExpression, object entity)
        {
            UnaryExpression objectMember = Expression.Convert(memberExpression, typeof(object));
            //memberExpression,.
            Expression<Func<object, object>> getterExpression = Expression.Lambda<Func<object,object>>(objectMember);
            var getter = getterExpression.Compile();
            return getter(entity);
        }

        private static Tuple<object, object> GetMemberValues(
            this MemberExpression memberExpression,
            object entity1,
            object entity2)
        {
            var popertyName = memberExpression.Member.Name;
            var type = entity1.GetType();
            var typeInfo = type.GetTypeInfo();
            var propertyInfo = typeInfo.GetDeclaredProperty(popertyName);
            Debug.Assert(propertyInfo != null, "propertyInfo is null");
            Debug.Assert(propertyInfo.CanRead && propertyInfo.GetIndexParameters().Length == 0, "propertyInfo can't be read");
            var value1 = propertyInfo.GetValue(entity1);
            var value2 = propertyInfo.GetValue(entity2);
            return new Tuple<object, object>(value1, value2);
        }

        private static Type GetMemberType(this MemberExpression memberExpression)
        {
            var type = memberExpression.Type;
            return type;
        }

        private static void SetValue(this MemberExpression memberExpression, object entity, object propertyValue)
        {
            var type = entity.GetType();
            var popertyName = memberExpression.Member.Name;
            var propertyInfo = type.GetTypeInfo().GetDeclaredProperty(popertyName);
            Debug.Assert(propertyInfo.CanWrite && propertyInfo.GetIndexParameters().Length == 0);
            propertyInfo.SetValue(entity, propertyValue);
        }
        
        private static Tuple<object, object> CopyMemberValue(
            this MemberExpression memberExpression,
            object source,
            object destination)
        {
            object copiedValue = default(object);
            var sourceValue = memberExpression.GetMemberValue(source);
            if (sourceValue != null)
            {
                var type = sourceValue.GetType();
                var typeInfo = type.GetTypeInfo();
                if (!typeInfo.IsClass || sourceValue is string)
                {
                    copiedValue = sourceValue;
                    SetValue(memberExpression, destination, copiedValue);
                }
                else
                {
                    var destinationValue = memberExpression.GetMemberValue(destination);
                    if (destinationValue == null)
                    {
                        if (type.IsArray)
                        {
                            var sourceArray = (Array)sourceValue;
                            copiedValue = Activator.CreateInstance(type, new object[] { sourceArray.Length });
                        }
                        else
                        {
                            var constructor = typeInfo.DeclaredConstructors.First(e=>e.GetParameters().Count()==0);
                            copiedValue = constructor.Invoke(null);
                        }
                    }
                    else
                    {
                        copiedValue = destinationValue;
                    }
                    SetValue(memberExpression, destination, copiedValue);
                }
            }
            return new Tuple<object, object>(sourceValue, copiedValue);
        }

        internal static void FlattenMemberExpressionNode(this IEnumerable<MemberExpressionNode> nodes, List<Type> types)
        {
            foreach (var node in nodes)
            {
                var type = node.MemberExpression.GetMemberType();
                if (!types.Any(t => t.AssemblyQualifiedName == type.AssemblyQualifiedName))
                    types.Add(type);
                FlattenMemberExpressionNode(node.Children, types);
            }
        }

        private static object CloneItem(object sourceItem, List<MemberExpressionNode> nodes, IReadOnlyCollection<Type> systemTypes)
        {
            if (sourceItem == null)
            {
                return null;
            }
            else
            {
                var type = sourceItem.GetType();
                var typeInfo = type.GetTypeInfo();
                if (sourceItem is string)
                {
                    return new String(((string)sourceItem).ToCharArray()); // String.Copy((string)sourceItem); absent in standard
                }
                else if (typeInfo.IsValueType)
                {
                    if (nodes.Count > 0)
                        throw new InvalidOperationException($"It is impossible to clone value type's '{type.FullName}' instance specifing includes '{string.Join(", ", nodes.Select(e=>e.MemberName))}' . Value type's instance can be cloned only entirely!");
                    return sourceItem;
                }
                else if (typeInfo.IsClass)
                {
                    var constructor = typeInfo.DeclaredConstructors.First(e => e.GetParameters().Count() == 0);
                    var destinationItem = constructor.Invoke(null);
                    CopyNodes(sourceItem, destinationItem, nodes, systemTypes);
                    return destinationItem;
                }
                else
                {
                    throw new InvalidOperationException($"Clonning of type '{type.FullName}' is not supported");
                }
            }
        }
        
        public static List<PropertyInfo> GetPrimitiveOrSimpleProperties(
            Type type, IReadOnlyCollection<Type> simpleTypes)
        {
            var properties = type.GetTypeInfo().DeclaredProperties;
            var list = new List<PropertyInfo>();
            foreach (var propertyInfo in properties)
                if (propertyInfo.CanRead && propertyInfo.CanWrite && propertyInfo.GetIndexParameters().Length == 0)
                {
                    var propertyType = propertyInfo.PropertyType;
                    var typeInfo = propertyType.GetTypeInfo();
                    if (typeInfo.IsPrimitive 
                        || propertyType==typeof(string) 
                        || simpleTypes.Contains(propertyInfo.PropertyType))
                    {
                        list.Add(propertyInfo);
                    }
                    else
                    {
                        var baseNullableType = Nullable.GetUnderlyingType(propertyType);
                        if (baseNullableType != null && baseNullableType.GetTypeInfo().IsPrimitive)
                        {
                            list.Add(propertyInfo);
                        }
                    }
                }
            return list;
        }

        public static List<PropertyInfo> GetSimpleProperties(
            Type type, IReadOnlyCollection<Type> systemTypes)
        {
            var properties = type.GetTypeInfo().DeclaredProperties;
            var list = new List<PropertyInfo>();
            foreach (var propertyInfo in properties)
                if (propertyInfo.CanRead && propertyInfo.CanWrite && propertyInfo.GetIndexParameters().Length == 0)
                    if (systemTypes.Contains(propertyInfo.PropertyType))
                        list.Add(propertyInfo);
            return list;
        }

        public static void CopySimpleTypesProperties(
            object source,
            object destination,
            IReadOnlyCollection<Type> systemTypes = null)
        {
            var properties = GetSimpleProperties(destination.GetType(), systemTypes);
            foreach (var propertyInfo in properties)
            {
                 var value = propertyInfo.GetValue(source, null);
                 propertyInfo.SetValue(destination, value);
            }
        }

        internal static void CopyNodes(
            object source,
            object destination,
            List<MemberExpressionNode> nodes,
            IReadOnlyCollection<Type> systemTypes)
        {
            if (source is Array)
            {
                CopyArray((Array)source, (Array)destination,  (sourceItem)=> CloneItem(sourceItem, nodes, systemTypes));
            }
            else if (source is IList)
            {
                CopyList((IEnumerable)source, ((IList)destination), (sourceItem) => CloneItem(sourceItem, nodes, systemTypes));
            }
            else if (source is IEnumerable && source.GetType().GetTypeInfo().ImplementedInterfaces.Any(t =>
                    t.GetTypeInfo().IsGenericType &&
                    t.GetGenericTypeDefinition() == typeof(ISet<>)))
            {
                CopySet((IEnumerable)source, (object)destination, (sourceItem) => CloneItem(sourceItem, nodes, systemTypes));
            }
            else
            {

                if (systemTypes != null)
                    CopySimpleTypesProperties(source, destination, systemTypes);
                foreach (var node in nodes)
                {
                    var value = node.MemberExpression.CopyMemberValue(source, destination);
                    var s = value.Item1;
                    var d = value.Item2;
                    if (s != null)
                        CopyNodes(s, d, node.Children, systemTypes); // recursion
                }
            }
        }

        internal static bool EqualsItem(object entity1Item, object entity2Item, List<MemberExpressionNode> nodes)
        {
            if (entity1Item == null && entity2Item == null)
                return true;
            else if ((entity1Item != null && entity2Item == null) || (entity1Item == null && entity2Item != null))
                return false;
            if (nodes.Count() > 0)
                return EqualsNodes(entity1Item, entity2Item, nodes);
            else if (entity1Item is IEnumerable && !(entity1Item is string))
                return EqualsNodes(entity1Item, entity2Item, nodes);
            else
                return entity1Item.Equals(entity2Item);
        }

        internal static bool EqualsNodes(
            object entity1,
            object entity2,
            List<MemberExpressionNode> nodes)
        {
            bool @value = true;

            if (entity1 is Array && entity2 is Array)
            {
                @value = EqualsArray((Array)entity1, (Array)entity2, (e1, e2) => EqualsItem(e1, e2, nodes));
            }
            else if (entity1 is IList && entity2 is IList)
            {
                @value = EqualsList((IList)entity1, (IList)entity2, (e1, e2) => EqualsItem(e1, e2, nodes));
            }
            else if (entity1 is IEnumerable && entity1.GetType().GetTypeInfo().ImplementedInterfaces.Any(t =>
                    t.GetTypeInfo().IsGenericType &&
                    t.GetGenericTypeDefinition() == typeof(ISet<>)))
            {
                @value = EqualsSet(entity1, entity2, (e1, e2) => EqualsItem(e1, e2, nodes));
            }
            else
            {
                foreach (var node in nodes)
                {
                    var copied = node.MemberExpression.GetMemberValues(entity1, entity2);
                    @value = EqualsItem(copied.Item1, copied.Item2, node.Children);
                    if (@value == false)
                        break;
                    //if (copied.Item1 != null)
                    //    @value = EqualsNodes(copied.Item1, copied.Item2, node.Children); // recursive
                    //if (@value == false)
                    //    break;
                }
            }
            return @value;
        }


        #region Collection's itearations

        private static void CopyList(
            IEnumerable sourceEnumerable,
            IList destinationList,
            Func<object, object> copyItem)
        {
            destinationList.Clear();
            foreach (var sourceItem in sourceEnumerable)
            {
                var destinationItem = copyItem(sourceItem);
                destinationList.Add(destinationItem);
            }
        }

        private static bool EqualsList(
            IList entity1List,
            IList entity2List,
            Func<object, object, bool> equals)
        {
            var @value = true;
            if (entity1List.Count != entity2List.Count)
            {
                @value = false;
            }
            else
            {
                for (int i = 0; i < entity2List.Count; i++)
                {
                    var entity2Item = entity2List[i];
                    var entity1Item = entity1List[i];
                    @value = equals(entity1Item, entity2Item);
                    if (@value == false)
                        break;
                }
            }
            return @value;
        }

        private static void CopySet(
            IEnumerable sourceEnumerable,
            object set, //object set,
            Func<object, object> copyItem)
        {

            var getTypeInfo = set.GetType().GetTypeInfo();
            var clear = getTypeInfo.GetDeclaredMethod("Clear");
            var add = getTypeInfo.GetDeclaredMethod("Add");
            clear.Invoke(set, null);
            //set.Clear();
            foreach (var sourceItem in sourceEnumerable)
            {
                var destinationItem = copyItem(sourceItem);
                add.Invoke(set, new[] { destinationItem });
                //  set.Add(destinationItem);
            }
        }

        private static bool EqualsSet(
            object set1,
            object set2, //object set,
            Func<object, object, bool> equals)
        {
            var setType = set1.GetType();
            var setTypeInfo = setType.GetTypeInfo();
            var countProperty = setTypeInfo.GetDeclaredProperty("Count");
            var count1 = (int)countProperty.GetValue(set1, null);
            var count2 = (int)countProperty.GetValue(set2, null);

            var genType = setType.GenericTypeArguments.First();
            var array1 = Array.CreateInstance(genType, count1);
            var array2 = Array.CreateInstance(genType, count2);
            var methods = setTypeInfo.GetDeclaredMethods("CopyTo");
            var сopyTo = methods
                .First(e => e.GetParameters().Count() == 1);
            сopyTo.Invoke(set1, new[] { array1 });
            сopyTo.Invoke(set2, new[] { array2 });
            var @value = EqualsArray(array1, array2, equals);
            return @value;
        }

        private static void CopyArray(
            Array sourceArray,
            Array destinationArray,
            Func<object, object> copyItem)
        {
            if (destinationArray.Length != sourceArray.Length)
                throw new InvalidOperationException($"Destination array type of '{destinationArray.GetType().FullName}' is not null and its length differs from source array's length");
            for (int i = 0; i < sourceArray.Length; i++)
            {
                var sourceItem = sourceArray.GetValue(i);
                var destinationItem = copyItem(sourceItem);
                destinationArray.SetValue(destinationItem, i);
            }
        }

        private static bool EqualsArray(
            Array entity1Array,
            Array entity2Array,
            Func<object, object, bool> equals
            )
        {
            var @value = true;
            if (entity1Array.Length != entity2Array.Length)
            {
                @value = false;
            }
            else
            {
                for (int i = 0; i < entity2Array.Length; i++)
                {
                    var entity2Item = entity2Array.GetValue(i);
                    var entity1Item = entity1Array.GetValue(i);
                    @value = equals(entity1Item, entity2Item);
                    if (@value == false)
                        break;
                }
            }
            return @value;
        }
        #endregion
    }

    //public static class IncludeExtensions
    //{

    //}
}

