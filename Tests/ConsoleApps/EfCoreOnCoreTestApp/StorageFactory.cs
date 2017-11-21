using System;
using System.Collections.Generic;

using DashboardCode.Routines.Storage.EfCore;
using DashboardCode.Routines.Storage.SqlServer;
using DashboardCode.Routines.Storage;
using DashboardCode.Routines.Storage.EfModelTest;
using DashboardCode.Routines.Storage.EfModelTest.EfCoreTest;

namespace DashboardCode.EfCore.NETFramework.Sandbox
{
    class StorageFactory
    {
        static readonly List<StorageModel> storageModel = new StorageMetaService().GetStorageModels();

        public static List<FieldError> Analyze(Exception exception, StorageModel storageModel)
        {
            var list = StorageErrorExtensions.AnalyzeException(exception,
                  (ex, l) => {
                      EfCoreManager.Analyze(exception, l, storageModel.Entity.Name);
                      SqlServerManager.Analyze(ex, l, storageModel);
                  }
            );
            return list;
        }

        public static IOrmStorage CreateStorage(MyDbContext dbContext)
        {
            return new OrmStorage(dbContext, null, (o) => { });
        }
    }
}