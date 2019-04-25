using DashboardCode.Routines;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;


namespace DashboardCode.AdminkaV1.Injected.AspCore.MvcApp
{
    public class Program
    {
        public static void Main(string[] args)
        {
            #if  DEBUG
                TestDependencies();  // fail early test
            #endif
            CreateWebHostBuilder(args).Build().Run();
        }

        public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .UseStartup<Startup>()
                //.ConfigureLogging((hostingContext, logging) =>
                //{
                //    // need those usings: Microsoft.Extensions.Logging, Microsoft.Extensions.Logging.Console;
                //    ConsoleLoggerExtensions.AddConsole(logging)
                //        .AddFilter<ConsoleLoggerProvider>
                //            (category: null, level: LogLevel.Information)
                //       .AddFilter<ConsoleLoggerProvider>
                //           ((category, level) => category == "A" ||
                //               level == LogLevel.Critical);
                //})
            //.UseKestrel()
            //.UseContentRoot(System.IO.Directory.GetCurrentDirectory())
            //.UseIISIntegration()
            //.UseStartup<Startup>()
            //.UseApplicationInsights()
            ;

        public static void TestDependencies()
        {
            var t0 = typeof(UserContext);
            var t1 = typeof(RoutineClosure<UserContext>);
            var identity = InjectedManager.GetDefaultIdentity();
            var html = InjectedManager.Markdown($"*** test {t1.GetType().Name} ***");
        }
    }
}