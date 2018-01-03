using System;
using System.Net;
using Microsoft.AspNetCore.Mvc;
using DashboardCode.Routines.Storage;

namespace DashboardCode.Routines.AspNetCore
{
    public static class MvcHandler
    {
        public static IActionResult MakeActionResultOnEntitySave(
            bool isValid, Func<StorageError> func, Func<IActionResult> error, Func<IActionResult> success,
            Action<Exception> publishException, Action<string, string> publishStorageError)
        {
            if (!isValid)
                return error();
            try
            {
                var storageError = func();
                if (storageError != null && storageError.FieldErrors.Count > 0)
                {
                    var dictionary = storageError.FieldErrors.ToDictionary((v1, v2) => v1 + ";" + Environment.NewLine + v2);
                    foreach (var i in dictionary)
                        publishStorageError(i.Key, i.Value);
                    publishException(storageError.Exception);
                    return error();
                }
                return success();
            }
            catch (Exception ex)
            {
                publishException(ex);
            }
            return error();
        }

        public static IActionResult MakeActionResultOnEntitySave(Action<string, object> addViewData, Action<string,string> addModelError, bool isValid, Func<StorageError> func, Func<IActionResult> error, Func<IActionResult> success) =>
            MakeActionResultOnEntitySave(isValid, func, error, success,
                (ex) => addViewData("Exception", ex),
                addModelError
                );
        

        public static IActionResult MakeActionResultOnEntityRequest<TEntity>(
          Func<object, IActionResult> view, Func<IActionResult> notFound, bool isValid, Func<TEntity> getEntity, Action<TEntity> prepareRendering = null)
        {
            if (!isValid)
                return new StatusCodeResult((int)HttpStatusCode.BadRequest);
            var entity = getEntity();
            if (entity == null)
                return notFound();
            prepareRendering?.Invoke(entity);
            return view(entity);
        }
    }
}