using System.IO;
using DashboardCode.Routines;
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
            var t0 = typeof(UserContext);
            var t1 = typeof(RoutineClosure<UserContext>);
            var identity = InjectedManager.GetDefaultIdentity();
            var html = InjectedManager.Markdown("*** test ***");
        }
    }
}