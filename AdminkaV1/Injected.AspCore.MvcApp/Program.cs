using System.IO;
using Microsoft.AspNetCore.Hosting;

namespace DashboardCode.AdminkaV1.Injected.AspCore.MvcApp
{
    public class Program
    {
        public static void Main(string[] args)
        {
            #if DEBUG
                TestDependencies();
            #endif
            var host = new WebHostBuilder()
                .UseKestrel()
                .UseContentRoot(Directory.GetCurrentDirectory())
                .UseIISIntegration()
                .UseStartup<Startup>()
                .UseApplicationInsights()
                .Build();

            host.Run();

        }
        public static void TestDependencies()
        {
            var userContextException = new UserContextException("test");
            var storageErrorException = new Routines.Storage.StorageErrorException("test", null);
            var identity = InjectedManager.GetDefaultIdentity();
            var html = InjectedManager.Markdown("*** test ***");
        }
    }
}