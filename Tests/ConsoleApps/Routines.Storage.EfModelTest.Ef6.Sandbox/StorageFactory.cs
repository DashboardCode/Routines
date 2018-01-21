using System;
using System.Collections.Generic;

using DashboardCode.Routines.Storage;
using DashboardCode.Routines.Storage.SqlServer;
using DashboardCode.Routines.Storage.EfModelTest;
using DashboardCode.Routines.Storage.Ef6;

namespace DashboardCode.Ef6.Sandbox
{
    class StorageFactory
    {
        static readonly List<IOrmEntitySchemaAdapter> storageModel = new StorageMetaService().GetStorageModels();

        public static StorageResult Analyze(Exception exception, IOrmEntitySchemaAdapter storageModel)
        {
            var storageResult = StorageResultBuilder.AnalyzeExceptionRecursive(
                exception,
                storageModel, 
                "",
                (ex, errorBuilder) => {
                    
                    Ef6Manager.Analyze(exception, errorBuilder);
                    SqlServerManager.Analyze(ex, errorBuilder);
                }
            );
            return storageResult;
        }

        public static IOrmStorage CreateStorage(MyDbContext dbContext)
        {
            return new OrmStorage(dbContext, null, (o) => { });
        }
    }
}