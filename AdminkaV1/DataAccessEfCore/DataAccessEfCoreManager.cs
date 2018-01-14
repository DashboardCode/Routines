using System;
using System.Text;
using System.Collections.Generic;

using DashboardCode.Routines.Storage;
using DashboardCode.Routines.Storage.EfCore;

namespace DashboardCode.AdminkaV1.DataAccessEfCore
{
    public static class DataAccessEfCoreManager
    {
        public static void Analyze(Exception exception, IErrorBuilder errorBuilder, /* List<FieldMessage> list, StorageModel storageModel,*/ Action<Exception> analyzeInnerException)
           => EfCoreManager.Analyze(exception, errorBuilder/*list, storageModel.Entity.Name*/ , analyzeInnerException);

        public static void Append(StringBuilder sb, Exception ex)
           => EfCoreManager.Append(sb, ex);
    }
}