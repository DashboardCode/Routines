using System;

using DashboardCode.Routines.Storage;
using Microsoft.AspNetCore.Http;

namespace DashboardCode.Routines.AspNetCore
{
    public interface IManyToMany<TEntity> where TEntity : class
    {
        void AddViewDataMultiSelectList(Action<string, object> addViewData, IRepository<TEntity> repository);
        void PrepareOptions(Action<string, object> addViewData, IRepository<TEntity> repository, out Action<TEntity> setViewDataMultiSelectLists);

        void ParseRelated(Action<string, object> addViewData, IRepository<TEntity> repository, HttpRequest request, TEntity entity, out Action<IBatch<TEntity>> modifyRelated, out Action setViewDataMultiSelectList);
    }

    public interface IOneToMany<TEntity> where TEntity : class
    {
        void SetViewDataSelectList(Action<string, object> addViewData, IRepository<TEntity> repository);
        void PrepareOptions(Action<string, object> addViewData, IRepository<TEntity> repository, out Action<TEntity> setViewDataMultiSelectLists);
        void ParseRequest(Action<string, object> addViewData, HttpRequest request, TEntity entity, IRepository<TEntity> repository, out Action setViewDataMultiSelectList);
    }
}