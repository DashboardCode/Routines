using System;

using DashboardCode.Routines.Storage;

namespace DashboardCode.Routines.AspNetCore
{
    public interface IManyToMany<TEntity> where TEntity : class
    {
        void SetViewDataMultiSelectList(RoutineController controller, IRepository<TEntity> repository);
        void PrepareOptions(RoutineController controller, IRepository<TEntity> repository, out Action<TEntity> setViewDataMultiSelectLists);

        void ParseRequest(RoutineController controller, TEntity entity, IRepository<TEntity> repository, out Action<IBatch<TEntity>> modifyRelated, out Action setViewDataMultiSelectList);
    }
}