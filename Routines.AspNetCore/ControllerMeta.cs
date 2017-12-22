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

            #region provide converter
            public NotEditables Add<TProperty>(Expression<Func<TEntity, TProperty>> getProperty, Func<StringValues, TProperty> converter,
                Func<Func<TProperty>, Action<Action<TProperty>>> action = null
            )
            {
                var memberExpression = (MemberExpression)getProperty.Body;
                Add(memberExpression.Member.Name, memberExpression, converter, action);
                return this;
            }

            public NotEditables Add<TProperty>(string formField, Expression<Func<TEntity, TProperty>> getProperty, Func<StringValues, TProperty> converter,
                Func<Func<TProperty>, Action<Action<TProperty>>> action = null
            )
            {
                Add(formField, (MemberExpression)getProperty.Body, converter, action);
                return this;
            }

            private NotEditables Add<TProperty>(string formField, MemberExpression memberExpression, Func<StringValues, TProperty> converter,
                Func<Func<TProperty>, Action<Action<TProperty>>> action = null
            )
            {
                if (action == null)
                    action = convert => set => set(convert());

                var setter = memberExpression.CompileSetProperty<TEntity, TProperty>();
                Func<TEntity, Action<StringValues>> straightAction = e => sv => action(() => converter(sv))(v => setter(e,v));
                Add(formField, straightAction);
                return this;
            }
            #endregion

            #region provide setter
            public NotEditables Add<TProperty>(Expression<Func<TEntity, TProperty>> getPropertyExpression, Func<Action<TProperty>, Action<StringValues>> action)
            {
                var memberExpression = (MemberExpression)getPropertyExpression.Body;
                return Add(memberExpression.Member.Name, memberExpression, action);
            }

            public NotEditables Add<TProperty>(string formName, Expression<Func<TEntity, TProperty>> getPropertyExpression, Func<Action<TProperty>, Action<StringValues>> action)
            {
                return Add(formName, (MemberExpression)getPropertyExpression.Body, action);
            }

            private NotEditables Add<TProperty>(string formName, MemberExpression memberExpression, Func<Action<TProperty>, Action<StringValues>> action)
            {
                var setter = memberExpression.CompileFunctionalSetter<TEntity, TProperty>();
                Func<TEntity, Action<StringValues>> straightAction = e=>action(setter(e));
                Add(formName, straightAction);
                return this;
            }
            #endregion

            public NotEditables Add(string formName, Func<TEntity, Action<StringValues>> straightAction)
            {
                notEditableBinders.Add(formName, straightAction);
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


            #region provide with validate

            public class AssertsCollection<TProperty> 
            {
                private readonly List<ValueTuple<Func<TProperty, bool>, string>> list = new List<ValueTuple<Func<TProperty, bool>, string>>();
                public IReadOnlyCollection<ValueTuple<Func<TProperty, bool>, string>> List { get { return list; } }

                public AssertsCollection<TProperty> Add(Func<TProperty, bool> f, string errorMessage)
                {
                    list.Add((f, errorMessage));
                    return this;
                }

                public static Func<TProperty, BinderResult> ToValidate(Action<AssertsCollection<TProperty>> addAsserts)
                {
                    Func<TProperty, BinderResult> validate = v =>
                    {
                        var errorMessages = new List<string>();
                        var assertsCollection = new AssertsCollection<TProperty>();
                        addAsserts(assertsCollection);
                        foreach (var (f, s) in assertsCollection.List)
                            if (!f(v)) errorMessages.Add(s);
                        return new BinderResult(errorMessages);
                    };
                    return validate;
                }
            }


            public Editables Add<TProperty>(
                string formField,
                Expression<Func<TEntity, TProperty>> getPropertyExpression,
                Func<StringValues, ConvertResult<TProperty>> convert,
                Action<AssertsCollection<TProperty>> addAsserts,
                Func<Func<ConvertResult<TProperty>>, Func<Func<ConvertResult<TProperty>, ConvertResult<TProperty>>, Func<Func<ConvertResult<TProperty>, BinderResult>, BinderResult>>> action = null)
            {
                var validate = AssertsCollection<TProperty>.ToValidate(addAsserts);
                Add(formField, getPropertyExpression, convert, validate, action);
                return this;
            }

            public Editables Add<TProperty>(
                Expression<Func<TEntity, TProperty>> getPropertyExpression,
                Func<StringValues, ConvertResult<TProperty>> convert,
                Action<AssertsCollection<TProperty>> addAsserts,
                Func<Func<ConvertResult<TProperty>>, Func<Func<ConvertResult<TProperty>, ConvertResult<TProperty>>, Func<Func<ConvertResult<TProperty>, BinderResult>, BinderResult>>> action = null)
            {
                var validate = AssertsCollection<TProperty>.ToValidate(addAsserts);
                Add(getPropertyExpression, convert, validate, action);
                return this;
            }

            public Editables Add<TProperty>(
                Expression<Func<TEntity, TProperty>> getPropertyExpression,
                Func<StringValues, ConvertResult<TProperty>> convert,
                Func<TProperty, BinderResult> validate = null,
                Func<Func<ConvertResult<TProperty>>, Func<Func<ConvertResult<TProperty>, ConvertResult<TProperty>>, Func<Func<ConvertResult<TProperty>, BinderResult>, BinderResult>>> action = null)
            {
                var memberExpression = (MemberExpression)getPropertyExpression.Body;
                Add(memberExpression.Member.Name, (MemberExpression)getPropertyExpression.Body, convert, validate, action);
                return this;
            }

            public Editables Add<TProperty>(
                string formField,
                Expression<Func<TEntity, TProperty>> getPropertyExpression,
                Func<StringValues, ConvertResult<TProperty>> convert,
                Func<TProperty, BinderResult> validate=null,
                Func<Func<ConvertResult<TProperty>>, Func<Func<ConvertResult<TProperty>, ConvertResult<TProperty>>, Func<Func<ConvertResult<TProperty>, BinderResult>, BinderResult>>> action = null)
            {
                Add(formField, (MemberExpression)getPropertyExpression.Body, convert, validate, action);
                return this;
            }

            private Editables Add<TProperty>(
                string formField,
                MemberExpression memberExpression,
                Func<StringValues, ConvertResult<TProperty>> convert,
                Func<TProperty, BinderResult> validate=null,
                Func<Func<ConvertResult<TProperty>>, Func<Func<ConvertResult<TProperty>, ConvertResult<TProperty>>, Func<Func<ConvertResult<TProperty>, BinderResult>, BinderResult>>> action = null)
            {
                if (action == null)
                    action = converter => setter => validator => validator(setter(converter()));
                var set = memberExpression.CompileSetProperty<TEntity, TProperty>();
                Func<TEntity, Func<StringValues, BinderResult>> straightAction;
                if (validate==null)
                    straightAction = e => sv => action(() => convert(sv))(cr => { if (cr.IsSuccess()) set(e, cr.Value); return cr; })(cr => cr.BinderResult);
                else
                    straightAction = e => sv => action(() => convert(sv))(cr => { if (cr.IsSuccess()) set(e, cr.Value); return cr; })
                        (cr => { if (cr.IsSuccess()) return validate(cr.Value); return cr.BinderResult; });
                editableBinders.Add(formField, straightAction);
                return this;
            }
            #endregion

            #region provide with setters & convert
            public Editables Add<TProperty>(
                Expression<Func<TEntity, TProperty>> getPropertyExpression,
                Func<StringValues, ConvertResult<TProperty>> convert,
                Func<Func<ConvertResult<TProperty>>, Func<Func<ConvertResult<TProperty>, ConvertResult<TProperty>>, BinderResult>> action)
            {
                var memberExpression = (MemberExpression)getPropertyExpression.Body;
                Add(memberExpression.Member.Name, memberExpression, convert, action);
                return this;
            }

            public Editables Add<TProperty>(
                string formField,
                Expression<Func<TEntity, TProperty>> getPropertyExpression,
                Func<StringValues, ConvertResult<TProperty>> convert,
                Func<Func<ConvertResult<TProperty>>, Func<Func<ConvertResult<TProperty>, ConvertResult<TProperty>>, BinderResult>> action)
            {
                Add(formField, (MemberExpression)getPropertyExpression.Body, convert, action);
                return this;
            }
            public Editables Add<TProperty>(
                string formField,
                MemberExpression memberExpression,
                Func<StringValues, ConvertResult<TProperty>> convert,
                Func<Func<ConvertResult<TProperty>>, Func<Func<ConvertResult<TProperty>, ConvertResult<TProperty>>, BinderResult>> action)
            {
                var setter = memberExpression.CompileSetProperty<TEntity, TProperty>();
                Func<TEntity, Func<StringValues, BinderResult>> straightAction = e => sv => action(() => convert(sv))(cr => { if (cr.IsSuccess()) setter(e, cr.Value); return cr; });
                editableBinders.Add(formField, straightAction);
                return this;
            }
            #endregion

            #region provide with setters
            public Editables Add<TProperty>(Expression<Func<TEntity, TProperty>> getPropertyExpression, Func<Action<TProperty>, Func<StringValues, BinderResult>> action)
            {
                var memberExpression = (MemberExpression)getPropertyExpression.Body;
                AddX(memberExpression.Member.Name, memberExpression, action);
                return this;
            }

            public Editables AddX<TProperty>(string formField, Expression<Func<TEntity, TProperty>> getPropertyExpression, Func<Action<TProperty>, Func<StringValues, BinderResult>> action)
            {
                var memberExpression = (MemberExpression)getPropertyExpression.Body;
                AddX(formField, memberExpression, action);
                return this;
            }

            private Editables AddX<TProperty>(string formField, MemberExpression memberExpression, Func<Action<TProperty>, Func<StringValues, BinderResult>> action)
            {
                var setter = memberExpression.CompileFunctionalSetter<TEntity, TProperty>();
                Func<TEntity, Func<StringValues, BinderResult>> straightAction = e => action(setter(e));
                Add(formField, straightAction);
                return this;
            }
            #endregion

            public Editables Add(string formName, Func<TEntity, Func<StringValues, BinderResult>> straightAction)
            {
                editableBinders.Add(formName, straightAction);
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

    public struct ConvertResult<T>
    {
        public T Value;
        public ConvertResult(string message)
        {
            Value = default(T);
            BinderResult = new BinderResult(message);
        }
        public BinderResult BinderResult;
        public bool IsSuccess() => BinderResult.IsSuccess();
    }

    public struct BinderResult
    {
        public BinderResult(List<string> errorMessages)
        {
            if (errorMessages != null && errorMessages.Count > 0)
            {
                ErrorMessages = errorMessages;
            }
            else
            {
                ErrorMessages = null;
            }
        }

        public BinderResult(string message = null)
        {
            if (message != null)
            {
                ErrorMessages = new List<string>() { message };
            }
            else
            {
                ErrorMessages = null;
            }
        }
        public List<string> ErrorMessages;
        public bool IsSuccess() { return ErrorMessages == null; }
    }

    public class Binder
    {
        public static Func<StringValues, ConvertResult<string>> ConvertToString
        {
            get
            {
                return (stringValues) =>
                {
                    return new ConvertResult<string> { Value = stringValues.ToString() };
                };
            }
        }

        public static Func<StringValues, ConvertResult<int>> ConvertToInt
        {
            get
            {
                return (stringValues) =>
                {
                    var str = stringValues.ToString();
                    if (int.TryParse(str, out int number))
                        return new ConvertResult<int> { Value = number };
                    return new ConvertResult<int>("Not number!");
                };
            }
        }

        public static BinderResult TryStringValidateLength(StringValues stringValues, Action<string> setter, int length)
        {
            var v = stringValues.ToString();
            setter(v);
            return new BinderResult(v.Length > length ? "Too long!" : null);
        }
    }
}