using System;

using DashboardCode.Routines.Storage.EfCore;
using DashboardCode.Routines.Storage.SqlServer;

namespace DashboardCode.Routines.Storage.EfModelTest.EfCore
{
    public static class ExceptionExtensions
    {
        public static StorageResult Analyze(this Exception exception, Type entityType, IOrmEntitySchemaAdapter ormEntitySchemaAdapter)
        {
            var storageResult = StorageResultBuilder.AnalyzeExceptionRecursive(exception, entityType, ormEntitySchemaAdapter, "",
                  (ex, errorBuilder) => {
                      EfCoreManager.Analyze(exception, errorBuilder/*, ex2=> SqlServerManager.Analyze(ex2, errorBuilder)*/);
                      SqlServerManager.Analyze(ex, errorBuilder);
                  }
            );
            return storageResult;
        }
    }
}