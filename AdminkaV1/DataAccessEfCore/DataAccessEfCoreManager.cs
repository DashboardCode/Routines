using System;
using System.Collections.Generic;
using System.Text;
using DashboardCode.Routines.Storage;
using DashboardCode.Routines.Storage.EfCore;

namespace DashboardCode.AdminkaV1.DataAccessEfCore
{
    public static class DataAccessEfCoreManager
    {
        public static void Analyze(Exception exception, List<FieldError> list, StorageModel storageModel)
        {
            EfCoreManager.Analyze(exception, list, storageModel.Entity.Name);
        }

        public static void Append(StringBuilder sb, Exception ex)
        {
            EfCoreManager.Append(sb, ex);
        }
    }
}