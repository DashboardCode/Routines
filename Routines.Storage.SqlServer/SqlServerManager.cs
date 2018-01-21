using System;
using System.Data.SqlClient;
using System.Text;
using System.Text.RegularExpressions;

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
            stringBuilder.AppendMarkdownLine("SqlException Properties: ");
            if (!string.IsNullOrEmpty(exception.Procedure))
            {
                stringBuilder
                    .Append("Procedure: ")
                    .Append(exception.Procedure)
                    .Append(", line ").AppendMarkdownLine(exception.LineNumber.ToString());
            }
            if (exception.Errors != null && exception.Errors.Count > 0)
            {
                stringBuilder.AppendMarkdownLine("Errors: ");
                int i = 1;
                foreach (var error in exception.Errors)
                {
                    if (error is SqlError sqlError)
                        stringBuilder.AppendMarkdownEnumeration(i, sqlError.ToString() + ((sqlError.LineNumber > 0) ? (" line:" + sqlError.LineNumber) : ""));
                    else
                        stringBuilder.AppendMarkdownEnumeration(i, error.ToString());
                    i++;
                }
            }
        }

        #region Analyze
        public static void Analyze(Exception exception, IStorageResultBuilder storageResultBuilder)
        {
            if (storageResultBuilder != null)
                if (exception is SqlException sqlException)
                    AnalyzeSqlException(sqlException, storageResultBuilder);
        }

        const string genericErrorField = "";
        static Regex fieldLengthRegex = new Regex("String or binary data would be truncated.");
        static Regex fieldRequiredRegex = new Regex("Cannot insert the value NULL into column '(?<column>.*?)', table '(?<table>.*?)'; column does not allow nulls. (?<statementType>.*?) fails.");
        static Regex fieldUniqueIndexRegex = new Regex(@"Cannot insert duplicate key row in object '(?<table>.*?)' with unique index '(?<index>.*?)'. The duplicate key value is ((?<value>.*?)).");
        static Regex fieldUniqueConstraintRegex = new Regex("Violation of UNIQUE KEY constraint '(?<constraint>.*?)'. Cannot insert duplicate key in object '(?<table>.*?)'");
        static Regex fieldPkConstraintRegex = new Regex(@"Violation of PRIMARY KEY constraint '(?<constraint>.*?)'. Cannot insert duplicate key in object '(?<table>.*?)'. The duplicate key value is ((?<value>.*?)).");
        static Regex fieldCkRegex = new Regex("The (?<statementType>.*?) statement conflicted with the CHECK constraint \"(?<constraint>.*?)\". The conflict occurred in database \"(?<database>.*?)\", table \"(?<table>.*?)\", column '(?<column>.*?)'.");
        public static void AnalyzeSqlException(this SqlException exception, IStorageResultBuilder errorBuilder)
        {
            var message = exception.Message;
            {
                var matchCollection = fieldLengthRegex.Matches(message);
                if (matchCollection.Count > 0)
                {
                    errorBuilder.AddTruncationError();
                    return;
                }

            };

            {
                var matchCollection = fieldPkConstraintRegex.Matches(message);
                if (matchCollection.Count > 0)
                {
                    var constraint = matchCollection[0].Groups["constraint"].Value;
                    var table = matchCollection[0].Groups["table"].Value;
                    errorBuilder.AddPkDuplicateError(constraint, table);
                    return;
                }
            }

            {
                var matchCollection = fieldRequiredRegex.Matches(message);
                if (matchCollection.Count > 0)
                {
                    var column = matchCollection[0].Groups["column"].Value;
                    var table = matchCollection[0].Groups["table"].Value;
                    errorBuilder.AddPkDuplicateError(column, table);
                    return;
                }
            }
            
            if (message.Contains("Violation of PRIMARY KEY constraint"))
            {
                errorBuilder.AddPkDuplicateError(); 
            }
            

            {
                var matchCollection = fieldUniqueIndexRegex.Matches(message);
                if (matchCollection.Count > 0)
                {
                    var index = matchCollection[0].Groups["index"].Value;
                    var table = matchCollection[0].Groups["table"].Value;
                    errorBuilder.AddUniqueIndexViolations(index, table);
                    return;
                }
            }

            {
                var matchCollection = fieldUniqueConstraintRegex.Matches(message);
                if (matchCollection.Count > 0)
                {
                    var constraint = matchCollection[0].Groups["constraint"].Value;
                    var table = matchCollection[0].Groups["table"].Value;
                    errorBuilder.AddUniqueConstraintViolations(constraint, table);
                    return;
                }
            }

            {
                var matchCollection = fieldCkRegex.Matches(message);
                if (matchCollection.Count > 0)
                {
                    var constraint = matchCollection[0].Groups["constraint"].Value;
                    var table = matchCollection[0].Groups["table"].Value;
                    var statementType = matchCollection[0].Groups["statementType"].Value;
                    var database = matchCollection[0].Groups["database"].Value;
                    var column = matchCollection[0].Groups["column"].Value;
                    errorBuilder.AddUniqueConstraintViolations(constraint, table);
                }
            }
        }
        #endregion
    }
}
