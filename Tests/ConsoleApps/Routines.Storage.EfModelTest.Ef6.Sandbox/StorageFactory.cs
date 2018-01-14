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
        static readonly List<Routines.Storage.StorageModel> storageModel = new StorageMetaService().GetStorageModels();

        public static List<FieldMessage> Analyze(Exception exception, Routines.Storage.StorageModel storageModel)
        {
            var list = StorageResultExtensions.AnalyzeException(exception,
                  (ex, l) => {
                      var errorBuilder = new ErrorBuilder(l, storageModel, "");
                      Ef6Manager.Analyze(exception, errorBuilder);
                      SqlServerManager.Analyze(ex, errorBuilder);
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