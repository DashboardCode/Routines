using System;
using System.Collections.Generic;

namespace DashboardCode.AdminkaV1.LoggingDom.WcfClient
{
    public static class ExceptionExtensions
    {
        internal static void CopyData(this Exception exception, Dictionary<string, string> data)
        {
            if (data != null)
                foreach (var pair in data)
                    exception.Data[pair.Key] = pair.Value;
        }
    }
}