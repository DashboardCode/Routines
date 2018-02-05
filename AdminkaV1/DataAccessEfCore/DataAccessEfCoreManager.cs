using System;
using System.Text;
using DashboardCode.Routines.Storage;
using DashboardCode.Routines.Storage.EfCore;

namespace DashboardCode.AdminkaV1.DataAccessEfCore
{
    public static class DataAccessEfCoreManager
    {
        // just proxy which role is stop DashboardCode.Routines.Storage.EfCore reference propogation to the DashboardCode.AdminkaV1.Injected project
        public static void Analyze(Exception exception, IStorageResultBuilder storageResultBuilder)
           => EfCoreManager.Analyze(exception, storageResultBuilder);

        // another proxy which role is stop DashboardCode.Routines.Storage.EfCore reference propogation to the DashboardCode.AdminkaV1.Injected project
        public static void Append(StringBuilder sb, Exception ex)
           => EfCoreManager.Append(sb, ex);

    }
}