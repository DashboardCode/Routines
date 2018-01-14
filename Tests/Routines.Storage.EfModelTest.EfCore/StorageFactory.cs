using System;
using System.Collections.Generic;

using DashboardCode.Routines.Storage.EfCore;
using DashboardCode.Routines.Storage.SqlServer;

namespace DashboardCode.Routines.Storage.EfModelTest.EfCore
{
    static class StorageFactory
    {
        static readonly List<StorageModel> storageModel = new StorageMetaService().GetStorageModels();

        public static List<FieldMessage> Analyze(Exception exception, StorageModel storageModel, Action<Exception> analyzeInnerException) =>
            StorageResultExtensions.AnalyzeException(exception,
                  (ex, l) => {
                      var errorBuilder = new ErrorBuilder(l, storageModel, "");
                      EfCoreManager.Analyze(exception, errorBuilder, analyzeInnerException);
                      SqlServerManager.Analyze(ex, errorBuilder);
                  }
            );

        public static IOrmStorage CreateStorage(MyDbContext dbContext) =>
            new OrmStorage(dbContext, (ex)=> Analyze(ex, null, (ex2) => { }/*SqlServerManager.Analyze(ex2, null, null)*/), (o) => { });
    }
}