using System;
using System.Net;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Primitives;

using DashboardCode.Routines.Storage;

namespace DashboardCode.Routines.AspNetCore
{
    public static class MvcHandler
    {
        public static IActionResult MakeActionResultOnEntitySave3<TEntity, TState>(
            IRepository<TEntity> repository,
            IOrmStorage<TEntity> storage,
            TState state,
            Func<IActionResult> unauthorized,
            HttpRequest request,
            Action<string, object> addViewData,
            Action<string, string> publishStorageError,
            Func<IActionResult> success,
            Func<object, IActionResult> view,
            //Action<Exception> publishException,
            Func<
                IRepository<TEntity>, 
                TState,
                Func<
                    Func<
                         Func<bool>, 
                         Func<HttpRequest, IComplexBinderResult<TEntity>>,
                         Func<HttpRequest, TEntity, Action<string, object>, IComplexBinderResult<ValueTuple<Action<IBatch<TEntity>>, Action>>>,
                         Action<TEntity, IBatch<TEntity>>,
                         IActionResult>,
                    IActionResult
                    >> action
            ) where TEntity : class
        {
            Func<Func<bool>,
                     Func<HttpRequest, IComplexBinderResult<TEntity>>,
                     Func<HttpRequest, TEntity, Action<string, object>, IComplexBinderResult<ValueTuple<Action<IBatch<TEntity>>, Action>>>,
                     Action<TEntity, IBatch<TEntity>>,
                     IActionResult> steps2 = (authorize, getEntity, getRelated, save) =>
                      {
                          if (!authorize())
                              return unauthorized();
                          var res = getEntity(request);
                          var entity = res.Value;
                          if (!res.IsOk())
                          {
                              foreach (var m in res.Message)
                                  publishStorageError(m.Item1, string.Join("; ", m.Item2.ToArray()));
                          }
                          IComplexBinderResult<ValueTuple<Action<IBatch<TEntity>>, Action>> res2;
                          if (getRelated != null)
                          {
                              res2 = getRelated(request, entity, addViewData);
                              foreach (var m in res2.Message)
                                  publishStorageError(m.Item1, string.Join("; ", m.Item2.ToArray()));
                          }
                          else
                          {
                              res2 = new ComplexBinderResult<ValueTuple<Action<IBatch<TEntity>>, Action>>();
                          }
                          var (modifyRelated, prepareViewData) = res2.Value;
                          if (res.IsOk() && res2.IsOk())
                          {
                              Action<Exception> publishException = (ex) => addViewData("Exception", ex);
                              try
                              {
                                  var storageError = storage.Handle(
                                  batch =>
                                  {
                                      save(entity, batch);
                                      modifyRelated?.Invoke(batch);
                                  });
                                  if (storageError != null && storageError.FieldErrors.Count > 0)
                                  {
                                      var dictionary = storageError.FieldErrors.ToDictionary((v1, v2) => v1 + ";" + Environment.NewLine + v2);
                                      foreach (var i in dictionary)
                                          publishStorageError(i.Key, i.Value);
                                      publishException(storageError.Exception);
                                      prepareViewData?.Invoke();
                                      return view(entity);
                                  }
                                  return success();
                              }
                              catch (Exception ex)
                              {
                                  prepareViewData?.Invoke();
                                  publishException(ex);
                              }
                          }
                          return view(entity);
                      };
            return action(repository,  state)(steps2);
        }

       
        public static IActionResult MakeActionResultOnEntityRequest<TEntity, TKey>(
          Func<string, ValuableResult<TKey>> keyConverter,
          HttpRequest request,
          Func<object, IActionResult> view, 
          Func<IActionResult> notFound, 
          Func<TKey,TEntity> getEntity, 
          Action<TEntity> prepareRendering = null)
        {
            var (key, isValid) = request.BindId(keyConverter);
            if (!isValid)
                return new StatusCodeResult((int)HttpStatusCode.BadRequest);
            var entity = getEntity(key.Value);
            if (entity == null)
                return notFound();
            prepareRendering?.Invoke(entity);
            return view(entity);
        }

        public static (T, bool) BindId<T>(this HttpRequest request, Func<string, T> converter)
        {
            bool value = false;
            T id = default(T);
            PathString pathString = request.Path;
            if (pathString.HasValue)
            {
                var path = pathString.Value;
                var idText = path.Substring(path.LastIndexOf("/") + 1);
                if (value = !string.IsNullOrEmpty(idText))
                    id = converter(idText);
            }
            return (id, value);
        }

        public static IComplexBinderResult<T> Bind<T>(
            this HttpRequest request,
            Func<T> constructor,
            Dictionary<string, Func<T, Func<StringValues, IVerboseResult<List<string>>>>> propertyBinders,
            Dictionary<string, Func<T, Action<StringValues>>> propertySetters)
        {
            var value = constructor();
            var messages = new List<ValueTuple<string, List<string>>>();
            foreach (var pair in propertyBinders)
            {
                var propertyName = pair.Key;
                if (request.Form.TryGetValue(propertyName, out StringValues stringValues))
                {
                    Func<T, Func<StringValues, IVerboseResult<List<string>>>> func = pair.Value;
                    var result = func(value)(stringValues);
                    if (!result.IsOk())
                        messages.Add((propertyName, result.Message));
                }
            }
            if (propertySetters != null)
            {
                foreach (var pair in propertySetters)
                {
                    var propertyName = pair.Key;
                    if (request.Form.TryGetValue(propertyName, out StringValues stringValues))
                    {
                        var action = pair.Value;
                        action(value)(stringValues);
                    }
                }
            }
            return new ComplexBinderResult<T>(value, messages); 
        }
    }
}