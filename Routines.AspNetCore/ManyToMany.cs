using System;
using System.Linq;
using System.Linq.Expressions;
using System.Collections.Generic;
using Microsoft.AspNetCore.Http;

using DashboardCode.Routines.Storage;

namespace DashboardCode.Routines.AspNetCore
{
    public class ManyToMany<TEntity, TF, TMM, TfID> : IManyToMany<TEntity> where TEntity : class where TF : class where TMM : class
    {
        private readonly Action<Action<string, object>, IReadOnlyCollection<TF>, IEnumerable<TfID>> addViewData;
        private readonly Func<IRepository<TEntity>, IReadOnlyCollection<TF>> getOptions;
        private readonly Expression<Func<TEntity, ICollection<TMM>>> getRelatedExpression;
        private readonly Func<TEntity, ICollection<TMM>> getRelated;
        private readonly Func<TMM, TMM, bool> equalsById;
        private readonly Func<TMM, TfID> getTmmId;
        private readonly string formField;

        private readonly Func<TF, TfID> getId;
        private readonly Func<TEntity, TF, TMM> construct;
        private readonly Func<string, TfID> toId;

        public ManyToMany(
            string formField,
            Action<Action<string, object>, IReadOnlyCollection<TF>, IEnumerable<TfID>> addViewData,
            Func<IRepository<TEntity>, IReadOnlyCollection<TF>> getOptions,
            Expression<Func<TEntity, ICollection<TMM>>> getRelatedExpression, 
            Func<TMM, TMM, bool> equalsById,
            Func<TMM, TfID> getTmmId,

            Func<TF, TfID> getId,
            Func<TEntity, TF, TMM> construct,
            Func<string, TfID> toId=null
            )
        {
            // common
            this.addViewData = addViewData;
            this.getOptions = getOptions;

            // used only in PreparePersistedOptions
            this.getRelated = getRelatedExpression.Compile();
            this.getTmmId = getTmmId;

            // used only in PrepareParsedOptions
            this.getRelatedExpression = getRelatedExpression;
            this.equalsById = equalsById;
            this.formField = formField;
            this.getId = getId;
            this.construct = construct;
            this.toId = toId?? Converters.GetParser<TfID>();
        }

        public void PrepareDefaultOptions(Action<string, object> addViewData, IRepository<TEntity> repository)
        {
            var options = getOptions(repository);
            this.addViewData(addViewData, options, new List<TfID>());
        }

        public void PreparePersistedOptions(Action<string, object> addViewData, IRepository<TEntity> repository, out Action<TEntity> addViewDataMultiSelectList)
        {
            var options = getOptions(repository);
            addViewDataMultiSelectList = (entity) =>
            {
                var tmm = getRelated(entity);
                var selected = tmm?.Select(getTmmId);
                this.addViewData(addViewData, options, selected);
            };
        }

        public void PrepareParsedOptions(Action<string, object> addViewData, IRepository<TEntity> repository,  HttpRequest request, TEntity entity,  out Action<IBatch<TEntity>> modifyRelated, out Action addViewDataMultiSelectList)
        {
            var options = getOptions(repository);

            var selectedIds = new List<TfID>();
            var selected = new List<TMM>();
            var stringValues = request.Form[formField];
            if (stringValues.Count() > 0)
            {
                foreach (var s in stringValues)
                    selectedIds.Add(toId(s));
                options.Where(e => selectedIds.Any(e2 => EqualityComparer<TfID>.Default.Equals(e2, getId(e))))
                    .ToList()
                    .ForEach(e => selected.Add(construct(entity, e)));
            }

            modifyRelated = batch => batch.ModifyRelated(entity, getRelatedExpression, selected, equalsById);
            addViewDataMultiSelectList = () => this.addViewData(addViewData, options, selectedIds);
        }
    }
}