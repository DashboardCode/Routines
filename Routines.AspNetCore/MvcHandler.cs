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
        public static void PublishResult(List<(string, List<string>)> message, Action<string, string> publishStorageError)
        {
            foreach (var messageItem in message)
                foreach (var errorText in messageItem.Item2)
                    publishStorageError(messageItem.Item1, errorText /*string.Join("; ", m.Item2.ToArray())*/);
        }

        public static void PublishResult(List<FieldMessage> message, Action<string, string> publishStorageError)
        {
            foreach (var errorField in message)
                publishStorageError(errorField.Field, errorField.Message);
        }

        public static IActionResult MakeActionResultOnSave<TEntity, TState>(
            IRepository<TEntity> repository,
            IOrmStorage<TEntity> storage,
            TState state,
            Func<IActionResult> unauthorized,
            HttpRequest request,
            Action<string, object> addViewData,
            Action<string, string> publishStorageError,
            Func<IActionResult> successView,
            Func<string, IActionResult> badRequestView,
            Func<TEntity, IActionResult> view,
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
                    IActionResult>
                > action
            ) where TEntity : class
        {
            Func<
                Func<bool>,
                Func<HttpRequest, IComplexBinderResult<TEntity>>,
                Func<HttpRequest, TEntity, Action<string, object>, IComplexBinderResult<ValueTuple<Action<IBatch<TEntity>>, Action>>>,
                Action<TEntity, IBatch<TEntity>>,
                IActionResult> steps = (authorize, getEntity, getRelated, save) =>
                {
                    if (!authorize())
                        return unauthorized();
                    var getEntityResult = getEntity(request);
                    if (!getEntityResult.IsOk())
                        PublishResult(getEntityResult.Message, publishStorageError);
                    var entity = getEntityResult.Value;
                    if (entity == null)
                        return badRequestView("Incorrect ID");
                    //bool isOk = getEntityResult.IsOk();
                    var getRelatedResult = getRelated(request, entity, addViewData);
                    if (!getRelatedResult.IsOk())
                        PublishResult(getRelatedResult.Message, publishStorageError);
                    var (modifyRelated, prepareViewData) = getRelatedResult.Value;
                    
                    if (getEntityResult.IsOk() && getRelatedResult.IsOk())
                    {
                        // TODO: open hidden layout Error dialog
                        //void publishException(Exception ex) { addViewData("Exception", ex); }
                        //try
                        //{
                            var storageResult = storage.Handle(
                                batch =>
                                {
                                    save(entity, batch);
                                    modifyRelated(batch);
                                });
                            if (storageResult.IsOk())
                                return successView();
                            //publishException(storageResult.Exception);
                            PublishResult(storageResult.Message, publishStorageError);
                        //}
                        //catch (Exception ex)
                        //{
                        //    publishException(ex);
                        //}
                    }
                    prepareViewData.Invoke();
                    return view(entity);
                };
            return action(repository,  state)(steps);
        }


        public static IActionResult MakeActionResultOnSave<TEntity, TState>(
            IRepository<TEntity> repository,
            IOrmStorage<TEntity> storage,
            TState state,
            Func<IActionResult> unauthorized,
            HttpRequest request,
            Action<string, object> addViewData,
            Action<string, string> publishStorageError,
            Func<IActionResult> successView,
            Func<string, IActionResult> badRequestView,
            Func<TEntity, IActionResult> view,
            Func<
                IRepository<TEntity>,
                TState,
                Func<
                    Func<
                        Func<bool>,
                        Func<HttpRequest, IComplexBinderResult<TEntity>>,
                        Action<TEntity, IBatch<TEntity>>,
                        IActionResult>,
                    IActionResult>
                > action
            ) where TEntity : class
        {
            Func<
                Func<bool>,
                Func<HttpRequest, IComplexBinderResult<TEntity>>,
                Action<TEntity, IBatch<TEntity>>,
                IActionResult> steps = (authorize, getEntity, save) =>
                {
                    if (!authorize())
                        return unauthorized();
                    var getEntityResult = getEntity(request);
                    if (!getEntityResult.IsOk())
                        PublishResult(getEntityResult.Message, publishStorageError);
                    var entity = getEntityResult.Value;
                    if (entity == null)
                        return badRequestView("Empty request");
                    if (getEntityResult.IsOk())
                    {
                        // TODO: open hidden layout Error dialog
                        //Action<Exception> publishException = (ex) => addViewData("Exception", ex);
                        //try
                        //{
                            var storageResult = storage.Handle(
                                batch => save(entity, batch)
                            );
                            if (storageResult.IsOk())
                                return successView();
                            //publishException(storageResult.Exception);
                            PublishResult(storageResult.Message, publishStorageError);
                        //}
                        //catch (Exception ex)
                        //{
                        //    publishException(ex);
                        //}
                    }
                    return view(entity);
                };
            return action(repository, state)(steps);
        }

        public static IActionResult MakeActionResultOnRequest<TKey, TEntity>(
          IRepository<TEntity> repository,
          Action<string, object> addViewData,
          Func<IActionResult> unauthorized,
          HttpRequest request,
          Func<TEntity, IActionResult> view,
          Func<string, IActionResult> badRequestView,
          Func<IActionResult> notFound,
          Func<
              IRepository<TEntity>,
              Func<
                  Func<
                      Func<bool>,
                      Func<string, ValuableResult<TKey>>, 
                      Func<TKey, TEntity>,
                      Action<TEntity, Action<string, object>>,
                      IActionResult>,
                  IActionResult>
              > action
          ) where TEntity: class
        {
            return action(repository)(
                (authorize, keyConverter, getEntity, publishViewData) =>
                {
                    if (!authorize())
                        return unauthorized();
                    var valuableResult = MvcHandler.BindId(request, keyConverter);
                    if (!valuableResult.IsOk())
                        return badRequestView("Incorrect ID");
                    var key = valuableResult.Value;
                    var entity = getEntity(key);
                    if (entity == null)
                        return notFound();
                    publishViewData?.Invoke(entity, addViewData);
                    return view(entity);
                }
            );
        }

        public static IActionResult MakeActionResultOnRequest<TKey, TEntity>(
          IRepository<TEntity> repository,
          Func<IActionResult> unauthorized,
          HttpRequest request,
          Func<TEntity, IActionResult> view,
          Func<string, IActionResult> badRequestView,
          Func<IActionResult> notFound,
          Func<
              IRepository<TEntity>,
              Func<
                  Func<
                      Func<bool>,
                      Func<string, ValuableResult<TKey>>,
                      Func<TKey, TEntity>,
                      IActionResult>,
                  IActionResult>
              > action
          ) where TEntity : class
        {
            return action(repository)(
                (authorize, keyConverter, getEntity) =>
                {
                    if (!authorize())
                        return unauthorized();
                    var valuableResult = MvcHandler.BindId(request, keyConverter);
                    if (!valuableResult.IsOk())
                        return badRequestView("Incorrect ID");
                    var key = valuableResult.Value;
                    var entity = getEntity(key);
                    if (entity == null)
                        return notFound();
                    return view(entity);
                }
            );
        }

        public static IActionResult MakeActionResultOnCreate<TEntity>(
            IRepository<TEntity> repository,
            Action<string, object> addViewData,
            Func<IActionResult> unauthorized,
            HttpRequest request,
            Func<TEntity, IActionResult> view,
            Func<
                IRepository<TEntity>,
                Func<
                    Func<
                        Func<bool>,
                        Func<TEntity>,
                        Action<TEntity, Action<string, object>>,
                        IActionResult>,
                    IActionResult>
                > action
        ) where TEntity : class
        {
            return action(repository)(
                (authorize, getEntity, publishViewData) =>
                {
                    if (!authorize())
                        return unauthorized();
                    var entity = getEntity();
                    publishViewData.Invoke(entity, addViewData);
                    return view(entity);
                }
            );
        }

        public static IActionResult MakeActionResultOnCreate<TEntity>(
            IRepository<TEntity> repository,
            Func<IActionResult> unauthorized,
            HttpRequest request,
            Func<TEntity, IActionResult> view,
            Func<
                IRepository<TEntity>,
                Func<
                    Func<
                        Func<bool>,
                        Func<TEntity>,
                        IActionResult>,
                    IActionResult>
                > action
        ) where TEntity : class
        {
            return action(repository)(
                (authorize, getEntity) =>
                {
                    if (!authorize())
                        return unauthorized();
                    var entity = getEntity();
                    return view(entity);
                }
            );
        }

        public static ValuableResult<T> BindId<T>(this HttpRequest request, Func<string, ValuableResult<T>> converter) //  Func<string, ValuableResult<TKey> >
        {

            // TODO: route to url like /User/5/
            if (request.Query.TryGetValue("id", out var stringValues))
            {
                if (stringValues.Count > 0)
                {
                    var idText = stringValues.ToString();
                    if (!string.IsNullOrEmpty(idText))
                           return converter(idText);
                }
            }

            //bool value = false;
            //PathString pathString = request.Path;
            //if (pathString.HasValue)
            //{
            //    var path = pathString.Value;
            //    var idText = path.Substring(path.LastIndexOf("/") + 1);
            //    if (value = !string.IsNullOrEmpty(idText))
            //        return converter(idText);
            //}
            return new ValuableResult<T>(default,false);
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