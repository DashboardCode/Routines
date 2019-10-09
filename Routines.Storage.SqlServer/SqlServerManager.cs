﻿using System;
using System.Text;
using System.Text.RegularExpressions;
using System.Data;
using System.Data.Common;
using Microsoft.Data.SqlClient;
using System.Threading.Tasks;


namespace DashboardCode.Routines.Storage.SqlServer
{
    public static class SqlServerManager
    {
        public static async Task<long> GetApproximateRowCountAsync(DbConnection connection, string objname)
        {
            var command = connection.CreateCommand();
            command.CommandType = CommandType.StoredProcedure;
            command.CommandText = "sp_spaceused";
            var parameter1 = command.CreateParameter();
            parameter1.ParameterName = "@objname";
            parameter1.Value = objname;
            parameter1.DbType = DbType.String;
            command.Parameters.Add(parameter1);
            using var reader = await command.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                var rowsText = reader[1] as string;
                return long.Parse(rowsText);
            }
            return 0;
        }

        public static long GetApproximateRowCount(DbConnection connection, string objname)
        {
            var command = connection.CreateCommand();
            command.CommandType = CommandType.StoredProcedure;
            command.CommandText = "sp_spaceused";
            var parameter1 = command.CreateParameter();
            parameter1.ParameterName = "@objname";
            parameter1.Value = objname;
            parameter1.DbType = DbType.String;
            command.Parameters.Add(parameter1);
            // TODO: 
            // ((SqlConnection)connection).RetrieveStatistics
            // https://docs.microsoft.com/en-us/dotnet/framework/data/adonet/sql/provider-statistics-for-sql-server 
            using var reader = command.ExecuteReader();
            while (reader.Read())
            {
                var rowsText = reader[1] as string;
                return long.Parse(rowsText);
            }
            return 0;
        }

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
        static readonly Regex fieldLengthRegex = new Regex("String or binary data would be truncated.");
        static readonly Regex fieldRequiredRegex = new Regex("Cannot insert the value NULL into column '(?<column>.*?)', table '(?<table>.*?)'; column does not allow nulls. (?<statementType>.*?) fails.");
        static readonly Regex fieldUniqueIndexRegex = new Regex(@"Cannot insert duplicate key row in object '(?<table>.*?)' with unique index '(?<index>.*?)'. The duplicate key value is ((?<value>.*?)).");
        static readonly Regex fieldUniqueConstraintRegex = new Regex("Violation of UNIQUE KEY constraint '(?<constraint>.*?)'. Cannot insert duplicate key in object '(?<table>.*?)'");
        static readonly Regex fieldPkConstraintRegex = new Regex(@"Violation of PRIMARY KEY constraint '(?<constraint>.*?)'. Cannot insert duplicate key in object '(?<table>.*?)'. The duplicate key value is ((?<value>.*?)).");
        static readonly Regex fieldCkRegex = new Regex("The (?<statementType>.*?) statement conflicted with the CHECK constraint \"(?<constraint>.*?)\". The conflict occurred in database \"(?<database>.*?)\", table \"(?<table>.*?)\", column '(?<column>.*?)'.");
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
                    errorBuilder.AddNullError(column, table);
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
                    errorBuilder.AddCheckConstraintViolations(constraint, table);
                }
            }
        }
        #endregion

        public static bool FindSqlException(AggregateException aggregateException, out SqlException sqlException)
        {
            sqlException = null;
            foreach (var ex in aggregateException.InnerExceptions)
            {
                if (ex is SqlException)
                {
                    sqlException = (SqlException)ex;
                    return true;
                }
            }
            return false;
        }

        
        public static RemoteServerErrorType QuickAnalyze(Exception unhandledException)
        {
            var remoteServerErrorType = RemoteServerErrorType.SPECIFIC;
            SqlException sqlException = null;
            if (unhandledException is SqlException)
            {
                sqlException = (SqlException)unhandledException;
            }
            else if (unhandledException is AggregateException aggregateException)
            {
                FindSqlException((AggregateException)unhandledException, out sqlException);
            }
            if (sqlException != null)
            {
                switch (sqlException.Number)
                {
                    case 17:    // SQL Server does not exist or access denied.
                    case 40:    // Could not open a connection to SQL Server
                    case 4060:  // Invalid Database (checked by SYS.MESSAGES)
                    case 18456: // Login Failed (checked by SYS.MESSAGES)
                    case 9002:  // Full transaction log (checked by SYS.MESSAGES) - means under maitinance job
                        remoteServerErrorType = RemoteServerErrorType.DOWN;
                        break;
                    case 1205:  // DeadLock Victim (checked by SYS.MESSAGES)
                    case -2:    // Client level timeout - Execution Timeout Expired.  The timeout period elapsed prior to completion of the operation or the server is not responding.
                        remoteServerErrorType = RemoteServerErrorType.OVERLOADED;
                        break;
                }
            }
            return remoteServerErrorType;
        }
    }
}