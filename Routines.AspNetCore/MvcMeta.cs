using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using Microsoft.AspNetCore.Mvc.Rendering; // SelectList and MultySelectList
using Microsoft.Extensions.Primitives;

using DashboardCode.Routines.Storage;
using System.Linq;

namespace DashboardCode.Routines.AspNetCore
{
    public class MvcMeta<TEntity, TKey> where TEntity : class, new()
    {
        public readonly Func<TEntity> Constructor;
        
        public readonly Func<string, ValuableResult<TKey>> KeyConverter;
        public readonly Include<TEntity> IndexIncludes;
        public readonly Include<TEntity> DetailsIncludes;
        public readonly Include<TEntity> DeleteIncludes;
        public readonly Include<TEntity> EditIncludes;
        public readonly Func<TKey, Expression<Func<TEntity, bool>>> FindPredicate;
        public readonly string formPrefix;

        public readonly Dictionary<string, Func<TEntity, Func<StringValues, IVerboseResult<List<string>>>>> FormFields;
        public readonly Dictionary<string, Func<TEntity, Action<StringValues>>> HiddenFormFields;
        public readonly Include<TEntity> DisabledFormFields;

        public readonly ReferencesCollection<TEntity, IRepository<TEntity>, IBatch<TEntity>> ReferencesCollection;

        public class HiddenFormFieldsScorer
        {
            readonly Dictionary<string, Func<TEntity, Action<StringValues>>> notEditableBinders;
            public HiddenFormFieldsScorer(Dictionary<string, Func<TEntity, Action<StringValues>>> notEditableBinders)
            {
                this.notEditableBinders = notEditableBinders;
            }

            public HiddenFormFieldsScorer Add<TProperty>(Expression<Func<TEntity, TProperty>> getProperty)
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
                    throw new NotSupportedException($"Type '{propertyType.Name}' is not supported by automation on '{nameof(HiddenFormFieldsScorer)}' controller's meta configuration");
                }
                Add(memberExpression.Member.Name, memberExpression, converter);
                return this;
            }

            #region provide converter
            public HiddenFormFieldsScorer Add<TProperty>(Expression<Func<TEntity, TProperty>> getProperty, Func<StringValues, TProperty> converter,
                Func<Func<TProperty>, Action<Action<TProperty>>> action = null
            )
            {
                var memberExpression = (MemberExpression)getProperty.Body;
                Add(memberExpression.Member.Name, memberExpression, converter, action);
                return this;
            }

            public HiddenFormFieldsScorer Add<TProperty>(string formField, Expression<Func<TEntity, TProperty>> getProperty, Func<StringValues, TProperty> converter,
                Func<Func<TProperty>, Action<Action<TProperty>>> action = null
            )
            {
                Add(formField, (MemberExpression)getProperty.Body, converter, action);
                return this;
            }

            private HiddenFormFieldsScorer Add<TProperty>(string formField, MemberExpression memberExpression, Func<StringValues, TProperty> converter,
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
            public HiddenFormFieldsScorer Add<TProperty>(Expression<Func<TEntity, TProperty>> getPropertyExpression, Func<Action<TProperty>, Action<StringValues>> action)
            {
                var memberExpression = (MemberExpression)getPropertyExpression.Body;
                return Add(memberExpression.Member.Name, memberExpression, action);
            }

            public HiddenFormFieldsScorer Add<TProperty>(string formName, Expression<Func<TEntity, TProperty>> getPropertyExpression, Func<Action<TProperty>, Action<StringValues>> action)
            {
                return Add(formName, (MemberExpression)getPropertyExpression.Body, action);
            }

            private HiddenFormFieldsScorer Add<TProperty>(string formName, MemberExpression memberExpression, Func<Action<TProperty>, Action<StringValues>> action)
            {
                var setter = memberExpression.CompileFunctionalSetter<TEntity, TProperty>();
                Func<TEntity, Action<StringValues>> straightAction = e=>action(setter(e));
                Add(formName, straightAction);
                return this;
            }
            #endregion

            public HiddenFormFieldsScorer Add(string formName, Func<TEntity, Action<StringValues>> straightAction)
            {
                notEditableBinders.Add(formName, straightAction);
                return this;
            }
        }

        public class FormFieldsScorer
        {
            readonly Dictionary<string, Func<TEntity, Func<StringValues, IVerboseResult<List<string>>>>> editableBinders;
            public FormFieldsScorer(Dictionary<string, Func<TEntity, Func<StringValues, IVerboseResult<List<string>>>>> editableBinders)
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

                public static Func<TProperty, IVerboseResult<List<string>>> ToValidate(Action<AssertsCollection<TProperty>> addAsserts)
                {
                    Func<TProperty, IVerboseResult<List<string>>> validate = v =>
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


            public FormFieldsScorer Add<TProperty>(
                string formField,
                Expression<Func<TEntity, TProperty>> getPropertyExpression,
                Func<StringValues, ConvertResult<TProperty>> convert,
                Action<AssertsCollection<TProperty>> addAsserts,
                Func<Func<ConvertResult<TProperty>>, Func<Func<ConvertResult<TProperty>, ConvertResult<TProperty>>, Func<Func<ConvertResult<TProperty>, IVerboseResult<List<string>>>, IVerboseResult<List<string>>>>> action = null)
            {
                var validate = AssertsCollection<TProperty>.ToValidate(addAsserts);
                Add(formField, getPropertyExpression, convert, validate, action);
                return this;
            }

            public FormFieldsScorer Add<TProperty>(
                Expression<Func<TEntity, TProperty>> getPropertyExpression,
                Func<StringValues, ConvertResult<TProperty>> convert,
                Action<AssertsCollection<TProperty>> addAsserts,
                Func<Func<ConvertResult<TProperty>>, Func<Func<ConvertResult<TProperty>, ConvertResult<TProperty>>, Func<Func<ConvertResult<TProperty>, IVerboseResult<List<string>>>, IVerboseResult<List<string>>>>> action = null)
            {
                var validate = AssertsCollection<TProperty>.ToValidate(addAsserts);
                Add(getPropertyExpression, convert, validate, action);
                return this;
            }

            public FormFieldsScorer Add<TProperty>(
                Expression<Func<TEntity, TProperty>> getPropertyExpression,
                Func<StringValues, ConvertResult<TProperty>> convert,
                Func<TProperty, IVerboseResult<List<string>>> validate = null,
                Func<Func<ConvertResult<TProperty>>, Func<Func<ConvertResult<TProperty>, ConvertResult<TProperty>>, Func<Func<ConvertResult<TProperty>, IVerboseResult<List<string>>>, IVerboseResult<List<string>>>>> action = null)
            {
                var memberExpression = (MemberExpression)getPropertyExpression.Body;
                Add(memberExpression.Member.Name, (MemberExpression)getPropertyExpression.Body, convert, validate, action);
                return this;
            }

            public FormFieldsScorer Add<TProperty>(
                string formField,
                Expression<Func<TEntity, TProperty>> getPropertyExpression,
                Func<StringValues, ConvertResult<TProperty>> convert,
                Func<TProperty, IVerboseResult<List<string>>> validate=null,
                Func<Func<ConvertResult<TProperty>>, Func<Func<ConvertResult<TProperty>, ConvertResult<TProperty>>, Func<Func<ConvertResult<TProperty>, IVerboseResult<List<string>>>, IVerboseResult<List<string>>>>> action = null)
            {
                Add(formField, (MemberExpression)getPropertyExpression.Body, convert, validate, action);
                return this;
            }

            private FormFieldsScorer Add<TProperty>(
                string formField,
                MemberExpression memberExpression,
                Func<StringValues, ConvertResult<TProperty>> convert,
                Func<TProperty, IVerboseResult<List<string>>> validate=null,
                Func<Func<ConvertResult<TProperty>>, Func<Func<ConvertResult<TProperty>, ConvertResult<TProperty>>, Func<Func<ConvertResult<TProperty>, IVerboseResult<List<string>>>, IVerboseResult<List<string>>>>> action = null)
            {
                if (action == null)
                    action = converter => setter => validator => validator(setter(converter()));
                var set = memberExpression.CompileSetProperty<TEntity, TProperty>();
                Func<TEntity, Func<StringValues, IVerboseResult<List<string>>>> straightAction;
                if (validate==null)
                    straightAction = e => sv => action(() => convert(sv))(cr => { if (cr.IsOk()) set(e, cr.Value); return cr; })(cr =>  cr.ToVerboseResult());
                else
                    straightAction = e => sv => action(() => convert(sv))(cr => { if (cr.IsOk()) set(e, cr.Value); return cr; })
                        (cr => { if (cr.IsOk()) return validate(cr.Value); return cr.ToVerboseResult(); });
                editableBinders.Add(formField, straightAction);
                return this;
            }
            #endregion

            #region provide with setters & convert
            public FormFieldsScorer Add<TProperty>(
                Expression<Func<TEntity, TProperty>> getPropertyExpression,
                Func<StringValues, ConvertResult<TProperty>> convert,
                Func<Func<ConvertResult<TProperty>>, Func<Func<ConvertResult<TProperty>, ConvertResult<TProperty>>, IVerboseResult<List<string>>>> action)
            {
                var memberExpression = (MemberExpression)getPropertyExpression.Body;
                Add(memberExpression.Member.Name, memberExpression, convert, action);
                return this;
            }

            public FormFieldsScorer Add<TProperty>(
                string formField,
                Expression<Func<TEntity, TProperty>> getPropertyExpression,
                Func<StringValues, ConvertResult<TProperty>> convert,
                Func<Func<ConvertResult<TProperty>>, Func<Func<ConvertResult<TProperty>, ConvertResult<TProperty>>, IVerboseResult<List<string>>>> action)
            {
                Add(formField, (MemberExpression)getPropertyExpression.Body, convert, action);
                return this;
            }
            public FormFieldsScorer Add<TProperty>(
                string formField,
                MemberExpression memberExpression,
                Func<StringValues, ConvertResult<TProperty>> convert,
                Func<Func<ConvertResult<TProperty>>, Func<Func<ConvertResult<TProperty>, ConvertResult<TProperty>>, IVerboseResult<List<string>>>> action)
            {
                var setter = memberExpression.CompileSetProperty<TEntity, TProperty>();
                Func<TEntity, Func<StringValues, IVerboseResult<List<string>>>> straightAction = e => sv => action(() => convert(sv))(cr => { if (cr.IsOk()) setter(e, cr.Value); return cr; });
                editableBinders.Add(formField, straightAction);
                return this;
            }
            #endregion

            #region provide with setters
            public FormFieldsScorer Add<TProperty>(Expression<Func<TEntity, TProperty>> getPropertyExpression, Func<Action<TProperty>, Func<StringValues, IVerboseResult<List<string>>>> action)
            {
                var memberExpression = (MemberExpression)getPropertyExpression.Body;
                Add(memberExpression.Member.Name, memberExpression, action);
                return this;
            }

            public FormFieldsScorer Add<TProperty>(string formField, Expression<Func<TEntity, TProperty>> getPropertyExpression, Func<Action<TProperty>, Func<StringValues, IVerboseResult<List<string>>>> action)
            {
                var memberExpression = (MemberExpression)getPropertyExpression.Body;
                Add(formField, memberExpression, action);
                return this;
            }

            private FormFieldsScorer Add<TProperty>(string formField, MemberExpression memberExpression, Func<Action<TProperty>, Func<StringValues, IVerboseResult<List<string>>>> action)
            {
                var setter = memberExpression.CompileFunctionalSetter<TEntity, TProperty>();
                Func<TEntity, Func<StringValues, IVerboseResult<List<string>>>> straightAction = e => action(setter(e));
                Add(formField, straightAction);
                return this;
            }
            #endregion

            public FormFieldsScorer Add(string formName, Func<TEntity, Func<StringValues, IVerboseResult<List<string>>>> straightAction)
            {
                editableBinders.Add(formName, straightAction);
                return this;
            }
            public FormFieldsScorer Add((string formName, Func<TEntity, Func<StringValues, IVerboseResult<List<string>>>> straightAction) tuple)
            {
                editableBinders.Add(tuple.formName, tuple.straightAction);
                return this;
            }
        }
        
        public class ManyToManyScorer
        {
            readonly Dictionary<string, IManyToMany<TEntity, IRepository<TEntity>, IBatch<TEntity>>> manyToManyDictionary;

            public ManyToManyScorer(Dictionary<string, IManyToMany<TEntity, IRepository<TEntity>, IBatch<TEntity>>> manyToManyDictionary) =>
                this.manyToManyDictionary = manyToManyDictionary;

            public ManyToManyScorer Add<TF, TMM, TfID>(
                string formFieldName,
                string viewDataMultiSelectListKey,
                Func<IRepository<TEntity>, IReadOnlyCollection<TF>> getOptions,
                
                Expression<Func<TEntity, ICollection<TMM>>> getTmmExpression,
                
                Func<TMM, TfID> getTmmTfId,
                Func<TMM, TKey> getTmmTKey,
                Func<TF, TfID> getTfId,
                string multiSelectListOptionValuePropertyName,
                string multiSelectListOptionTextPropertyName,

                Func<TEntity, TF, TMM> construct,
                Func<string, TfID> toId = null
                ) where TF : class where TMM : class
            {
                Func<TMM, TMM, bool> equalsById = (e1,e2) => getTmmTfId(e1).Equals(getTmmTfId(e2));

                Action<Action<string, object>, IReadOnlyCollection<TF>, IEnumerable<TfID>> addViewData2 =
                    (addViewData, options, selectedIds) =>
                        addViewData(viewDataMultiSelectListKey, 
                        new MultiSelectList(options, multiSelectListOptionValuePropertyName, multiSelectListOptionTextPropertyName, selectedIds.ToList()));

                var getTmm = getTmmExpression.Compile();
                var memberExpression = (MemberExpression)getTmmExpression.Body;
                var setTmm = MemberExpressionExtensions.CompileSetPropertyCovariance<TEntity, IEnumerable<TMM>>(memberExpression);

                Expression<Func<TEntity, IEnumerable<TMM>>> getRelationAsEnumerable = getTmmExpression.ContravarianceToIEnumerable();

                var manyToMany = new ManyToMany<TEntity, TF, TMM, TfID, IRepository<TEntity>, IBatch<TEntity>>(
                    formFieldName, addViewData2, getOptions,
                    (repository, batch, entity, selected) =>
                    {
                        var oldRelations = new List<TMM>();
                        setTmm(entity, oldRelations);
                        batch.ModifyRelated(entity, oldRelations, selected, equalsById);
                    },
                    (repository, batch, entity, selected) =>
                        {
                            repository.LoadCollection(entity, getRelationAsEnumerable);
                            var oldRelations = getTmm(entity);
                            batch.ModifyRelated(entity, oldRelations, selected, equalsById);
                        },
                    getTmmExpression.Compile(), 
                    getTmmTfId, getTfId, construct, toId);
                manyToManyDictionary.Add(formFieldName, manyToMany);
                return this;
            }

            public ManyToManyScorer Add<TF, TMM, TfID>(
                string formFieldName0,
                string formFieldName1,
                string viewDataMultiSelectListKey0,
                string viewDataMultiSelectListKey1,
                Func<IRepository<TEntity>, IReadOnlyCollection<TF>> getOptions,

                Expression<Func<TEntity, ICollection<TMM>>> getTmmExpression,

                Func<TMM, TfID> getTmmTfId,
                Func<TMM, TKey> getTmmTKey,
                Func<TMM, bool> getTmmValue,
                Func<TF, TfID> getTfId,
                Func<TMM, TMM, bool> equalsByValue,
                Action<TMM, TMM> updateValue,
                string multiSelectListOptionValuePropertyName,
                string multiSelectListOptionTextPropertyName,

                Func<TEntity, TF, bool, TMM> construct,
                Func<string, TfID> toId = null
                ) where TF : class where TMM : class
            {
                Func<TMM, TMM, bool> equalsById = (e1, e2) => getTmmTfId(e1).Equals(getTmmTfId(e2));

                Action<Action<string, object>, IReadOnlyCollection<TF>, IEnumerable<TfID>, IEnumerable<TfID>> addViewData2 =
                    (addViewData, options, selectedIds0, selectedIds1) => {
                        addViewData(viewDataMultiSelectListKey0,
                            new MultiSelectList(options, multiSelectListOptionValuePropertyName, multiSelectListOptionTextPropertyName, selectedIds0.ToList()));
                        addViewData(viewDataMultiSelectListKey1,
                            new MultiSelectList(options, multiSelectListOptionValuePropertyName, multiSelectListOptionTextPropertyName, selectedIds1.ToList()));
                    };

                var getTmm = getTmmExpression.Compile();
                var memberExpression = (MemberExpression)getTmmExpression.Body;
                var setTmm = MemberExpressionExtensions.CompileSetPropertyCovariance<TEntity, IEnumerable<TMM>>(memberExpression);

                Expression<Func<TEntity, IEnumerable<TMM>>> getRelationAsEnumerable = getTmmExpression.ContravarianceToIEnumerable();

                var manyToMany = new ManyToMany2<TEntity, TF, TMM, TfID, IRepository<TEntity>, IBatch<TEntity>>(
                    formFieldName0, formFieldName1, addViewData2, getOptions,
                    (repository, batch, entity, selected) =>
                    {
                        var oldRelations = new List<TMM>();
                        setTmm(entity, oldRelations);
                        batch.ModifyRelated(entity, oldRelations, selected, equalsById, equalsByValue, updateValue);
                    },
                    (repository, batch, entity, selected) =>
                    {
                        repository.LoadCollection(entity, getRelationAsEnumerable);
                        var oldRelations = getTmm(entity);
                        batch.ModifyRelated(entity, oldRelations, selected, equalsById, equalsByValue, updateValue);
                    },
                    getTmmExpression.Compile(),
                    getTmmTfId,
                    getTmmValue,
                    getTfId, construct, toId);
                manyToManyDictionary.Add(formFieldName0+ "."+formFieldName1, manyToMany);
                return this;
            }
        }

        public class OneToManyScorer
        {
            readonly Dictionary<string, IOneToMany<TEntity, IRepository<TEntity>>> onyToManyDictionary;

            public OneToManyScorer(Dictionary<string, IOneToMany<TEntity, IRepository<TEntity>>> onyToManyDictionary) =>
                this.onyToManyDictionary = onyToManyDictionary;

            public OneToManyScorer Add<TF, TMM, TfID>(
                string formFieldName,
                string viewDataSelectListKey,
                Func<IRepository<TEntity>, IReadOnlyCollection<TF>> getOptions,

                //Func<TF, TfID> getId,
                Func<TEntity, TfID> getRefId,

                string selectListOptionValuePropertyName,
                string selectListOptionTextPropertyName//,

                //Func<TEntity, TF, TMM> construct,
                //Func<string, TfID> toId = null
                ) where TF : class where TMM : class
            {

                Action<Action<string, object>, IReadOnlyCollection<TF>, TfID> addViewData2 =
                    (addViewData, options, selectedId) =>
                        addViewData(viewDataSelectListKey, new SelectList(options, selectListOptionValuePropertyName, selectListOptionTextPropertyName, selectedId));

                var m = new OneToMany<TEntity, TF, TfID, IRepository<TEntity>>(
                    formFieldName, addViewData2, getOptions, getRefId);
                onyToManyDictionary.Add(formFieldName, m);
                return this;
            }
        }

        public MvcMeta(
            Func<TKey, Expression<Func<TEntity, bool>>> findByIdExpression,
            Func<string, ValuableResult<TKey>> keyConverter,
            Include<TEntity> indexIncludes,
            Include<TEntity> editIncludes,
            Include<TEntity> disabledProperties,
            Action<FormFieldsScorer> addEditableBinders,
            Action<HiddenFormFieldsScorer> addNotEditableBinders,
            Action<OneToManyScorer> oneToMany,
            Action<ManyToManyScorer> manyToMany
            ) : this(findByIdExpression, keyConverter, indexIncludes, indexIncludes, indexIncludes, editIncludes, disabledProperties, addEditableBinders, addNotEditableBinders, oneToMany, manyToMany)
        {

        }

        public MvcMeta(
            Func<TKey, Expression<Func<TEntity, bool>>> findByIdPredicate,
            Func<string, ValuableResult<TKey>> keyConverter,

            Include<TEntity>   indexIncludes,
            Include<TEntity>   detailsIncludes,
            Include<TEntity>   deleteIncludes,

            Include<TEntity>   editIncludes,
            Include<TEntity>   disabledProperties,
            Action<FormFieldsScorer>  addFieldBinders,
            Action<HiddenFormFieldsScorer> addFieldSetters,
            Action<OneToManyScorer> addOneToMany,
            Action<ManyToManyScorer> addManyToMany
            )
        {
            this.Constructor = () => new TEntity();
            this.KeyConverter = keyConverter;
            this.IndexIncludes = indexIncludes;
            this.DetailsIncludes = detailsIncludes;
            this.DeleteIncludes = deleteIncludes;
            this.EditIncludes = editIncludes;
            this.FindPredicate = findByIdPredicate;
            this.DisabledFormFields = disabledProperties;
            this.formPrefix = formPrefix;

            var manyToManyBinders = new Dictionary<string, IManyToMany<TEntity, IRepository<TEntity>, IBatch<TEntity>>>();
            addManyToMany?.Invoke(new ManyToManyScorer(manyToManyBinders));
            var oneToManyBinders = new Dictionary<string, IOneToMany<TEntity, IRepository<TEntity>>>();
            addOneToMany?.Invoke(new OneToManyScorer(oneToManyBinders));

            this.ReferencesCollection = new ReferencesCollection<TEntity, IRepository<TEntity>, IBatch<TEntity>>(oneToManyBinders, manyToManyBinders);

            this.FormFields = new Dictionary<string, Func<TEntity, Func<StringValues, IVerboseResult<List<string>>>>>();
            addFieldBinders?.Invoke(new FormFieldsScorer(FormFields));

            this.HiddenFormFields = new Dictionary<string, Func<TEntity, Action<StringValues>>> ();
            addFieldSetters(new HiddenFormFieldsScorer(HiddenFormFields));
        }
    }
}