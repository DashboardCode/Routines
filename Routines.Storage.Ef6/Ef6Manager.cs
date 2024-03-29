﻿using System;
using System.Text;
using System.Text.RegularExpressions;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.Validation;

namespace DashboardCode.Routines.Storage.Ef6
{
    public static class Ef6Manager
    {
        public static void Analyze(Exception exception, IStorageResultBuilder erorrBuilder)
        {
            if (exception is DbEntityValidationException dbEntityValidationException)
                AnalyzeDbEntityValidationException(dbEntityValidationException, erorrBuilder);
            if (exception is DbUpdateConcurrencyException dbUpdateConcurrencyException)
                AnalyzeDbUpdateConcurrencyException(dbUpdateConcurrencyException, erorrBuilder);
            if (exception is InvalidOperationException invalidOperationException)
                AnalyzeInvalidOperationException(invalidOperationException, erorrBuilder);
        }


        public static void AnalyzeDbEntityValidationException(DbEntityValidationException ex, IStorageResultBuilder erorrBuilder)
        {
            foreach(var eve in ex.EntityValidationErrors)
            {
                foreach (var ve in eve.ValidationErrors)
                {
                    erorrBuilder.AddValidation(eve.Entry.Entity.GetType().Name, ve.PropertyName, ve.ErrorMessage);
                }
                //if (matchCollectionV1.Count > 0)
                //{
                //    var entity = matchCollectionV1[0].Groups["entity"].Value;
                //    erorrBuilder.AddNullPrimaryOrAlternateKey(entity);
                //    return;
                //}
            }

        }

        static readonly Regex fieldPkOrUniqueConstraintNullRegexV1 = new Regex("Unable to create or track an entity of type '(?<entity>.*?)' because it has a null primary or alternate key value.");
        static readonly Regex fieldPkOrUniqueConstraintNullRegexV2 = new Regex("Unable to track an entity of type '(?<entity>.*?)' because alternate key property '(?<field>.*?)' is null.");

        public static void AnalyzeInvalidOperationException(InvalidOperationException ex, IStorageResultBuilder erorrBuilder)
        {
            {
                var matchCollectionV1 = fieldPkOrUniqueConstraintNullRegexV1.Matches(ex.Message);
                if (matchCollectionV1.Count > 0)
                {
                    var entity = matchCollectionV1[0].Groups["entity"].Value;
                    erorrBuilder.AddNullPrimaryOrAlternateKey(entity);
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
                        erorrBuilder.AddNullPrimaryOrAlternateKey(entity, field);
                    }
                    return;
                }
            }
        }

        public static void AnalyzeDbUpdateConcurrencyException(DbUpdateConcurrencyException exception, IStorageResultBuilder erorrBuilder)
        {
            foreach(var e in exception.Entries)
                erorrBuilder.AddConcurrencyError(e.Entity.GetType().Name);
        }

        public static void Append(StringBuilder sb, Exception ex)
        {
            if (ex is DbUpdateConcurrencyException dbUpdateConcurrencyException)
                AppendDbUpdateConcurrencyException(sb, dbUpdateConcurrencyException);
            if (ex is DbEntityValidationException dbEntityValidationException)
                AppendDbEntityValidationException(sb, dbEntityValidationException);
        }

        public static void AppendDbUpdateConcurrencyException(StringBuilder stringBuilder, DbUpdateConcurrencyException ex)
        {
            stringBuilder.AppendMarkdownLine("DbUpdateConcurrencyException data: ");
            int i = 1;
            foreach (var e in ex.Entries)
            {
                stringBuilder.AppendMarkdownProperty(i.ToString(), e.Entity.GetType().FullName);
                var propertyNames = e.CurrentValues.PropertyNames;
                foreach (var p in propertyNames)
                    stringBuilder.AppendMarkdownProperty("   " + p, (e.CurrentValues.GetValue<object>(p)?.ToString()) ?? "(null)");
                i++;
            }
        }

        public static void AppendDbEntityValidationException(StringBuilder stringBuilder, DbEntityValidationException ex)
        {
            stringBuilder.AppendMarkdownLine("DbEntityValidationException data:");
            foreach (var errors in ex.EntityValidationErrors)
            {
                foreach (var dbValidationError in errors.ValidationErrors)
                {
                    stringBuilder.AppendMarkdownProperty("   " + dbValidationError.PropertyName, dbValidationError.ErrorMessage);
                }
            }
        }
    }
}