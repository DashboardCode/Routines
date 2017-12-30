using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using DashboardCode.Routines.Storage;
using Microsoft.Extensions.Primitives;

namespace DashboardCode.Routines.AspNetCore
{
    
    public class ControllerMeta<TEntity, TKey> where TEntity : class, new()
    {
        public readonly Func<TEntity> Constructor;
        
        public readonly Func<string, ConvertFuncResult<TKey>> KeyConverter;
        public readonly Include<TEntity> IndexIncludes;
        public readonly Include<TEntity> DetailsIncludes;
        public readonly Include<TEntity> DeleteIncludes;
        public readonly Include<TEntity> EditIncludes;
        public readonly Func<TKey, Expression<Func<TEntity, bool>>> FindPredicate;
        public readonly Include<TEntity> DisabledProperties;
        public readonly Dictionary<string, Func<TEntity, Func<StringValues, VerboseResult>>> editableBinders;
        public readonly Dictionary<string, Func<TEntity, Action<StringValues>>> notEditableBinders;
        public readonly ReferencesCollection<TEntity> ReferencesMeta;

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
            readonly Dictionary<string, Func<TEntity, Func<StringValues, VerboseResult>>> editableBinders;
            public Editables(Dictionary<string, Func<TEntity, Func<StringValues, VerboseResult>>> editableBinders)
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

                public static Func<TProperty, VerboseResult> ToValidate(Action<AssertsCollection<TProperty>> addAsserts)
                {
                    Func<TProperty, VerboseResult> validate = v =>
                    {
                        var errorMessages = new List<string>();
                        var assertsCollection = new AssertsCollection<TProperty>();
                        addAsserts(assertsCollection);
                        foreach (var (f, s) in assertsCollection.List)
                            if (!f(v)) errorMessages.Add(s);
                        return new VerboseResult(errorMessages);
                    };
                    return validate;
                }
            }


            public Editables Add<TProperty>(
                string formField,
                Expression<Func<TEntity, TProperty>> getPropertyExpression,
                Func<StringValues, ConvertVerboseResult<TProperty>> convert,
                Action<AssertsCollection<TProperty>> addAsserts,
                Func<Func<ConvertVerboseResult<TProperty>>, Func<Func<ConvertVerboseResult<TProperty>, ConvertVerboseResult<TProperty>>, Func<Func<ConvertVerboseResult<TProperty>, VerboseResult>, VerboseResult>>> action = null)
            {
                var validate = AssertsCollection<TProperty>.ToValidate(addAsserts);
                Add(formField, getPropertyExpression, convert, validate, action);
                return this;
            }

            public Editables Add<TProperty>(
                Expression<Func<TEntity, TProperty>> getPropertyExpression,
                Func<StringValues, ConvertVerboseResult<TProperty>> convert,
                Action<AssertsCollection<TProperty>> addAsserts,
                Func<Func<ConvertVerboseResult<TProperty>>, Func<Func<ConvertVerboseResult<TProperty>, ConvertVerboseResult<TProperty>>, Func<Func<ConvertVerboseResult<TProperty>, VerboseResult>, VerboseResult>>> action = null)
            {
                var validate = AssertsCollection<TProperty>.ToValidate(addAsserts);
                Add(getPropertyExpression, convert, validate, action);
                return this;
            }

            public Editables Add<TProperty>(
                Expression<Func<TEntity, TProperty>> getPropertyExpression,
                Func<StringValues, ConvertVerboseResult<TProperty>> convert,
                Func<TProperty, VerboseResult> validate = null,
                Func<Func<ConvertVerboseResult<TProperty>>, Func<Func<ConvertVerboseResult<TProperty>, ConvertVerboseResult<TProperty>>, Func<Func<ConvertVerboseResult<TProperty>, VerboseResult>, VerboseResult>>> action = null)
            {
                var memberExpression = (MemberExpression)getPropertyExpression.Body;
                Add(memberExpression.Member.Name, (MemberExpression)getPropertyExpression.Body, convert, validate, action);
                return this;
            }

            public Editables Add<TProperty>(
                string formField,
                Expression<Func<TEntity, TProperty>> getPropertyExpression,
                Func<StringValues, ConvertVerboseResult<TProperty>> convert,
                Func<TProperty, VerboseResult> validate=null,
                Func<Func<ConvertVerboseResult<TProperty>>, Func<Func<ConvertVerboseResult<TProperty>, ConvertVerboseResult<TProperty>>, Func<Func<ConvertVerboseResult<TProperty>, VerboseResult>, VerboseResult>>> action = null)
            {
                Add(formField, (MemberExpression)getPropertyExpression.Body, convert, validate, action);
                return this;
            }

            private Editables Add<TProperty>(
                string formField,
                MemberExpression memberExpression,
                Func<StringValues, ConvertVerboseResult<TProperty>> convert,
                Func<TProperty, VerboseResult> validate=null,
                Func<Func<ConvertVerboseResult<TProperty>>, Func<Func<ConvertVerboseResult<TProperty>, ConvertVerboseResult<TProperty>>, Func<Func<ConvertVerboseResult<TProperty>, VerboseResult>, VerboseResult>>> action = null)
            {
                if (action == null)
                    action = converter => setter => validator => validator(setter(converter()));
                var set = memberExpression.CompileSetProperty<TEntity, TProperty>();
                Func<TEntity, Func<StringValues, VerboseResult>> straightAction;
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
                Func<StringValues, ConvertVerboseResult<TProperty>> convert,
                Func<Func<ConvertVerboseResult<TProperty>>, Func<Func<ConvertVerboseResult<TProperty>, ConvertVerboseResult<TProperty>>, VerboseResult>> action)
            {
                var memberExpression = (MemberExpression)getPropertyExpression.Body;
                Add(memberExpression.Member.Name, memberExpression, convert, action);
                return this;
            }

            public Editables Add<TProperty>(
                string formField,
                Expression<Func<TEntity, TProperty>> getPropertyExpression,
                Func<StringValues, ConvertVerboseResult<TProperty>> convert,
                Func<Func<ConvertVerboseResult<TProperty>>, Func<Func<ConvertVerboseResult<TProperty>, ConvertVerboseResult<TProperty>>, VerboseResult>> action)
            {
                Add(formField, (MemberExpression)getPropertyExpression.Body, convert, action);
                return this;
            }
            public Editables Add<TProperty>(
                string formField,
                MemberExpression memberExpression,
                Func<StringValues, ConvertVerboseResult<TProperty>> convert,
                Func<Func<ConvertVerboseResult<TProperty>>, Func<Func<ConvertVerboseResult<TProperty>, ConvertVerboseResult<TProperty>>, VerboseResult>> action)
            {
                var setter = memberExpression.CompileSetProperty<TEntity, TProperty>();
                Func<TEntity, Func<StringValues, VerboseResult>> straightAction = e => sv => action(() => convert(sv))(cr => { if (cr.IsSuccess()) setter(e, cr.Value); return cr; });
                editableBinders.Add(formField, straightAction);
                return this;
            }
            #endregion

            #region provide with setters
            public Editables Add<TProperty>(Expression<Func<TEntity, TProperty>> getPropertyExpression, Func<Action<TProperty>, Func<StringValues, VerboseResult>> action)
            {
                var memberExpression = (MemberExpression)getPropertyExpression.Body;
                Add(memberExpression.Member.Name, memberExpression, action);
                return this;
            }

            public Editables Add<TProperty>(string formField, Expression<Func<TEntity, TProperty>> getPropertyExpression, Func<Action<TProperty>, Func<StringValues, VerboseResult>> action)
            {
                var memberExpression = (MemberExpression)getPropertyExpression.Body;
                Add(formField, memberExpression, action);
                return this;
            }

            private Editables Add<TProperty>(string formField, MemberExpression memberExpression, Func<Action<TProperty>, Func<StringValues, VerboseResult>> action)
            {
                var setter = memberExpression.CompileFunctionalSetter<TEntity, TProperty>();
                Func<TEntity, Func<StringValues, VerboseResult>> straightAction = e => action(setter(e));
                Add(formField, straightAction);
                return this;
            }
            #endregion

            public Editables Add(string formName, Func<TEntity, Func<StringValues, VerboseResult>> straightAction)
            {
                editableBinders.Add(formName, straightAction);
                return this;
            }
            public Editables Add((string formName, Func<TEntity, Func<StringValues, VerboseResult>> straightAction) tuple)
            {
                editableBinders.Add(tuple.formName, tuple.straightAction);
                return this;
            }
        }
        
        public class ManyToMany
        {
            readonly Dictionary<string, IManyToMany<TEntity>> manyToManyDictionary;

            public ManyToMany(Dictionary<string, IManyToMany<TEntity>> manyToManyDictionary)
            {
                this.manyToManyDictionary = manyToManyDictionary;
            }

            public ManyToMany Add<TF, TMM, TfID>(
                string formFieldName,
                string multiSelectListViewDataKey,
                Func<IRepository<TEntity>, IReadOnlyCollection<TF>> getOptions,
                
                Expression<Func<TEntity, ICollection<TMM>>> getRelatedExpression,
                
                Func<TMM, TfID> getTmmTfId,
                Func<TMM, TKey> getTmmTKey,
                Func<TF, TfID> getId,
                string multiSelectListFormDataValueField,
                string multiSelectListOptionText,

                Func<TEntity, TF, TMM> construct,
                Func<string, TfID> toId = null
                ) where TF : class where TMM : class
            {
                Func<TMM, TMM, bool> equalsById = (e1,e2) => getTmmTfId(e1).Equals(getTmmTfId(e2));

                var navigation = new MvcNavigationFacade<TEntity, TF, TMM, TfID>(
                    multiSelectListViewDataKey, 
                    getId, multiSelectListFormDataValueField, multiSelectListOptionText, construct, toId);
                var m = new ManyToMany<TEntity, TF, TMM, TfID>(
                    formFieldName, navigation, getOptions, getRelatedExpression, equalsById, getTmmTfId);
                manyToManyDictionary.Add(formFieldName, m);
                return this;
            }
        }

        public class OneToMany
        {
            readonly Dictionary<string, IOneToMany<TEntity>> onyToManyDictionary;

            public OneToMany(Dictionary<string, IOneToMany<TEntity>> onyToManyDictionary)
            {
                this.onyToManyDictionary = onyToManyDictionary;
            }

            public OneToMany Add<TF, TMM, TfID>(
                string formFieldName,
                string multiSelectListViewDataKey,
                Func<IRepository<TEntity>, IReadOnlyCollection<TF>> getOptions,

                Expression<Func<TEntity, ICollection<TMM>>> getRelatedExpression,

                Func<TMM, TfID> getTmmTfId,
                Func<TMM, TKey> getTmmTKey,
                Func<TF, TfID> getId,
                string multiSelectListFormDataValueField,
                string multiSelectListOptionText,

                Func<TEntity, TF, TMM> construct,
                Func<string, TfID> toId = null
                ) where TF : class where TMM : class
            {
                Func<TMM, TMM, bool> equalsById = (e1, e2) => getTmmTfId(e1).Equals(getTmmTfId(e2));

                var navigation = new MvcNavigationFacade<TEntity, TF, TMM, TfID>(
                    multiSelectListViewDataKey,
                    getId, multiSelectListFormDataValueField, multiSelectListOptionText, construct, toId);
                var m = new OneToMany<TEntity, TF, TMM, TfID>(
                    formFieldName, navigation, getOptions, getRelatedExpression, equalsById, getTmmTfId);
                onyToManyDictionary.Add(formFieldName, m);
                return this;
            }
        }

        public ControllerMeta(
            Func<TKey, Expression<Func<TEntity, bool>>> findByIdExpression,
            Func<string, ConvertFuncResult<TKey>> keyConverter,
             
            Include<TEntity> indexIncludes,
            Include<TEntity> editIncludes,
            Include<TEntity> disabledProperties,
            Action<Editables> addEditableBinders,
            Action<NotEditables> addNotEditableBinders,
            Action<OneToMany> oneToMany,
            Action<ManyToMany> manyToMany
            ) : this(findByIdExpression, keyConverter, indexIncludes, indexIncludes, indexIncludes, editIncludes, disabledProperties, addEditableBinders, addNotEditableBinders, oneToMany, manyToMany)
        {

        }

        

        public ControllerMeta(
            Func<TKey, Expression<Func<TEntity, bool>>> findByIdPredicate,
            Func<string, ConvertFuncResult<TKey>> keyConverter,

            Include<TEntity>   indexIncludes,
            Include<TEntity>   detailsIncludes,
            Include<TEntity>   deleteIncludes,

            Include<TEntity>   editIncludes,
            Include<TEntity>   disabledProperties,
            Action<Editables>  addEditableBinders,
            Action<NotEditables> addNotEditableBinders,
            Action<OneToMany> addOneToMany,
            Action<ManyToMany> addManyToMany
            )
        {
            this.Constructor = ()=>new TEntity();
            this.KeyConverter = keyConverter;
            //this.ReferencesMeta=referencesMeta;
            this.IndexIncludes = indexIncludes;
            this.DetailsIncludes = detailsIncludes;
            this.DeleteIncludes = deleteIncludes;
            this.EditIncludes = editIncludes;
            this.FindPredicate = findByIdPredicate;
            this.DisabledProperties = disabledProperties;

            //List<IManyToMany<TEntity>> manyToManyCollection
            var manyToManyBinders = new Dictionary<string, IManyToMany<TEntity>>();
            var manyToMany = new ManyToMany(manyToManyBinders);
            addManyToMany(manyToMany);

            var oneToManyBinders = new Dictionary<string, IOneToMany<TEntity>>();
            var oneToMany = new OneToMany(oneToManyBinders);
            addOneToMany(oneToMany);

            ReferencesMeta = new ReferencesCollection<TEntity>(oneToManyBinders, manyToManyBinders);

            this.editableBinders = new Dictionary<string, Func<TEntity, Func<StringValues, VerboseResult>>>();
            var editables = new Editables(editableBinders);
            addEditableBinders(editables);

            this.notEditableBinders = new Dictionary<string, Func<TEntity, Action<StringValues>>> ();
            var notEditables = new NotEditables(notEditableBinders);
            addNotEditableBinders(notEditables);
            //addNotEditableBinders((s, a) => this.notEditableBinders.Add(s, a));

        }
    }



    public class Binder
    {
        public static Func<StringValues, ConvertVerboseResult<string>> ConvertToString
        {
            get
            {
                return (stringValues) =>
                {
                    return new ConvertVerboseResult<string> { Value = stringValues.ToString() };
                };
            }
        }

        public static Func<StringValues, ConvertVerboseResult<int>> ConvertToInt
        {
            get
            {
                return (stringValues) =>
                {
                    var str = stringValues.ToString();
                    if (int.TryParse(str, out int number))
                        return new ConvertVerboseResult<int> { Value = number };
                    return new ConvertVerboseResult<int>("Not number!");
                };
            }
        }

        public static VerboseResult TryStringValidateLength(StringValues stringValues, Action<string> setter, int length)
        {
            var v = stringValues.ToString();
            setter(v);
            return new VerboseResult(v.Length > length ? "Too long!" : null);
        }
    }
}