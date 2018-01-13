using System;
using System.Collections.Generic;

using DashboardCode.Routines.Storage.EfCore;
using DashboardCode.Routines.Storage.SqlServer;

namespace DashboardCode.Routines.Storage.EfModelTest.EfCore
{
    static class StorageFactory
    {
        static readonly List<StorageModel> storageModel = new StorageMetaService().GetStorageModels();

        public static List<FieldError> Analyze(Exception exception, StorageModel storageModel, Action<Exception> analyzeInnerException) =>
            StorageErrorExtensions.AnalyzeException(exception,
                  (ex, l) => {
                      EfCoreManager.Analyze(exception, l, storageModel.Entity.Name, analyzeInnerException);
                      SqlServerManager.Analyze(ex, l, storageModel);
                  }
            );

        public static IOrmStorage CreateStorage(MyDbContext dbContext) =>
            new OrmStorage(dbContext, (ex)=> Analyze(ex, null, (ex2) => { }/*SqlServerManager.Analyze(ex2, null, null)*/), (o) => { });
    }
}