using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using DashboardCode.Routines.Storage;

namespace DashboardCode.Routines.Storage.SqlServer
{
    public static class SqlServerManager
    {
        public static void Append(StringBuilder stringBuilder, Exception exception)
        {
            if (exception is SqlException sqlException)
                 AppendSqlException(stringBuilder, sqlException);
        }

        public static void AppendSqlException(this StringBuilder stringBuilder, SqlException exception)
        {
            stringBuilder.AppendMarkdownLine("SqlException data: ");
            if (!string.IsNullOrEmpty(exception.Procedure))
            {
                stringBuilder
                    .Append("procedure: ")
                    .Append(exception.Procedure)
                    .Append(", line ").AppendMarkdownLine(exception.LineNumber.ToString());
            }
            stringBuilder.AppendLine("SqlException messages: ");
            int i = 1;
            foreach (var error in exception.Errors)
            {
                stringBuilder.AppendMarkdownProperty(i.ToString(), error.ToString());
                i++;
            }
        }

        #region Analyze
        public static void Analyze(Exception exception, List<FieldError> list, StorageModel storageModel)
        {
            if (storageModel != null)
                if (exception is SqlException sqlException)
                    AnalyzeSqlException(sqlException, storageModel, list);
        }

        const string genericErrorField = "";
        static Regex fieldLengthRegex = new Regex("String or binary data would be truncated.");
        static Regex fieldRequiredRegex = new Regex("Cannot insert the value NULL into column '(?<column>.*?)', table '(?<table>.*?)'; column does not allow nulls. (?<statementType>.*?) fails.");
        static Regex fieldUniqueIndexRegex = new Regex(@"Cannot insert duplicate key row in object '(?<table>.*?)' with unique index '(?<index>.*?)'. The duplicate key value is ((?<value>.*?)).");
        static Regex fieldUniqueConstraintRegex = new Regex("Violation of UNIQUE KEY constraint '(?<constraint>.*?)'. Cannot insert duplicate key in object '(?<table>.*?)'");
        static Regex fieldPkConstraintRegex = new Regex(@"Violation of PRIMARY KEY constraint '(?<constraint>.*?)'. Cannot insert duplicate key in object '(?<table>.*?)'. The duplicate key value is ((?<value>.*?)).");
        static Regex fieldCkRegex = new Regex("The (?<statementType>.*?) statement conflicted with the CHECK constraint \"(?<constraint>.*?)\". The conflict occurred in database \"(?<database>.*?)\", table \"(?<table>.*?)\", column '(?<column>.*?)'.");
        public static void AnalyzeSqlException(this SqlException exception, StorageModel storageModel, List<FieldError> fieldsErrors)
        {
            var message = exception.Message;
            {
                var matchCollection = fieldLengthRegex.Matches(message);
                if (matchCollection.Count > 0)
                {
                    fieldsErrors.Add("", "Some of text " + (storageModel.Binaries != null ? "" : "(or binaries) ") + "fields cannot fit into DB");
                    return;
                }

            };

            {
                var matchCollection = fieldPkConstraintRegex.Matches(message);
                if (matchCollection.Count > 0)
                {
                    var constraint = matchCollection[0].Groups["constraint"].Value;
                    var table = matchCollection[0].Groups["table"].Value;
                    if (table.Contains(storageModel.TableName) && storageModel.Key != null)
                    {
                        if (storageModel.Key.Attributes.Length == 1)
                        {
                            fieldsErrors.Add(storageModel.Key.Attributes[0], $"Allready exists in DB");
                        }
                        else
                        {
                            var csv = string.Join(",", storageModel.Key.Attributes);
                            foreach (var f in storageModel.Key.Attributes)
                                fieldsErrors.Add(f, "Allready exists is DB (multicolumn key: " + csv + ")");
                        }
                    }

                    return;
                }
            }

            {
                var matchCollection = fieldRequiredRegex.Matches(message);
                if (matchCollection.Count > 0)
                {
                    var column = matchCollection[0].Groups["column"].Value;
                    var table = matchCollection[0].Groups["table"].Value;
                    if (table.Contains(storageModel.TableName))
                    {
                        if (storageModel.Requireds != null && storageModel.Requireds.Contains(column))
                        {
                            fieldsErrors.Add(column, $"Is required!");
                        }
                    }
                    return;
                }
            }

            if (storageModel.Key != null)
            {
                if (message.Contains("Violation of PRIMARY KEY constraint"))
                {
                    if (storageModel.Key.Attributes.Length == 1)
                    {
                        fieldsErrors.Add(storageModel.Key.Attributes[0], $"ID exists in database");
                    }
                    else
                    {
                        var csv = string.Join(",", storageModel.Key.Attributes);
                        foreach (var a in storageModel.Key.Attributes)
                            fieldsErrors.Add(a, "ID exists in database (multiple fields: " + csv + ")");
                    }
                }
            };

            {
                var matchCollection = fieldUniqueIndexRegex.Matches(message);
                if (matchCollection.Count > 0)
                {
                    var index = matchCollection[0].Groups["index"].Value;
                    var table = matchCollection[0].Groups["table"].Value;
                    if (table.Contains(storageModel.TableName) && storageModel.Uniques != null)
                    {
                        var unique = storageModel.Uniques.FirstOrDefault(e => e.IndexName == index);
                        if (unique != null)
                        {
                            if (unique.Fields.Length == 1)
                            {
                                fieldsErrors.Add(unique.Fields[0], $"Allready used");
                            }
                            else
                            {
                                var csv = string.Join(",", unique.Fields);
                                foreach (var f in unique.Fields)
                                    fieldsErrors.Add(f, "Allready used (multiple fields: " + csv + ")");
                            }
                        }
                    }
                    return;
                }
            }

            {
                var matchCollection = fieldUniqueConstraintRegex.Matches(message);
                if (matchCollection.Count > 0)
                {
                    var constraint = matchCollection[0].Groups["constraint"].Value;
                    var table = matchCollection[0].Groups["table"].Value;
                    if (table.Contains(storageModel.TableName) && storageModel.Uniques != null)
                    {
                        var unique = storageModel.Uniques.FirstOrDefault(e => e.IndexName == constraint);
                        if (unique != null)
                        {
                            if (unique.Fields.Length == 1)
                            {
                                fieldsErrors.Add(unique.Fields[0], $"Allready used");
                            }
                            else
                            {
                                var csv = string.Join(",", unique.Fields);
                                foreach (var f in unique.Fields)
                                    fieldsErrors.Add(f, "Allready used (multiple fields: " + csv + ")");
                            }
                        }
                    }
                    return;
                }
            }

            // column constraint 
            if (storageModel.Constraints != null)
            {
                var matchCollection = fieldCkRegex.Matches(message);
                if (matchCollection.Count > 0)
                {
                    var constraint = matchCollection[0].Groups["constraint"].Value;
                    var table = matchCollection[0].Groups["table"].Value;
                    var statementType = matchCollection[0].Groups["statementType"].Value;
                    var database = matchCollection[0].Groups["database"].Value;
                    var column = matchCollection[0].Groups["column"].Value;
                    if (table == storageModel.TableName)
                    {
                        var ck = storageModel.Constraints.FirstOrDefault(e => e.Name == constraint);
                        if (ck != null)
                        {
                            if (ck.Fields == null || ck.Fields.Length == 0)
                            {
                                fieldsErrors.Add(genericErrorField, ck.Message);
                            }
                            if (ck.Fields.Length == 1)
                            {
                                fieldsErrors.Add(ck.Fields[0], ck.Message);
                            }
                            else
                            {
                                var csv = string.Join(",", ck.Fields);
                                foreach (var f in ck.Fields)
                                    fieldsErrors.Add(f, ck.Message);
                            }
                        }
                    }
                }
            }
        }
        #endregion
    }
}
