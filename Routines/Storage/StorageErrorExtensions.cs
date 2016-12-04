using System;
using System.Collections.Generic;
using System.Linq;

namespace Vse.Routines.Storage
{
    public static class StorageErrorExtensions
    {
        public static void Desert(this StorageError storageError, string exceptionText = "StorageError contains exception")
        {
            if (storageError != null && storageError.FieldErrors != null)
            {
                if (storageError.FieldErrors.Count >= 1)
                    throw new ApplicationException(exceptionText, storageError.Exception);
            }
        }

        public static void Assert(this StorageError storageError, int number, string field, string messageFragment, string exceptionText)
        {
            if (storageError != null && storageError.FieldErrors != null)
            {
                if (storageError.FieldErrors.Count == number)
                {
                    if (messageFragment == null && storageError.Contains(field)) return;
                    if (messageFragment != null && storageError.ContainsLike(field, messageFragment)) return;
                }
            }
            throw new ApplicationException(exceptionText, storageError.Exception);
        }

        public static void Assert(this StorageError storageError, int number, string[] fields, string messageFragment, string exceptionText)
        {
            if (storageError != null && storageError.FieldErrors != null)
            {
                if (storageError.FieldErrors.Count == number)
                {
                    if (messageFragment == null && storageError.Contains(fields)) return;
                    if (messageFragment != null && storageError.ContainsLike(fields, messageFragment)) return;
                }
            }
            throw new ApplicationException(exceptionText, storageError.Exception);
        }
        public static bool Contains(this List<FieldError> fieldErrors, string field)
        {
            var @value = false;
            if (fieldErrors != null)
            {
                @value = fieldErrors.Any(e => e.Field == field);
            }
            return @value;
        }

        public static bool Contains(this StorageError storageError, string field)
        {
            var @value = false;
            if (storageError != null && storageError.FieldErrors != null)
            {
                @value = storageError.FieldErrors.Any(e => e.Field == field);
            }
            return @value;
        }

        public static bool Contains(this StorageError storageError, string[] fields)
        {
            var @value = false;
            if (storageError != null && storageError.FieldErrors != null)
            {
                foreach (var field in fields)
                {
                    bool success = storageError.FieldErrors.Any(e => e.Field == field);
                    if (!success)
                        break;
                }
                @value = true;
            }
            return @value;
        }
        public static bool ContainsLike(this StorageError storageError, string[] fields, string message)
        {
            var @value = false;
            if (storageError != null && storageError.FieldErrors != null)
            {
                foreach (var field in fields)
                {
                    bool success = storageError.FieldErrors.Any(e => e.Field == field && e.Message.StartsWith(message));
                    if (!success)
                        break;
                }
                @value = true;
            }
            return @value;
        }

        public static bool Contains(this StorageError storageError, string field, string message)
        {
            var @value = false;
            if (storageError != null && storageError.FieldErrors != null)
            {
                @value = storageError.FieldErrors.Any(e => e.Field == field && e.Message == message);
            }
            return @value;
        }
        public static bool ContainsLike(this StorageError storageError, string field, string message)
        {
            var @value = false;
            if (storageError != null && storageError.FieldErrors != null)
            {
                @value = storageError.FieldErrors.Any(e => e.Field == field && e.Message.StartsWith(message));
            }
            return @value;
        }
        public static int Count(this StorageError storageError)
        {
            var @value = 0;
            if (storageError != null && storageError.FieldErrors != null)
            {
                @value = storageError.FieldErrors.Count();
            }
            return @value;
        }
        public static void Add(this StorageError storageError, string field, string message)
        {
            storageError.FieldErrors.Add(new FieldError(field, message));
        }

        public static void Add(this List<FieldError> fieldErrors, string field, string message)
        {
            fieldErrors.Add(new FieldError(field, message));
        }

        public static Dictionary<string, string> ToDictionary(this List<FieldError> list, Func<string, string, string> concatent)
        {
            var dictionary = new Dictionary<string, string>();
            foreach (var item in list)
            {
                string value;
                if (dictionary.TryGetValue(item.Field, out value))
                    dictionary[item.Field] = concatent(value, item.Message);
                else
                    dictionary.Add(item.Field, item.Message);
            }
            return dictionary;
        }

        public static List<FieldError> AnalyzeException(this Exception exception, Action<Exception, List<FieldError>> parser = null)
        {
            var list = new List<FieldError>();
            do
            {
                parser?.Invoke(exception, list);
                exception = exception.InnerException;
            } while (exception != null);
            return list;
        }
    }
}
