using System;
using System.Net;
using Microsoft.AspNetCore.Mvc;
using DashboardCode.Routines.Storage;

namespace DashboardCode.Routines.AspNetCore
{
    public static class MvcHandler
    {
        public static IActionResult MakeActionResultOnSave(this Controller controller, Func<StorageError> func, Func<IActionResult> error, Func<IActionResult> success = null)
        {
            return MakeActionResultOnSave(controller, controller.ModelState.IsValid, func, error);
        }

        public static IActionResult MakeActionResultOnSave(this Controller controller, bool isValid, Func<StorageError> func, Func<IActionResult> error, Func<IActionResult> success = null)
        {
            if (!isValid)
                return error();
            if (success == null)
                success = () => controller.RedirectToAction("Index");
            try
            {
                var storageError = func();
                if (storageError != null && storageError.FieldErrors.Count > 0)
                {
                    var dictionary = storageError.FieldErrors.ToDictionary((v1, v2) => v1 + ";" + Environment.NewLine + v2);
                    foreach (var i in dictionary)
                        controller.ModelState.AddModelError(i.Key, i.Value);
                    controller.ViewBag.Exception = storageError.Exception;
                    return error();
                }
                return success();
            }
            catch (Exception ex)
            {
                controller.ViewBag.Exception = ex;
            }
            return error();
        }

        public static IActionResult MakeActionResultOnRequest<TEntity>(this Controller controller, Func<bool> isValidInput, Func<TEntity> getEntity, Action<TEntity> prepareRendering = null)
        {
            if (!isValidInput())
                return new StatusCodeResult((int)HttpStatusCode.BadRequest);
            var entity = getEntity();
            if (entity == null)
                return controller.NotFound();
            prepareRendering?.Invoke(entity);
            return controller.View(entity);
        }
    }
}