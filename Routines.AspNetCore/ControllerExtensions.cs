using System;
using System.Text;
using System.Collections.Generic;
using Microsoft.Extensions.Primitives;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;

using DashboardCode.Routines.Json;

namespace DashboardCode.Routines.AspNetCore
{
    public static class ControllerExtensions
    {
        public static string ToLog(this HttpRequest request)
        {
            var sb = new StringBuilder();
            if (!request.HasFormContentType)
            {
                sb.Append(request.GetDisplayUrl());
            }
            else
            {
                var formCollection = request.Form;
                
                if (formCollection.Keys.Count > 0)
                {
                    sb.Append("{");
                    foreach (var key in formCollection.Keys)
                    {
                        var value = formCollection[key];
                        sb.Append(key).Append(":");
                        if (value.Count == 1)
                            sb.Append("\"").AppendJsonEscaped(value).Append("\"").Append(",");
                        else if (value.Count > 1)
                        {
                            sb.Append("[");
                            foreach (var v in value)
                                sb.Append("\"").AppendJsonEscaped(value).Append("\"").Append(",");
                            sb.Length--;
                            sb.Append("]");
                        }
                    }
                    sb.Length--;
                    sb.Append("}");
                }
            }
            var text = sb.ToString();
            return text;
        }

        public static T Bind<T>(this Controller controller, 
            Func<T> constructor, 
            Dictionary<string, Func<T, Func<StringValues, BinderResult>>> propertyBinders,
            Dictionary<string, Func<T, Action<StringValues>>> propertySetters=null)
        {
            var t = constructor();
            foreach (var pair in propertyBinders)
            {
                var propertyName = pair.Key;
                if (controller.HttpContext.Request.Form.TryGetValue(propertyName, out StringValues stringValues))
                {
                    Func<T, Func<StringValues, BinderResult>> func = pair.Value;
                    var result = func(t)(stringValues);
                    if (!result.IsSuccess())
                        foreach(var errorMessage in result.ErrorMessages)
                            controller.ModelState.AddModelError(propertyName, errorMessage);
                }
            }
            if (propertySetters != null)
            {
                foreach (var pair in propertySetters)
                {
                    var propertyName = pair.Key;
                    if (controller.HttpContext.Request.Form.TryGetValue(propertyName, out StringValues stringValues))
                    {
                        var action = pair.Value;
                        action(t)(stringValues);
                    }
                }
            }
            return t;
        }
    }
}