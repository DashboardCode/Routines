using System;
using System.Collections.Generic;
using System.Text;
using Vse.Routines.Storage;
using Vse.Routines.Storage.EfCore;

namespace Vse.AdminkaV1.DataAccessEfCore
{
    public static class DataAccessEfCoreManager
    {
        public static void Analyze(Exception exception, List<FieldError> list, StorageModel storageModel)
        {
            EfCoreManager.Analyze(exception, list, storageModel);
        }

        public static void Append(StringBuilder sb, Exception ex)
        {
            EfCoreManager.Append(sb, ex);
        }
    }
}