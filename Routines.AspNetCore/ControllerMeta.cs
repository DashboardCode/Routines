using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using Microsoft.Extensions.Primitives;

namespace DashboardCode.Routines.AspNetCore
{
    
    public class ControllerMeta<TEntity, TKey> where TEntity : class
    {
        public readonly Func<TEntity> Constructor;
        public readonly ReferencesMeta<TEntity> ReferencesManager;
        public readonly Include<TEntity> IndexIncludes;
        public readonly Include<TEntity> DetailsIncludes;
        public readonly Include<TEntity> DeleteIncludes;
        public readonly Include<TEntity> EditIncludes;
        public readonly Func<TKey, Expression<Func<TEntity, bool>>> FindPredicate;
        public readonly Include<TEntity> DisabledProperties;
        public readonly Dictionary<string, Func<TEntity, Func<StringValues, BinderResult>>> editableBinders;
        public readonly Dictionary<string, Func<TEntity, Action<StringValues>>> notEditableBinders;

        public class NotEditables
        {
            readonly Dictionary<string, Func<TEntity, Action<StringValues>>> notEditableBinders;
            public NotEditables(Dictionary<string, Func<TEntity, Action<StringValues>>> notEditableBinders)
            {
                this.notEditableBinders = notEditableBinders;
            }

            public NotEditables Add<TProperty>(Expression<Func<TEntity, TProperty>> getProperty)
            {
                var memberExpression = (MemberExpression)getProperty.Body;

                var propertyType = typeof(TProperty);
                Func<StringValues, TProperty> converter;
                if (propertyType == typeof(int))
                {
                    Func<StringValues, int> f = sv => int.Parse(sv.ToString());
                    converter = (Func<StringValues, TProperty>)(Delegate)f;
                }
                else if (propertyType == typeof(long))
                {
                    Func<StringValues, long> f = sv => long.Parse(sv.ToString());
                    converter = (Func<StringValues, TProperty>)(Delegate)f;
                }
                else if (propertyType == typeof(byte))
                {
                    Func<StringValues, byte> f = sv => byte.Parse(sv.ToString());
                    converter = (Func<StringValues, TProperty>)(Delegate)f;
                }
                else if (propertyType == typeof(string))
                {
                    Func<StringValues, string> f = sv => sv.ToString();
                    converter = (Func<StringValues, TProperty>)(Delegate)f;
                }
                else if (propertyType == typeof(byte[]))
                {
                    Func<StringValues, byte[]> f = sv => Convert.FromBase64String(sv);
                    converter = (Func<StringValues, TProperty>)(Delegate)f;
                }
                else
                {
                    throw new NotSupportedException($"Type '{propertyType.Name}' is not supported by automation on '{nameof(NotEditables)}' controller's meta configuration");
                }
                Add(memberExpression.Member.Name, memberExpression, converter);
                return this;
            }

            public NotEditables Add<TProperty>(Expression<Func<TEntity, TProperty>> getProperty, Func<StringValues, TProperty> converter)
            {
                var memberExpression = (MemberExpression)getProperty.Body;
                Add(memberExpression.Member.Name, memberExpression, converter);
                return this;
            }

            public NotEditables Add<TProperty>(string formName, Expression<Func<TEntity, TProperty>> getPropertyExpression, Func<StringValues, TProperty> converter)
            {
                Add(formName, (MemberExpression)getPropertyExpression.Body, converter);
                return this;
            }

            private NotEditables Add<TProperty>(string formName, MemberExpression memberExpression, Func<StringValues, TProperty> converter)
            {
                Func<TEntity, Action<StringValues>> action = memberExpression.CompileSettterWithConverter<TEntity, StringValues, TProperty>(converter);
                Add(formName, action);
                return this;
            }

            public NotEditables Add<TProperty>(Expression<Func<TEntity, TProperty>> getPropertyExpression, Func<Action<TEntity, TProperty>, Func<TEntity, Action<StringValues>>> action)
            {
                var memberExpression = (MemberExpression)getPropertyExpression.Body;
                return Add(memberExpression.Member.Name, memberExpression, action);
            }

            public NotEditables Add<TProperty>(string formName, Expression<Func<TEntity, TProperty>> getPropertyExpression, Func<Action<TEntity, TProperty>, Func<TEntity, Action<StringValues>>> action)
            {
                return Add(formName, (MemberExpression)getPropertyExpression.Body, action);
            }

            public NotEditables Add<TProperty>(string formName, MemberExpression memberExpression, Func<Action<TEntity, TProperty>, Func<TEntity, Action<StringValues>>> action)
            {
                var setter = memberExpression.CompileSetter<TEntity, TProperty>();
                Func<TEntity, Action<StringValues>> action2 = action(setter);
                Add(formName, action2);
                return this;
            }

            public NotEditables Add(string formName, Func<TEntity, Action<StringValues>> action)
            {
                notEditableBinders.Add(formName, action);
                return this;
            }
        }

        public class Editables
        {
            readonly Dictionary<string, Func<TEntity, Func<StringValues, BinderResult>>> editableBinders;
            public Editables(Dictionary<string, Func<TEntity, Func<StringValues, BinderResult>>> editableBinders)
            {
                this.editableBinders = editableBinders;
            }

            //public NotEditables Add<TProperty>(Expression<Func<TEntity, TProperty>> getProperty)
            //{
            //    var memberExpression = (MemberExpression)getProperty.Body;

            //    var propertyType = typeof(TProperty);
            //    Func<StringValues, TProperty> converter;
            //    if (propertyType == typeof(int))
            //    {
            //        Func<StringValues, int> f = sv => int.Parse(sv.ToString());
            //        converter = (Func<StringValues, TProperty>)(Delegate)f;
            //    }
            //    else if (propertyType == typeof(long))
            //    {
            //        Func<StringValues, long> f = sv => long.Parse(sv.ToString());
            //        converter = (Func<StringValues, TProperty>)(Delegate)f;
            //    }
            //    else if (propertyType == typeof(byte))
            //    {
            //        Func<StringValues, byte> f = sv => byte.Parse(sv.ToString());
            //        converter = (Func<StringValues, TProperty>)(Delegate)f;
            //    }
            //    else if (propertyType == typeof(string))
            //    {
            //        Func<StringValues, string> f = sv => sv.ToString();
            //        converter = (Func<StringValues, TProperty>)(Delegate)f;
            //    }
            //    else if (propertyType == typeof(byte[]))
            //    {
            //        Func<StringValues, byte[]> f = sv => Convert.FromBase64String(sv);
            //        converter = (Func<StringValues, TProperty>)(Delegate)f;
            //    }
            //    else
            //    {
            //        throw new NotSupportedException($"Type '{propertyType.Name}' is not supported by automation on '{nameof(NotEditables)}' controller's meta configuration");
            //    }
            //    Add(memberExpression.Member.Name, memberExpression, converter);
            //    return this;
            //}


            public Editables Add<TProperty>(Expression<Func<TEntity, TProperty>> getPropertyExpression, Func<Action<TEntity, TProperty>, Func<TEntity, Func<StringValues, BinderResult>>> action)
            {
                var memberExpression = (MemberExpression)getPropertyExpression.Body;
                Add(memberExpression.Member.Name, memberExpression, action);
                return this;
            }

            public Editables Add<TProperty>(string formName, Expression<Func<TEntity, TProperty>> getPropertyExpression, Func< Action<TEntity, TProperty> , Func<TEntity, Func<StringValues, BinderResult>>> action)
            {
                var memberExpression = (MemberExpression)getPropertyExpression.Body;
                Add(formName, memberExpression, action);
                return this;
            }

            private Editables Add<TProperty>(string formName, MemberExpression memberExpression, Func<Action<TEntity, TProperty>, Func<TEntity, Func<StringValues, BinderResult>>> action)
            {
                var setter = memberExpression.CompileSetter<TEntity, TProperty>();
                var action2 = action(setter);
                Add(formName, action2);
                return this;
            }

            public Editables Add(string formName, Func<TEntity, Func<StringValues, BinderResult>> action)
            {
                editableBinders.Add(formName, action);
                return this;
            }
        }

        public ControllerMeta(
            Func<TEntity> constructor,
            ReferencesMeta<TEntity> referencesManager,
            Include<TEntity> indexIncludes,
            Include<TEntity> detailsIncludes,
            Include<TEntity> deleteIncludes,
            Include<TEntity> editIncludes,
            Func<TKey, Expression<Func<TEntity, bool>>> findPredicate,
            Include<TEntity> disabledProperties,
            Action<Editables> addEditableBinders,

            Action<NotEditables> addNotEditableBinders//Action<Action<string, Func<TEntity, Action<StringValues>>>> addNotEditableBinders 
            )
        {
            this.Constructor = constructor;
            this.ReferencesManager=referencesManager;
            this.IndexIncludes = indexIncludes;
            this.DetailsIncludes = detailsIncludes;
            this.DeleteIncludes = deleteIncludes;
            this.EditIncludes = editIncludes;
            this.FindPredicate = findPredicate;
            this.DisabledProperties = disabledProperties;

            this.editableBinders = new Dictionary<string, Func<TEntity, Func<StringValues, BinderResult>>>();
            var editables = new Editables(editableBinders);
            addEditableBinders(editables);

            this.notEditableBinders = new Dictionary<string, Func<TEntity, Action<StringValues>>> ();
            var notEditables = new NotEditables(notEditableBinders);
            addNotEditableBinders(notEditables);
            //addNotEditableBinders((s, a) => this.notEditableBinders.Add(s, a));
        }

        public ControllerMeta(
            Func<TEntity> constructor,
            ReferencesMeta<TEntity> referencesManager,
            Include<TEntity> indexIncludes,
            Include<TEntity> editIncludes,
            Func<TKey, Expression<Func<TEntity, bool>>> findPredicate,
            Include<TEntity> disabledProperties,
            Action<Editables> addEditableBinders,
            //Action<Action<string, Func<TEntity, Func<StringValues, BinderResult>>>> addEditableBinders,
            Action<NotEditables> addNotEditableBinders
            //Action<Action<string, Func<TEntity, Action<StringValues>>>> addNotEditableBinders
            ) :this(constructor, referencesManager, indexIncludes, indexIncludes, indexIncludes, editIncludes, findPredicate, disabledProperties, addEditableBinders, addNotEditableBinders)
        {

        }
    }
}