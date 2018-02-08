using System;
using System.Collections.Generic;

using DashboardCode.Routines.Storage.EfCore;
using DashboardCode.Routines.Storage.SqlServer;

namespace DashboardCode.Routines.Storage.EfModelTest.EfCore
{
    static class StorageFactory
    {
        static readonly List<IOrmEntitySchemaAdapter> storageModel = new StorageMetaService().GetStorageModels();

        public static StorageResult Analyze(Exception exception, Type entityType, IOrmEntitySchemaAdapter storageModel, Action<Exception> analyzeInnerException) =>
            StorageResultBuilder.AnalyzeExceptionRecursive(exception, entityType, storageModel, "",
                  (ex, errorBuilder) => {
                      //var errorBuilder = new ErrorBuilder(l, storageModel, "");
                      EfCoreManager.Analyze(exception, errorBuilder/*, analyzeInnerException*/);
                      SqlServerManager.Analyze(ex, errorBuilder);
                  }
            );

        public static IOrmStorage CreateStorage(MyDbContext dbContext) =>
            new OrmStorage(dbContext, (ex)=> Analyze(ex, null, null, (ex2) => { }/*SqlServerManager.Analyze(ex2, null, null)*/), (o) => { });
    }
}