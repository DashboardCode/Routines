using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Http;

using DashboardCode.Routines.Storage;

namespace DashboardCode.Routines.AspNetCore
{
    public class ReferencesCollection<TEntity, TDAL> where TEntity : class 
    {
        Dictionary<string, IManyToMany<TEntity, TDAL>> ManyToManyBinders = new Dictionary<string, IManyToMany<TEntity, TDAL>>();
        Dictionary<string, IOneToMany<TEntity, TDAL>>  OneToManyBinders  = new Dictionary<string, IOneToMany<TEntity, TDAL>>();
        public ReferencesCollection(Dictionary<string, IOneToMany<TEntity, TDAL>> OneToManyBinders, Dictionary<string, IManyToMany<TEntity, TDAL>> ManyToManyBinders)
        {
            this.OneToManyBinders = OneToManyBinders;
            this.ManyToManyBinders = ManyToManyBinders;
        }

        public void PrepareEmptyOptions(Action<string, object> addViewData, TDAL repository)
        {
            foreach (var i in ManyToManyBinders)
            {
                i.Value.PrepareDefaultOptions(addViewData, repository);
            }
            foreach (var i in OneToManyBinders)
            {
                i.Value.PrepareDefaultOptions(addViewData, repository);
            }
        }

        public Action<TEntity> PrepareOptions(Action<string, object> addViewData, TDAL repository)
        {
            var tmp = new List<Action<TEntity>>();
            foreach (var i in ManyToManyBinders)
            {
                i.Value.PreparePersistedOptions(addViewData, repository, out Action<TEntity> setViewDataMultiSelectList);
                tmp.Add(setViewDataMultiSelectList);
            }

            return (entity) => tmp.ForEach(i => i(entity));
        }

        public IComplexBinderResult<ValueTuple<Action<IBatch<TEntity>>, Action>> ParseRelated(Action<string, object> addViewData, HttpRequest request, TDAL repository, TEntity entity)
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
            return new ComplexBinderResult<ValueTuple<Action<IBatch<TEntity>>, Action>>((modifyRelateds, setViewDataMultiSelectLists));
        }
    }
}