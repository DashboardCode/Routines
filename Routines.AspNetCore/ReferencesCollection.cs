using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Http;

using DashboardCode.Routines.Storage;

namespace DashboardCode.Routines.AspNetCore
{
    public class ReferencesCollection<TEntity> where TEntity : class
    {
        Dictionary<string, IManyToMany<TEntity>> ManyToManyBinders = new Dictionary<string, IManyToMany<TEntity>>();
        Dictionary<string, IOneToMany<TEntity>> OneToManyBinders = new Dictionary<string, IOneToMany<TEntity>>();
        public ReferencesCollection(Dictionary<string, IOneToMany<TEntity>> OneToManyBinders, Dictionary<string, IManyToMany<TEntity>> ManyToManyBinders)
        {
            this.OneToManyBinders = OneToManyBinders;
            this.ManyToManyBinders = ManyToManyBinders;
        }

        public void PrepareEmptyOptions(Action<string, object> addViewData, IRepository<TEntity> repository)
        {
            foreach (var i in ManyToManyBinders)
                i.Value.PrepareDefaultOptions(addViewData, repository);
            foreach (var i in OneToManyBinders)
                i.Value.PrepareDefaultOptions(addViewData, repository);
        }

        public Action<TEntity> PrepareOptions(Action<string, object> addViewData, IRepository<TEntity> repository)
        {
            var tmp = new List<Action<TEntity>>();
            foreach (var i in ManyToManyBinders)
            {
                i.Value.PreparePersistedOptions(addViewData, repository, out Action<TEntity> setViewDataMultiSelectList);
                tmp.Add(setViewDataMultiSelectList);
            }

            return (entity) => tmp.ForEach(i => i(entity));
        }

        public ValueTuple<Action<IBatch<TEntity>>, Action> ParseRelated(Action<string, object> addViewData, IRepository<TEntity> repository, HttpRequest request, TEntity entity)
        {
            List<Action<IBatch<TEntity>>> tmp0 = new List<Action<IBatch<TEntity>>>();
            List<Action> tmp1 = new List<Action>();
            foreach (var i in ManyToManyBinders)
            {
                i.Value.PrepareParsedOptions(addViewData, repository, request, entity, out Action<IBatch<TEntity>> modifyRelated, out Action setViewDataMultiSelectList);
                tmp0.Add(modifyRelated);
                tmp1.Add(setViewDataMultiSelectList);
            }
            Action<IBatch<TEntity>> modifyRelateds = batch => tmp0.ForEach(i => i(batch));
            Action setViewDataMultiSelectLists = () => tmp1.ForEach(i => i());
            return (modifyRelateds, setViewDataMultiSelectLists);
        }
    }

}
