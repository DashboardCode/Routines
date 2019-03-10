using System;

using DashboardCode.Routines.Storage;
using Microsoft.AspNetCore.Http;

namespace DashboardCode.Routines.AspNetCore
{
    public interface IOneToMany<TEntity, TDAL> where TEntity : class
    {
        void PrepareDefaultOptions(Action<string, object> addViewData, TDAL repository);
        void PreparePersistedOptions(Action<string, object> addViewData, TDAL repository, out Action<TEntity> setViewDataMultiSelectLists);
        void PrepareParsedOptions(Action<string, object> addViewData, HttpRequest request, TEntity entity, TDAL repository, out Action setViewDataMultiSelectList);
    }
}