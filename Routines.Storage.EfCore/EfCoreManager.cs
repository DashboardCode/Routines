using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using System.Text.RegularExpressions;
using System.Text;

namespace Vse.Routines.Storage.EfCore
{
    public static class EfCoreManager
    {
        public static void Analyze(Exception exception, List<FieldError> fieldsErrors, StorageModel storageModel)
        {
            if (exception is DbUpdateConcurrencyException)
            {
                AnalyzeDbUpdateConcurrencyException((DbUpdateConcurrencyException)exception, fieldsErrors);
            }
            if (exception is InvalidOperationException)
            {
                AnalyzeInvalidOperationException((InvalidOperationException)exception, fieldsErrors, storageModel);
            }
        }

        static Regex fieldPkOrUniqueConstraintNullRegex = new Regex("Unable to create or track an entity of type '(?<entity>.*?)' because it has a null primary or alternate key value.");
        public static void AnalyzeInvalidOperationException(InvalidOperationException ex, List<FieldError> fieldsErrors, StorageModel storageModel)
        {
            var matchCollection = fieldPkOrUniqueConstraintNullRegex.Matches(ex.Message);
            if (matchCollection.Count > 0)
            {
                var entity = matchCollection[0].Groups["entity"].Value;
                if (storageModel.Entity.Name == entity)
                {
                    fieldsErrors.Add(StorageModel.GenericErrorField, "ID or alternate id has no value");
                }
                return;
            }
        }

        public static void AnalyzeDbUpdateConcurrencyException(DbUpdateConcurrencyException exception, List<FieldError> fieldsErrors)
        {
            if (exception is DbUpdateConcurrencyException)
            {
                fieldsErrors.Add(StorageModel.GenericErrorField, "The record you are attempted to edit is currently being modified by another user. The save operation was canceled! Refresh page and reload form before continue.");
            }
        }

        public static void Append(StringBuilder sb, Exception ex)
        {
            if (ex is DbUpdateConcurrencyException)
            {
                AppendDbUpdateConcurrencyException(sb, (DbUpdateConcurrencyException)ex);
            }
        }

        public static void AppendDbUpdateConcurrencyException(StringBuilder stringBuilder, DbUpdateConcurrencyException ex)
        {
            if (ex is DbUpdateConcurrencyException)
            {
                stringBuilder.AppendMarkdownLine("DbUpdateConcurrencyException data: ");
                int i = 1;
                foreach (var e in ex.Entries)
                {
                    stringBuilder.AppendMarkdownProperty(i.ToString(), e.Entity.GetType().FullName);
                    foreach (var p in e.Properties)
                    {
                        stringBuilder.AppendMarkdownProperty("   "+p.Metadata.Name, (p.CurrentValue?.ToString())??"(null)");
                    }
                    i++;
                }
            }
        }
    }
}
