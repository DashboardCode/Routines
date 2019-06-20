using System;
using Microsoft.AspNetCore.Http;

namespace DashboardCode.Routines.AspNetCore
{
    public interface IManyToMany<TEntity, TDAL, TDST>: IManyToManyDisabled<TEntity, TDAL, TDST> where TEntity : class
    {
        void PrepareParsedOptionsOnUpdate(
            Action<string, object> addViewData,
            TDAL repository, 
            HttpRequest request, 
            TEntity entity, out Action<TDST> modifyRelated, out Action setViewDataMultiSelectList);

        void PrepareParsedOptionsOnInsert(
            Action<string, object> addViewData,
            TDAL repository,
            HttpRequest request,
            TEntity entity, out Action<TDST> modifyRelated, out Action setViewDataMultiSelectList);
    }

    public interface IManyToManyDisabled<TEntity, TDAL, TDST> where TEntity : class
    {
        void PrepareDefaultOptions(Action<string, object> addViewData,
            TDAL repository);

        void PreparePersistedOptions(Action<string, object> addViewData,
            TDAL repository, out Action<TEntity> setViewDataMultiSelectLists);
    }
}