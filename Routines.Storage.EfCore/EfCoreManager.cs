using System;
using System.Text;
using System.Text.RegularExpressions;
using Microsoft.EntityFrameworkCore;

namespace DashboardCode.Routines.Storage.EfCore
{
    public static class EfCoreManager
    {
        public static void Analyze(Exception exception, IStorageResultBuilder storageResultBuilder/*, List<FieldMessage> fieldsErrors, string entityName,*/ /*Action<Exception> analyzeInnerException*/)
        {
            if (exception is DbUpdateConcurrencyException dbUpdateConcurrencyException)
                AnalyzeDbUpdateConcurrencyException(dbUpdateConcurrencyException, storageResultBuilder /*fieldsErrors*/);
            //else if (exception is DbUpdateException dbUpdateException && dbUpdateException.InnerException!=null)
            //    analyzeInnerException(dbUpdateException.InnerException);
            else if (exception is InvalidOperationException invalidOperationException)
                AnalyzeInvalidOperationException(invalidOperationException, storageResultBuilder/* fieldsErrors, entityName*/);
        }

        static Regex fieldPkOrUniqueConstraintNullRegexV1 = new Regex("Unable to create or track an entity of type '(?<entity>.*?)' because it has a null primary or alternate key value.");
        static Regex fieldPkOrUniqueConstraintNullRegexV2 = new Regex("Unable to track an entity of type '(?<entity>.*?)' because alternate key property '(?<field>.*?)' is null.");
        public static void AnalyzeInvalidOperationException(InvalidOperationException ex, IStorageResultBuilder storageResultBuilder /*List<FieldMessage> fieldsErrors, string entityName*/)
        {
            {
                var matchCollectionV1 = fieldPkOrUniqueConstraintNullRegexV1.Matches(ex.Message);
                if (matchCollectionV1.Count > 0)
                {
                    var entity = matchCollectionV1[0].Groups["entity"].Value;
                    storageResultBuilder.AddNullPrimaryOrAlternateKey(entity);
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
                        storageResultBuilder.AddNullPrimaryOrAlternateKey(entity, field);
                    }
                    return;
                }
            }
        }

        public static void AnalyzeDbUpdateConcurrencyException(DbUpdateConcurrencyException exception, IStorageResultBuilder storageResultBuilder /*List<FieldMessage> fieldsErrors*/)
        {
            storageResultBuilder.AddConcurrencyError();
        }

        public static void Append(StringBuilder sb, Exception ex)
        {
            if (ex is DbUpdateConcurrencyException dbUpdateConcurrencyException)
                AppendDbUpdateException(sb, dbUpdateConcurrencyException);
            if (ex is DbUpdateException dbUpdateException)
                AppendDbUpdateException(sb, dbUpdateException);
        }

        public static void AppendDbUpdateException(StringBuilder stringBuilder, DbUpdateException ex)
        {
            stringBuilder.AppendMarkdownLine($"{ex.GetType().Name} data: ");
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