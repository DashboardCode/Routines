using System;
using System.Linq;
using System.Linq.Expressions;
using System.Collections.Generic;
using Microsoft.AspNetCore.Http;

using DashboardCode.Routines.Storage;

namespace DashboardCode.Routines.AspNetCore
{
    public class ManyToMany<TEntity, TF, TMM, TfID, TDAL> : IManyToMany<TEntity, TDAL> where TEntity : class where TF : class where TMM : class
    {
        private readonly Action<Action<string, object>, IReadOnlyCollection<TF>, IEnumerable<TfID>> addViewData;
        private readonly Func<TDAL, IReadOnlyCollection<TF>> getOptions;
        private readonly Expression<Func<TEntity, ICollection<TMM>>> getTmmExpression;
        private readonly Func<TEntity, ICollection<TMM>> getRelated;
        private readonly Func<TMM, TMM, bool> equalsById;
        private readonly Func<TMM, TfID> getTmmTfId;
        private readonly string formField;

        private readonly Func<TF, TfID> getTfId;
        private readonly Func<TEntity, TF, TMM> construct;
        private readonly Func<string, TfID> parseId;

        public ManyToMany(
            string formField,
            Action<Action<string, object>, IReadOnlyCollection<TF>, IEnumerable<TfID>> addViewData,
            Func<TDAL, IReadOnlyCollection<TF>> getOptions,
            Expression<Func<TEntity, ICollection<TMM>>> getTmmExpression, 
            Func<TMM, TMM, bool> equalsById,
            Func<TMM, TfID> getTmmTfId,

            Func<TF, TfID> getTfId,
            Func<TEntity, TF, TMM> construct,
            Func<string, TfID> parseId = null
            )
        {
            // common
            this.addViewData = addViewData;
            this.getOptions = getOptions;

            // used only in PreparePersistedOptions
            this.getRelated = getTmmExpression.Compile();
            this.getTmmTfId = getTmmTfId;

            // used only in PrepareParsedOptions
            this.getTmmExpression = getTmmExpression;
            this.equalsById = equalsById;
            this.formField = formField;
            this.getTfId = getTfId;
            this.construct = construct;
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

        public void PrepareParsedOptions(Action<string, object> addViewData, TDAL repository,  HttpRequest request, TEntity entity,  out Action<IBatch<TEntity>> modifyRelated, out Action addViewDataMultiSelectList)
        {
            var options = getOptions(repository);

            var selectedIds = new List<TfID>();
            var selected = new List<TMM>();
            var stringValues = request.Form[formField];
            if (stringValues.Count() > 0)
            {
                foreach (var s in stringValues)
                    selectedIds.Add(parseId(s));
                options.Where(e => selectedIds.Any(e2 => EqualityComparer<TfID>.Default.Equals(e2, getTfId(e))))
                    .ToList()
                    .ForEach(e => selected.Add(construct(entity, e)));
            }

            modifyRelated = batch => batch.ModifyRelated(entity, getTmmExpression, selected, equalsById);
            addViewDataMultiSelectList = () => this.addViewData(addViewData, options, selectedIds);
        }
    }
}