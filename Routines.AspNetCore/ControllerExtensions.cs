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

        public static Func<T, string, string> IntBinder<T>(this Action<T, string> action)
        {
            return (t, value) =>
            {
                action(t, value);
                return null;
            };
        }

        public static Func<T, string, string> LenghtBinder<T>(this Action<T, string> action, int lenght)
        {
            return (t, value) =>
            {   if (value== null)
                action(t, value);
                return null;
            };
        }
    }

    public struct ConvertResult<T>
    {
        public T Value;
        public string ErrorMessage;
        public bool IsSuccess() { return ErrorMessage == null; }
    }

    public struct BinderResult
    {
        public List<string> ErrorMessages;
        public bool IsSuccess() { return ErrorMessages == null; }
    }

    public  class Binder
    {
        public static BinderResult TryString(StringValues stringValues, Action<string> setter)
        {
            return Try(sv => new ConvertResult<string>{Value = sv.ToString()}, v => setter(v))(stringValues);
        }


        public static BinderResult TryStringValidateLength(StringValues stringValues, Action<string> setter, int length)
        {
            return Try(sv => new ConvertResult<string>{Value = sv.ToString()}, setter, v => v.Length > length ? "Too long!" : null)(stringValues);
        }

        public static Func<StringValues, BinderResult> Try<TValue>(Func<StringValues, ConvertResult<TValue>> convert, Action<TValue> setter)
        {
            return Try(sv => convert(sv), v => { setter(v); return new BinderResult(); });
        }

        public static Func<StringValues, BinderResult> Try<TValue>(
            Func<StringValues, ConvertResult<TValue>> convert,
            Action<TValue> setter,
            Func<TValue, string> validate)
        {
            return Try(sv => convert(sv), v => { setter(v); var error = validate(v); return new BinderResult() { ErrorMessages = error!=null? new List<string> { error } : null }; });
        }

        public static Func<StringValues, BinderResult> Try<TValue>(
            Func<StringValues, ConvertResult<TValue>> convert,
            Func<TValue, BinderResult> setter)
        {
            return (stringValues) =>
            {
                var binderResult = new BinderResult();

                var convertResult = convert(stringValues);
                if (!convertResult.IsSuccess())
                {
                    binderResult.ErrorMessages = new List<string>() { convertResult.ErrorMessage };
                }
                else
                {
                    var setterResult = setter(convertResult.Value);
                    if (!setterResult.IsSuccess())
                        binderResult = setterResult;
                }
                return binderResult;
            };
        }
    }
}