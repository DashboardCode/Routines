using System;
using System.Linq;
using System.Collections.Generic;
using Microsoft.AspNetCore.Http;

namespace DashboardCode.Routines.AspNetCore
{
    
    public class ManyToMany2<TEntity, TF, TMM, TfID, TDAL, TDST> : IManyToMany<TEntity, TDAL, TDST> where TEntity : class where TF : class where TMM : class
    {
        private readonly Action<Action<string, object>, IReadOnlyCollection<TF>, IEnumerable<TfID>, IEnumerable<TfID>> addViewData;
        private readonly Func<TDAL, IReadOnlyCollection<TF>> getOptions;
        private readonly Action<TDAL, TDST, TEntity, List<TMM>> storeUpdate;
        private readonly Action<TDAL, TDST, TEntity, List<TMM>> storeInsert;
        //private readonly Expression<Func<TEntity, ICollection<TMM>>> getTmmExpression;
        private readonly Func<TEntity, ICollection<TMM>> getRelated;
        //private readonly Func<TMM, TMM, bool> equalsById;
        private readonly Func<TMM, TfID> getTmmTfId;
        private readonly string formFieldName0;
        private readonly string formFieldName1;

        private readonly Func<TF, TfID> getTfId;
        private readonly Func<TEntity, TF, bool, TMM> construct;
        private readonly Func<string, TfID> parseId;
        private readonly Func<TMM, bool> getTmmValue;

        public ManyToMany2(
            string formFieldName0,
            string formFieldName1,
            Action<Action<string, object>, IReadOnlyCollection<TF>, IEnumerable<TfID>, IEnumerable<TfID>> addViewData,
            Func<TDAL, IReadOnlyCollection<TF>> getOptions,
            //Action<TDST, TEntity, List<TMM>> getOldRelations,
            Action<TDAL, TDST, TEntity, List<TMM>> storeInsert,
            Action<TDAL, TDST, TEntity, List<TMM>> storeUpdate,
            Func<TEntity, ICollection<TMM>> getTmm, 
            //Func<TMM, TMM, bool> equalsById,
            Func<TMM, TfID> getTmmTfId,
            Func<TMM, bool> getTmmValue,
            Func<TF, TfID> getTfId,
            Func<TEntity, TF, bool, TMM> construct,
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
            this.formFieldName0 = formFieldName0;
            this.formFieldName1 = formFieldName1;
            this.getTfId = getTfId;
            this.construct = construct;
            this.parseId = parseId ?? Converters.GetParser<TfID>();

            this.getTmmValue = getTmmValue;
        }

        public void PrepareDefaultOptions(Action<string, object> addViewData, TDAL repository)
        {
            var options = getOptions(repository);
            this.addViewData(addViewData, options, new List<TfID>(), new List<TfID>());
        }

        public void PreparePersistedOptions(Action<string, object> addViewData, TDAL repository, out Action<TEntity> addViewDataMultiSelectList)
        {
            var options = getOptions(repository);
            addViewDataMultiSelectList = (entity) =>
            {
                var tmm = getRelated(entity);
                var selected0 = tmm?.Where(e=> !getTmmValue(e))?.Select(getTmmTfId);
                var selected1 = tmm?.Where(e => getTmmValue(e))?.Select(getTmmTfId);
                this.addViewData(addViewData, options, selected0, selected1);
            };
        }

        public void PrepareParsedOptionsOnInsert(Action<string, object> addViewData, TDAL repository, HttpRequest request, TEntity entity
            , out Action<TDST> modifyRelated
            , out Action addViewDataMultiSelectList)
        {
            var (selected, options, selectedIds0, selectedIds1) = Parse(repository, request, entity);
            modifyRelated = batch => storeInsert(repository, batch, entity, selected);
            addViewDataMultiSelectList = () => this.addViewData(addViewData, options, selectedIds0, selectedIds1);
        }

        public void PrepareParsedOptionsOnUpdate(Action<string, object> addViewData, TDAL repository,  HttpRequest request, TEntity entity
            , out Action<TDST> modifyRelated 
            , out Action addViewDataMultiSelectList)
        {
            var (selected, options, selectedIds0, selectedIds1) = Parse(repository, request, entity);
            modifyRelated = batch => storeUpdate(repository, batch, entity, selected);
            addViewDataMultiSelectList = () => this.addViewData(addViewData, options, selectedIds0, selectedIds1);
        }

        private (List<TMM> selected, IReadOnlyCollection<TF> options, List<TfID> selectedIds0, List<TfID> selectedIds1) Parse(TDAL repository, HttpRequest request, TEntity entity)
        {
            var options = getOptions(repository);
            var selected = new List<TMM>();

            var selectedIds0 = new List<TfID>();
            var selectedIds1 = new List<TfID>();

            var stringValues0 = request.Form[formFieldName0];
            if (stringValues0.Count() > 0)
            {
                foreach (var s in stringValues0)
                    selectedIds0.Add(parseId(s));
                options.Where(e => selectedIds0.Any(e2 =>  EqualityComparer<TfID>.Default.Equals(e2, getTfId(e))))
                    .ToList()
                    .ForEach(e => selected.Add(construct(entity, e, false)));
            }
            var stringValues1 = request.Form[formFieldName1];
            if (stringValues1.Count() > 0)
            {
                foreach (var s in stringValues1)
                    selectedIds1.Add(parseId(s));
                options.Where(e => 
                        selectedIds1.Any(e2 => EqualityComparer<TfID>.Default.Equals(e2, getTfId(e)))
                        && !selectedIds0.Any(e2=> EqualityComparer<TfID>.Default.Equals(e2, getTfId(e)))
                        )
                    .ToList()
                    .ForEach(e => selected.Add(construct(entity, e, true)));
            }

            return (selected, options, selectedIds0, selectedIds1);
        }
    }
}
