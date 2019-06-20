using System;
using System.Linq;
using System.Collections.Generic;
using Microsoft.AspNetCore.Http;

namespace DashboardCode.Routines.AspNetCore
{
    public class ManyToMany<TEntity, TF, TMM, TfID, TDAL, TDST> : IManyToMany<TEntity, TDAL, TDST> where TEntity : class where TF : class where TMM : class
    {
        private readonly Action<Action<string, object>, IReadOnlyCollection<TF>, IEnumerable<TfID>> addViewData;
        private readonly Func<TDAL, IReadOnlyCollection<TF>> getOptions;
        private readonly Action<TDAL, TDST, TEntity, List<TMM>> storeUpdate;
        private readonly Action<TDAL, TDST, TEntity, List<TMM>> storeInsert;
        //private readonly Expression<Func<TEntity, ICollection<TMM>>> getTmmExpression;
        private readonly Func<TEntity, ICollection<TMM>> getRelated;
        //private readonly Func<TMM, TMM, bool> equalsById;
        private readonly Func<TMM, TfID> getTmmTfId;
        private readonly string formFieldName;

        private readonly Func<TF, TfID> getTfId;
        private readonly Func<TEntity, TF, TMM> construct;
        private readonly Func<string, TfID> parseId;
        //private readonly bool disabled;

        public ManyToMany(
            string formFieldName,
            Action<Action<string, object>, IReadOnlyCollection<TF>, IEnumerable<TfID>> addViewData,
            Func<TDAL, IReadOnlyCollection<TF>> getOptions,
            //Action<TDST, TEntity, List<TMM>> getOldRelations,
            Action<TDAL, TDST, TEntity, List<TMM>> storeInsert,
            Action<TDAL, TDST, TEntity, List<TMM>> storeUpdate,
            Func<TEntity, ICollection<TMM>> getTmm, 
            //Func<TMM, TMM, bool> equalsById,
            Func<TMM, TfID> getTmmTfId,

            Func<TF, TfID> getTfId,
            Func<TEntity, TF, TMM> construct,
            //bool disabled,
            Func<string, TfID> parseId = null
            )
        {
            // common
            this.addViewData = addViewData;
            this.getOptions = getOptions;

            // used only in PreparePersistedOptions
            this.getRelated = getTmm;// Expression.Compile();
            this.getTmmTfId = getTmmTfId;

            // used only in PrepareParsedOptions
            this.storeUpdate = storeUpdate;
            this.storeInsert = storeInsert;
            //this.getTmmExpression = getTmmExpression;
            //this.equalsById = equalsById;
            this.formFieldName = formFieldName;
            this.getTfId = getTfId;
            this.construct = construct;
            //this.disabled = disabled;
            this.parseId = parseId ?? Converters.GetParser<TfID>();
        }

        public void PrepareDefaultOptions(Action<string, object> addViewData, TDAL repository)
        {
            var options = getOptions(repository);
            this.addViewData(addViewData, options, new List<TfID>());
        }

        public void PreparePersistedOptions(Action<string, object> addViewData, TDAL repository, out Action<TEntity> addViewDataMultiSelectList)
        {
            var options = getOptions(repository);
            addViewDataMultiSelectList = (entity) =>
            {
                var tmm = getRelated(entity);
                var selected = tmm?.Select(getTmmTfId);
                this.addViewData(addViewData, options, selected);
            };
        }

        public void PrepareParsedOptionsOnInsert(Action<string, object> addViewData, TDAL repository, HttpRequest request, TEntity entity
            , out Action<TDST> modifyRelated
            , out Action addViewDataMultiSelectList)
        {
            var (selected, options, selectedIds) = Parse(repository, request, entity);
            modifyRelated = batch => storeInsert(repository, batch, entity, selected);
            addViewDataMultiSelectList = () => this.addViewData(addViewData, options, selectedIds);
        }

        public void PrepareParsedOptionsOnUpdate(Action<string, object> addViewData, TDAL repository,  HttpRequest request, TEntity entity
            , out Action<TDST> modifyRelated 
            , out Action addViewDataMultiSelectList)
        {
            var (selected, options, selectedIds) = Parse(repository, request, entity);
            modifyRelated = batch => storeUpdate(repository, batch, entity, selected);
            addViewDataMultiSelectList = () => this.addViewData(addViewData, options, selectedIds);
        }

        private (List<TMM> selected, IReadOnlyCollection<TF> options, List<TfID> selectedIds) Parse(TDAL repository, HttpRequest request, TEntity entity)
        {
            var options = getOptions(repository);

            var selectedIds = new List<TfID>();
            var selected = new List<TMM>();
            var stringValues = request.Form[formFieldName];
            if (stringValues.Count() > 0)
            {
                foreach (var s in stringValues)
                    selectedIds.Add(parseId(s));
                var existed = options.Where(e => selectedIds.Any(e2 => EqualityComparer<TfID>.Default.Equals(e2, getTfId(e))))
                    .ToList();
                existed.ForEach(e => selected.Add(construct(entity, e)));
            }
            return (selected, options, selectedIds);
        }
    }

    public class ManyToManyDisabled<TEntity, TF, TMM, TfID, TDAL, TDST> : IManyToManyDisabled<TEntity, TDAL, TDST> where TEntity : class where TF : class where TMM : class
    {
        private readonly Action<Action<string, object>, IReadOnlyCollection<TF>, IEnumerable<TfID>> addViewData;
        private readonly Func<TDAL, IReadOnlyCollection<TF>> getOptions;
        private readonly Func<TEntity, ICollection<TMM>> getRelated;
        private readonly Func<TMM, TfID> getTmmTfId;

        public ManyToManyDisabled(
            Action<Action<string, object>, IReadOnlyCollection<TF>, IEnumerable<TfID>> addViewData,
            Func<TDAL, IReadOnlyCollection<TF>> getOptions,
            Func<TEntity, ICollection<TMM>> getTmm,
            Func<TMM, TfID> getTmmTfId
            )
        {
            // common
            this.addViewData = addViewData;
            this.getOptions = getOptions;

            // used only in PreparePersistedOptions
            this.getRelated = getTmm;
            this.getTmmTfId = getTmmTfId;
        }

        public void PrepareDefaultOptions(Action<string, object> addViewData, TDAL repository)
        {
            var options = getOptions(repository);
            this.addViewData(addViewData, options, new List<TfID>());
        }

        public void PreparePersistedOptions(Action<string, object> addViewData, TDAL repository, out Action<TEntity> addViewDataMultiSelectList)
        {
            var options = getOptions(repository);
            addViewDataMultiSelectList = (entity) =>
            {
                var tmm = getRelated(entity);
                var selected = tmm?.Select(getTmmTfId);
                this.addViewData(addViewData, options, selected);
            };
        }
    }
}