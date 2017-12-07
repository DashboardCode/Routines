using Ascon.NetMemoryProfiler;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace ProfilerAutomation
{
    class Program
    {
        static void Main(string[] args)
        {
            using (var session = Profiler.AttachToProcess("DashboardCode.Routines.Storage.EfModelTest.EfCore.NETFramework.Sandbox"))
            {
                var objects = session.GetAliveObjects(x => x.Type == "Microsoft.Extensions.Logging.LoggingFactory");
                var retentions = session.FindRetentions(objects);
                System.Console.WriteLine(DumpRetentions(retentions));
            }
        }

        // https://habrahabr.ru/company/ascon/blog/343684/
        private static string DumpRetentions(IEnumerable<RetentionsInfo> retentions)
        {
            StringBuilder sb = new StringBuilder();
            foreach (var group in retentions.GroupBy(x => x.Instance.TypeName))
            {
                var instances = group.ToList();
                sb.AppendLine($"Found {instances.Count} instances of {group.Key}");
                for (int i = 0; i < instances.Count; i++)
                {
                    var instance = instances[i];
                    sb.AppendLine($"Instance {i + 1}:");
                    foreach (var retentionPath in instance.RetentionPaths)
                    {
                        sb.AppendLine(retentionPath);
                        sb.AppendLine("----------------------------");
                    }
                }
            }
            return sb.ToString();
        }
    }
}
