using System;
using System.Linq;
using System.Collections.Generic;

namespace DashboardCode.Routines.Storage
{
    public static class StorageResultExtensions
    {
        public static void ThrowIfFailed(this StorageResult storageResult, string exceptionText = "Storage returns exception")
        {
            if (storageResult.Exception!=null)
            {
                throw new StorageResultException(exceptionText, storageResult);
            }
        }

        public static void ThrowIfFailed(this StorageResult storageResult, Func<StorageResult, string> exceptionText)
        {
            if (storageResult.Exception != null)
            {
                throw new StorageResultException(exceptionText(storageResult), storageResult);
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
            throw new InvalidOperationException(exceptionText);
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
            throw new InvalidOperationException(exceptionText);
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
                @value = storageResult.Message.Count();
            return @value;
        }

        public static void Add(this List<FieldMessage> fieldErrors, string field, string message)
        {
            fieldErrors.Add(new FieldMessage(field, message));
        }

        //public static StorageResult AnalyzeExceptionRecursive(Exception exception, IOrmEntitySchemaAdapter relationalEntitySchemaAdapter, string genericErrorField, Action<Exception, IStorageResultBuilder> parser = null)
        //{
        //    return StorageResultExtensions.AnalyzeExceptionRecursive(
        //        exception,
        //        (recursiveParse) =>
        //        {

        //            var errorBuilder = new StorageResultBuilder(exception, relationalEntitySchemaAdapter, genericErrorField);
        //            recursiveParse(errorBuilder);
        //            return errorBuilder.Build();
        //        },
        //        parser
        //    );
        //}

        //public static StorageResult AnalyzeExceptionRecursive2(
        //    Exception exception,
        //    IOrmEntitySchemaAdapter relationalEntitySchemaAdapter, 
        //    string genericErrorField,
        //    Func<Action<IStorageResultBuilder>, StorageResult> analyzeException, Action<Exception, IStorageResultBuilder> parser = null)
        //{
        //    var storageResultBuilder = new StorageResultBuilder(exception, relationalEntitySchemaAdapter, genericErrorField);

        //    var ex = exception;
        //    do
        //    {
        //        parser?.Invoke(ex, storageResultBuilder);
        //        ex = ex.InnerException;
        //    } while (ex != null);

        //    return storageResultBuilder.Build();
        //}

        public static StorageResult AnalyzeExceptionRecursive(Exception exception, Func<Action<IStorageResultBuilder>, StorageResult> analyzeException,  Action<Exception, IStorageResultBuilder> parser = null)
        {
            return analyzeException(
                    (storageResultBuilder) =>
                    {
                        var ex = exception;
                        do
                        {
                            parser?.Invoke(ex, storageResultBuilder);
                            ex = ex.InnerException;
                        } while (ex != null);
                    }
                );
        }
    }


    //public interface IStorageResultBuilderEntityFactory
    //{
    //    IStorageResultBuilder CreateStorageResultBuilder(Exception exception);
    //}

    //public class StorageResultBuilderEntityFactory<TEntity> : IStorageResultBuilderEntityFactory where TEntity : class
    //{
    //    IOrmEntitySchemaAdapter<TEntity> ormEntitySchemaAdapter;
    //    string genericErrorFieldName;
    //    public StorageResultBuilderEntityFactory(IOrmEntitySchemaAdapter<TEntity> ormEntitySchemaAdapter, string genericErrorFieldName)
    //    {
    //        this.ormEntitySchemaAdapter = ormEntitySchemaAdapter;
    //        this.genericErrorFieldName = genericErrorFieldName;
    //    }

    //    public IStorageResultBuilder CreateStorageResultBuilder(Exception exception)
    //    {
    //        var storageResultBuilder = new StorageResultBuilder(exception, ormEntitySchemaAdapter, genericErrorFieldName);
    //        return storageResultBuilder;
    //    }
    //}
}
