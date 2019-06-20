using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Http;

namespace DashboardCode.Routines.AspNetCore
{
    public class ReferencesCollection<TEntity, TDAL, TDST> where TEntity : class 
    {
        readonly Dictionary<string, IManyToMany<TEntity, TDAL, TDST>> ManyToManyBinders = new Dictionary<string, IManyToMany<TEntity, TDAL, TDST>>();
        readonly Dictionary<string, IManyToManyDisabled<TEntity, TDAL, TDST>> ManyToManyDisabledBinders = new Dictionary<string, IManyToManyDisabled<TEntity, TDAL, TDST>>();
        readonly Dictionary<string, IOneToMany<TEntity, TDAL>> OneToManyBinders = new Dictionary<string, IOneToMany<TEntity, TDAL>>();
        public ReferencesCollection(
            Dictionary<string, IOneToMany<TEntity, TDAL>> OneToManyBinders, 
            Dictionary<string, IManyToMany<TEntity, TDAL, TDST>> ManyToManyBinders,
            Dictionary<string, IManyToManyDisabled<TEntity, TDAL, TDST>> ManyToManyDisabledBinders)
        {
            this.OneToManyBinders = OneToManyBinders;
            this.ManyToManyBinders = ManyToManyBinders;
            this.ManyToManyDisabledBinders = ManyToManyDisabledBinders;
        }

        public void PrepareEmptyOptions(Action<string, object> addViewData, TDAL repository)
        {
            foreach (var i in ManyToManyBinders)
            {
                i.Value.PrepareDefaultOptions(addViewData, repository);
            }
            foreach (var i in ManyToManyDisabledBinders)
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
            foreach (var i in ManyToManyDisabledBinders)
            {
                i.Value.PreparePersistedOptions(addViewData, repository, out Action<TEntity> setViewDataMultiSelectList);
                tmp.Add(setViewDataMultiSelectList);
            }
            foreach (var i in OneToManyBinders)
            {
                i.Value.PreparePersistedOptions(addViewData, repository, out Action<TEntity> setViewDataSelectList);
                tmp.Add(setViewDataSelectList);
            }

            return (entity) => tmp.ForEach(i => i(entity));
        }

        public IComplexBinderResult<ValueTuple<Action<TDST>, Action>> ParseRelatedOnUpdate(Action<string, object> addViewData, HttpRequest request,
            TDAL repository, TEntity entity)
        {
            List<Action<TDST>> modifyRelatedsTmp = new List<Action<TDST>>();
            List<Action> setViewDataMultiSelectListsTmp = new List<Action>();

            foreach (var i in ManyToManyDisabledBinders)
            {
                i.Value.PreparePersistedOptions(addViewData, repository, out Action<TEntity> setViewDataMultiSelectList);
                setViewDataMultiSelectListsTmp.Add(()=>setViewDataMultiSelectList(entity));
            }
            foreach (var i in ManyToManyBinders)
            {
                    i.Value.PrepareParsedOptionsOnUpdate(addViewData, repository, request, entity,
                        out Action<TDST> modifyRelated,
                        out Action setViewDataMultiSelectList);
                    modifyRelatedsTmp.Add(modifyRelated);
                    setViewDataMultiSelectListsTmp.Add(setViewDataMultiSelectList);
            }
            Action<TDST> modifyRelatedInLoop = batch => modifyRelatedsTmp.ForEach(i => i(batch));
            Action setViewDataMultiSelectLists = () => setViewDataMultiSelectListsTmp.ForEach(i => i());
            return new ComplexBinderResult<ValueTuple<Action<TDST>, Action>>((modifyRelatedInLoop, setViewDataMultiSelectLists));
        }

        public IComplexBinderResult<ValueTuple<Action<TDST>, Action>> ParseRelatedOnInsert(Action<string, object> addViewData, HttpRequest request,
            TDAL repository, TEntity entity)
        {
            List<Action<TDST>> modifyRelatedsTmp = new List<Action<TDST>>();
            List<Action> setViewDataMultiSelectListsTmp = new List<Action>();
            foreach (var i in ManyToManyBinders)
            {
                i.Value.PrepareParsedOptionsOnInsert(addViewData, repository, request, entity,
                    out Action<TDST> modifyRelated,
                    out Action setViewDataMultiSelectList);
                modifyRelatedsTmp.Add(modifyRelated);
                setViewDataMultiSelectListsTmp.Add(setViewDataMultiSelectList);
            }
            Action < TDST > modifyRelatedInLoop = batch => modifyRelatedsTmp.ForEach(i => i(batch));
            Action setViewDataMultiSelectLists = () => setViewDataMultiSelectListsTmp.ForEach(i => i());
            return new ComplexBinderResult<ValueTuple<Action<TDST>, Action>>((modifyRelatedInLoop, setViewDataMultiSelectLists));
        }
    }
}