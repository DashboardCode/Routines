using System;

using DashboardCode.Routines.Storage;
using Microsoft.AspNetCore.Http;

namespace DashboardCode.Routines.AspNetCore
{
    public interface IManyToMany<TEntity> where TEntity : class
    {
        void PrepareDefaultOptions(Action<string, object> addViewData, IRepository<TEntity> repository);
        void PreparePersistedOptions(Action<string, object> addViewData, IRepository<TEntity> repository, out Action<TEntity> setViewDataMultiSelectLists);
        void PrepareParsedOptions(Action<string, object> addViewData, IRepository<TEntity> repository, HttpRequest request, TEntity entity, out Action<IBatch<TEntity>> modifyRelated, out Action setViewDataMultiSelectList);
    }

    public interface IOneToMany<TEntity> where TEntity : class
    {
        void PrepareDefaultOptions(Action<string, object> addViewData, IRepository<TEntity> repository);
        void PreparePersistedOptions(Action<string, object> addViewData, IRepository<TEntity> repository, out Action<TEntity> setViewDataMultiSelectLists);
        void PrepareParsedOptions(Action<string, object> addViewData, HttpRequest request, TEntity entity, IRepository<TEntity> repository, out Action setViewDataMultiSelectList);
    }
}