using System;
using System.Collections.Generic;

using DashboardCode.Routines.Storage;

namespace DashboardCode.Routines.AspNetCore
{
    public class ReferencesMeta<TEntity> where TEntity : class
    {
        readonly Dictionary<string, IManyToMany<TEntity>> manyToManyDictionary;

        public ReferencesMeta(Dictionary<string, IManyToMany<TEntity>> manyToManyDictionary)
        {
            this.manyToManyDictionary = manyToManyDictionary;
        }

        public void SetViewDataMultiSelectLists(RoutineController controller, IRepository<TEntity> repository)
        {
            foreach (var i in manyToManyDictionary)
            {
                i.Value.SetViewDataMultiSelectList(controller, repository);
            }
        }

        public void ParseRequests(RoutineController controller, TEntity entity, IRepository<TEntity> repository, out Action<IBatch<TEntity>> modifyRelateds, out Action setViewDataMultiSelectLists)
        {
            List<Action<IBatch<TEntity>>> tmp0 = new List<Action<IBatch<TEntity>>>();
            List<Action> tmp1 = new List<Action>();
            foreach (var i in manyToManyDictionary)
            {
                i.Value.ParseRequest(controller, entity, repository, out Action<IBatch<TEntity>> modifyRelated, out Action setViewDataMultiSelectList);
                tmp0.Add(modifyRelated);
                tmp1.Add(setViewDataMultiSelectList);
            }
            modifyRelateds = batch => {
                foreach (var i in tmp0)
                    i.Invoke(batch);
            };
            setViewDataMultiSelectLists = () => {
                foreach (var i in tmp1)
                    i.Invoke();
            };
        }

        public void PrepareOptions(RoutineController controller, IRepository<TEntity> repository, out Action<TEntity> setViewDataMultiSelectLists)
        {
            List<Action<TEntity>> tmp1 = new List<Action<TEntity>>();
            foreach (var i in manyToManyDictionary)
            {
                i.Value.PrepareOptions(controller, repository, out Action<TEntity> setViewDataMultiSelectList);
                tmp1.Add(setViewDataMultiSelectList);
            }
            setViewDataMultiSelectLists = (entity) => {
                foreach (var i in tmp1)
                    i.Invoke(entity);
            };
        }
    }
}