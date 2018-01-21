using System;
using System.Text;

using DashboardCode.Routines.Storage;
using DashboardCode.Routines.Storage.EfCore;

namespace DashboardCode.AdminkaV1.DataAccessEfCore
{
    public static class DataAccessEfCoreManager
    {
        public static void Analyze(Exception exception, IStorageResultBuilder storageResultBuilder)
           => EfCoreManager.Analyze(exception, storageResultBuilder);

        public static void Append(StringBuilder sb, Exception ex)
           => EfCoreManager.Append(sb, ex);
    }
}