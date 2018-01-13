using System;
using System.Collections.Generic;

using DashboardCode.Routines.Storage.EfCore;
using DashboardCode.Routines.Storage.SqlServer;

namespace DashboardCode.Routines.Storage.EfModelTest.EfCore
{
    public static class ExceptionExtensions
    {
        public static List<FieldError> Analyze(this Exception exception, StorageModel storageModel)
        {
            var list = StorageErrorExtensions.AnalyzeException(exception,
                  (ex, l) => {
                      EfCoreManager.Analyze(exception, l, storageModel.Entity.Name, ex2=> SqlServerManager.Analyze(ex2, l, storageModel));
                      SqlServerManager.Analyze(ex, l, storageModel);
                  }
            );
            return list;
        }
    }
}