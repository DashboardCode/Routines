using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;

using DashboardCode.Routines;


namespace DashboardCode.AdminkaV1.Injected.AspCore.WebApp
{
    public static class Program
    {
        public static void Main(string[] args) 
        {
            try
            {
#if DEBUG
                TestDependencies();  // fail early test
#endif
                CreateWebHostBuilder(args).Build().Run();
            }
            finally
            {
                // Ensure to flush and stop internal timers/threads before application-exit (Avoid segmentation fault on Linux)
                NLog.LogManager.Shutdown();
            }
        }

        public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
           WebHost.CreateDefaultBuilder(args)
               .UseStartup<Startup>();


        public static string TestDependencies()
        {

            var t0 = typeof(UserContext);
            var t1 = typeof(RoutineClosure<UserContext>);
            var identity = InjectedManager.GetDefaultIdentity();
            return InjectedManager.Markdown($"*** fail early {t1.GetType().Name} {t0.Assembly} {identity}***");
        }
    }
}