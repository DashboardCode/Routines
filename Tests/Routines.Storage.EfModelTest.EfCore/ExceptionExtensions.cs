using System;
using System.Collections.Generic;

using DashboardCode.Routines.Storage.EfCore;
using DashboardCode.Routines.Storage.SqlServer;

namespace DashboardCode.Routines.Storage.EfModelTest.EfCore
{
    public static class ExceptionExtensions
    {
        public static List<FieldMessage> Analyze(this Exception exception, StorageModel storageModel)
        {
            var list = StorageResultExtensions.AnalyzeException(exception,
                  (ex, l) => {
                      var errorBuilder = new ErrorBuilder(l, storageModel, "");
                      EfCoreManager.Analyze(exception, errorBuilder, ex2=> SqlServerManager.Analyze(ex2, errorBuilder));
                      SqlServerManager.Analyze(ex, errorBuilder);
                  }
            );
            return list;
        }
    }
}