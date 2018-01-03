using System;
using System.Linq;
using System.Linq.Expressions;
using System.Collections.Generic;

using DashboardCode.Routines.Storage;
using Microsoft.AspNetCore.Http;

namespace DashboardCode.Routines.AspNetCore
{
    public class ManyToMany<TEntity, TF, TMM, TfID> : IManyToMany<TEntity> where TEntity : class where TF : class where TMM : class
    {
        private readonly MvcViewDataMultiSelectListFacade<TF, TfID> navigation;
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
            MvcViewDataMultiSelectListFacade<TF, TfID> navigation,
            Func<IRepository<TEntity>, IReadOnlyCollection<TF>> getOptions,
            Expression<Func<TEntity, ICollection<TMM>>> getRelatedExpression, 
            Func<TMM, TMM, bool> equalsById,
            Func<TMM, TfID> getTmmId,

            Func<TF, TfID> getId,
            Func<TEntity, TF, TMM> construct,
            Func<string, TfID> toId=null
            )
        {
            this.navigation = navigation;
            this.getOptions = getOptions;

            this.getRelated = getRelatedExpression.Compile();
            this.getTmmId = getTmmId;

            this.getRelatedExpression = getRelatedExpression;
            this.equalsById = equalsById;
            
            this.formField = formField;
            this.getId = getId;
            this.construct = construct;
            this.toId = toId?? Converters.GetParser<TfID>();
        }

        public void AddViewDataMultiSelectList(Action<string, object> addViewData, IRepository<TEntity> repository)
        {
            var options = getOptions(repository);
            navigation.AddViewData(addViewData, options, new List<TfID>());
        }

        public void PrepareOptions(Action<string, object> addViewData, IRepository<TEntity> repository, out Action<TEntity> addViewDataMultiSelectList)
        {
            var options = getOptions(repository);
            addViewDataMultiSelectList = (entity) =>
                navigation.AddViewData(addViewData, options, getRelated(entity).Select(getTmmId));
        }

        public void ParseRelated(Action<string, object> addViewData, IRepository<TEntity> repository,  HttpRequest request, TEntity entity,  out Action<IBatch<TEntity>> modifyRelated, out Action addViewDataMultiSelectList)
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
            addViewDataMultiSelectList = () => navigation.AddViewData(addViewData, options, selectedIds);
        }
    }

    public class OneToMany<TP, TF, TfID> : IOneToMany<TP> where TP : class where TF : class 
    {
        private readonly MvcOneToManyNavigationFacade<TP, TF, TfID> navigation;
        private readonly Func<IRepository<TP>, IReadOnlyCollection<TF>> getOptions;
        private readonly string formField;
        private readonly Func<TP, TfID> getId;
        public OneToMany(
            string formFieldName,
            MvcOneToManyNavigationFacade<TP, TF, TfID> navigation,
            Func<IRepository<TP>, IReadOnlyCollection<TF>> getOptions,
            Func<TP, TfID> getId
            )
        {
            this.formField = formFieldName;
            this.navigation = navigation;
            this.getOptions = getOptions;
            this.getId = getId;
        }


        public void PrepareOptions(Action<string, object> addViewData, IRepository<TP> repository, out Action<TP> setViewDataMultiSelectLists)
        {
            var options = getOptions(repository);
            setViewDataMultiSelectLists = (entity) =>
                navigation.AddViewData(addViewData, options, getId(entity));
        }

        public void ParseRequest(Action<string, object> addViewData, HttpRequest request, TP entity, IRepository<TP> repository, out Action setViewDataSelectList)
        {
            var options = getOptions(repository);
            navigation.Parse(request, entity, options, formField);
            //modifyRelated = batch => batch.ModifyRelated(entity, getRelatedExpression, navigation.Selected, equalsById);
            setViewDataSelectList = () => navigation.AddViewData(addViewData, options);
        }

        public void SetViewDataSelectList(Action<string, object> addViewData, IRepository<TP> repository)
        {
            var options = getOptions(repository);
            SetViewDataSelectList(addViewData, options);
        }

        void SetViewDataSelectList(Action<string, object> addViewData, IReadOnlyCollection<TF> options)
        {
            navigation.AddViewData(addViewData, options);
        }
    }
}