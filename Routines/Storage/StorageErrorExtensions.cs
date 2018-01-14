using System;
using System.Linq;
using System.Collections.Generic;

namespace DashboardCode.Routines.Storage
{
    public static class StorageResultExtensions
    {
        public static void ThrowIfNotNull(this StorageResult storageResult, string exceptionText = "StorageError contains exception")
        {
            if (!storageResult.IsOk())
            {
                if (storageResult.Message.Count >= 1)
                    throw new StorageErrorException(exceptionText, storageResult.Exception);
            }
        }

        public static void Assert(this StorageResult storageResult, int number, string field, string messageFragment, string exceptionText)
        {
            if (!storageResult.IsOk())
            {
                if (storageResult.Message.Count == number)
                {
                    if (messageFragment == null && storageResult.Contains(field)) return;
                    if (messageFragment != null && storageResult.ContainsLike(field, messageFragment)) return;
                }
            }
            throw new StorageErrorException(exceptionText, storageResult.Exception);
        }

        public static void Assert(this StorageResult storageResult, int number, string[] fields, string messageFragment, string exceptionText)
        {
            if (!storageResult.IsOk())
            {
                if (storageResult.Message.Count == number)
                {
                    if (messageFragment == null && storageResult.Contains(fields)) return;
                    if (messageFragment != null && storageResult.ContainsLike(fields, messageFragment)) return;
                }
            }
            throw new StorageErrorException(exceptionText, storageResult.Exception);
        }
        public static bool Contains(this List<FieldMessage> fieldErrors, string field)
        {
            var @value = false;
            if (fieldErrors != null)
            {
                @value = fieldErrors.Any(e => e.Field == field);
            }
            return @value;
        }

        public static bool Contains(this StorageResult storageResult, string field)
        {
            var @value = false;
            if (!storageResult.IsOk())
            {
                @value = storageResult.Message.Any(e => e.Field == field);
            }
            return @value;
        }

        public static bool Contains(this StorageResult storageResult, string[] fields)
        {
            var @value = false;
            if (!storageResult.IsOk())
            {
                foreach (var field in fields)
                {
                    bool success = storageResult.Message.Any(e => e.Field == field);
                    if (!success)
                        break;
                }
                @value = true;
            }
            return @value;
        }
        public static bool ContainsLike(this StorageResult storageResult, string[] fields, string message)
        {
            var @value = false;
            if (!storageResult.IsOk())
            {
                foreach (var field in fields)
                {
                    bool success = storageResult.Message.Any(e => e.Field == field && e.Message.StartsWith(message));
                    if (!success)
                        break;
                }
                @value = true;
            }
            return @value;
        }

        public static bool Contains(this StorageResult storageResult, string field, string message)
        {
            var @value = false;
            if (!storageResult.IsOk())
            {
                @value = storageResult.Message.Any(e => e.Field == field && e.Message == message);
            }
            return @value;
        }
        public static bool ContainsLike(this StorageResult storageResult, string field, string message)
        {
            var @value = false;
            if (!storageResult.IsOk())
            {
                @value = storageResult.Message.Any(e => e.Field == field && e.Message.StartsWith(message));
            }
            return @value;
        }
        public static int Count(this StorageResult storageResult)
        {
            var @value = 0;
            if (!storageResult.IsOk())
            {
                @value = storageResult.Message.Count();
            }
            return @value;
        }
        public static void Add(this StorageResult storageResult, string field, string message)
        {
            storageResult.Message.Add(new FieldMessage(field, message));
        }

        public static void Add(this List<FieldMessage> fieldErrors, string field, string message)
        {
            fieldErrors.Add(new FieldMessage(field, message));
        }

        public static Dictionary<string, string> ToDictionary(this List<FieldMessage> list, Func<string, string, string> concatent)
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

        public static List<FieldMessage> AnalyzeException(this Exception exception, Action<Exception, List<FieldMessage>> parser = null)
        {
            var list = new List<FieldMessage>();
            do
            {
                parser?.Invoke(exception, list);
                exception = exception.InnerException;
            } while (exception != null);
            return list;
        }
    }
}
