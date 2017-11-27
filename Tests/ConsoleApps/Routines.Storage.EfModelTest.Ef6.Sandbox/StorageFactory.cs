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
        static readonly List<StorageModel> storageModel = new StorageMetaService().GetStorageModels();

        public static List<FieldError> Analyze(Exception exception, StorageModel storageModel)
        {
            var list = StorageErrorExtensions.AnalyzeException(exception,
                  (ex, l) => {
                      Ef6Manager.Analyze(exception, l, storageModel);
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