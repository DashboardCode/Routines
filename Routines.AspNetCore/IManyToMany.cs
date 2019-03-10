﻿using System;

using DashboardCode.Routines.Storage;
using Microsoft.AspNetCore.Http;

namespace DashboardCode.Routines.AspNetCore
{
    public interface IManyToMany<TEntity, TDAL> where TEntity : class
    {
        void PrepareDefaultOptions(Action<string, object> addViewData,
            TDAL repository);
        void PreparePersistedOptions(Action<string, object> addViewData,
            TDAL repository, out Action<TEntity> setViewDataMultiSelectLists);
        void PrepareParsedOptions(
            Action<string, object> addViewData,
            TDAL repository, 
            HttpRequest request, 
            TEntity entity, out Action<IBatch<TEntity>> modifyRelated, out Action setViewDataMultiSelectList);
    }
}