using System;
using System.Collections.Generic;

using DashboardCode.Routines.Storage;

namespace DashboardCode.Routines.AspNetCore
{
    public class ReferencesMeta<T> where T : class
    {
        readonly IManyToMany<T>[] manyToManyCollection;

        public ReferencesMeta(IManyToMany<T>[] manyToManyCollection)
        {
            this.manyToManyCollection = manyToManyCollection;
        }

        public void SetViewDataMultiSelectLists(RoutineController controller, IRepository<T> repository)
        {
            foreach (var i in manyToManyCollection)
                i.SetViewDataMultiSelectList(controller, repository);
        }

        public void ParseRequests(RoutineController controller, T entity, IRepository<T> repository, out Action<IBatch<T>> modifyRelateds, out Action setViewDataMultiSelectLists)
        {
            List<Action<IBatch<T>>> tmp0 = new List<Action<IBatch<T>>>();
            List<Action> tmp1 = new List<Action>();
            foreach (var i in manyToManyCollection)
            {
                i.ParseRequest(controller, entity, repository, out Action<IBatch<T>> modifyRelated, out Action setViewDataMultiSelectList);
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

        public void PrepareOptions(RoutineController controller, IRepository<T> repository, out Action<T> setViewDataMultiSelectLists)
        {
            List<Action<T>> tmp1 = new List<Action<T>>();
            foreach (var i in manyToManyCollection)
            {
                i.PrepareOptions(controller, repository, out Action<T> setViewDataMultiSelectList);
                tmp1.Add(setViewDataMultiSelectList);
            }
            setViewDataMultiSelectLists = (entity) => {
                foreach (var i in tmp1)
                    i.Invoke(entity);
            };
        }
    }
}