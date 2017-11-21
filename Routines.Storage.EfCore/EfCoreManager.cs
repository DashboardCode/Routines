using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using Microsoft.EntityFrameworkCore;

namespace DashboardCode.Routines.Storage.EfCore
{
    public static class EfCoreManager
    {
        public static void Analyze(Exception exception, List<FieldError> fieldsErrors, string entityName)
        {
            if (exception is DbUpdateConcurrencyException dbUpdateConcurrencyException)
                AnalyzeDbUpdateConcurrencyException(dbUpdateConcurrencyException, fieldsErrors);
            if (exception is InvalidOperationException invalidOperationException)
                AnalyzeInvalidOperationException(invalidOperationException, fieldsErrors, entityName);
        }

        static Regex fieldPkOrUniqueConstraintNullRegexV1 = new Regex("Unable to create or track an entity of type '(?<entity>.*?)' because it has a null primary or alternate key value.");
        static Regex fieldPkOrUniqueConstraintNullRegexV2 = new Regex("Unable to track an entity of type '(?<entity>.*?)' because alternate key property '(?<field>.*?)' is null.");
        public static void AnalyzeInvalidOperationException(InvalidOperationException ex, List<FieldError> fieldsErrors, string entityName)
        {
            {
                var matchCollectionV1 = fieldPkOrUniqueConstraintNullRegexV1.Matches(ex.Message);
                if (matchCollectionV1.Count > 0)
                {
                    var entity = matchCollectionV1[0].Groups["entity"].Value;
                    if (entityName == entity)
                        fieldsErrors.Add(StorageModel.GenericErrorField, "ID or alternate id has no value");
                    return;
                }
            }

            {
                var matchCollectionV2 = fieldPkOrUniqueConstraintNullRegexV2.Matches(ex.Message);
                if (matchCollectionV2.Count > 0)
                {
                    if (ex.Message.Contains("If the alternate key is not used in a relationship, then consider using a unique index instead."))
                    {
                        var entity = matchCollectionV2[0].Groups["entity"].Value;
                        var field = matchCollectionV2[0].Groups["field"].Value;
                        if (entityName == entity)
                            fieldsErrors.Add(field, "ID or alternate id has no value");
                    }
                    return;
                }
            }
        }

        public static void AnalyzeDbUpdateConcurrencyException(DbUpdateConcurrencyException exception, List<FieldError> fieldsErrors)
        {
            fieldsErrors.Add(StorageModel.GenericErrorField, "The record you are attempted to edit is currently being modified by another user. The save operation was canceled! Refresh page and reload form before continue.");
        }

        public static void Append(StringBuilder sb, Exception ex)
        {
            if (ex is DbUpdateConcurrencyException dbUpdateConcurrencyException)
                AppendDbUpdateConcurrencyException(sb, dbUpdateConcurrencyException);
        }

        public static void AppendDbUpdateConcurrencyException(StringBuilder stringBuilder, DbUpdateConcurrencyException ex)
        {
            stringBuilder.AppendMarkdownLine("DbUpdateConcurrencyException data: ");
            int i = 1;
            foreach (var e in ex.Entries)
            {
                stringBuilder.AppendMarkdownProperty(i.ToString(), e.Entity.GetType().FullName);
                foreach (var p in e.Properties)
                    stringBuilder.AppendMarkdownProperty("   " + p.Metadata.Name, (p.CurrentValue?.ToString()) ?? "(null)");
                i++;
            }
        }
    }
}