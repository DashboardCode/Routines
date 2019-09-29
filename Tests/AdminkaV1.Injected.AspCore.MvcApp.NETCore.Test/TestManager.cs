using System.Linq;
using System.IO;
using System.Reflection;

using Microsoft.CodeAnalysis;
//using Microsoft.Extensions.PlatformAbstractions;
using Microsoft.Extensions.DependencyInjection;

using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.ApplicationParts;
using Microsoft.AspNetCore.Mvc.ViewComponents;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Razor;

namespace DashboardCode.AdminkaV1.Injected.AspCore.MvcApp.NETCore.Test
{
    public static class TestManager
    {
        public readonly static ApplicationSettings ApplicationSettings = InjectedManager.CreateApplicationSettingsStandard();
        /// <summary>
        ///   D:\cot\DashboardCode\Routines\Tests\AdminkaV1.Injected.AspCore.MvcApp.NETCore.Test\bin\Debug\netcoreapp2.0\
        ///   to
        ///   D:\cot\DashboardCode\Routines\AdminkaV1\Injected.AspCore.MvcApp\wwwroot
        /// </summary>
        /// <returns></returns>
        public static string GetContentRoot()
        {
            // alternatively
            //var path = System.AppContext.BaseDirectory;
            var path = System.AppDomain.CurrentDomain.BaseDirectory;
                
            var contentRoot = Path.GetFullPath(Path.Combine(path, ".\\..\\..\\..\\..\\..\\AdminkaV1\\Injected.AspCore.MvcApp"));
            return contentRoot;
        }

        //public static void InitializeServices(IServiceCollection services)
        //{
        //    services.Configure((RazorViewEngineOptions options) =>
        //    {
        //        var previous = options.CompilationCallback;
        //        options.CompilationCallback = (context) =>
        //        {
        //            previous?.Invoke(context);

        //            var assembly = typeof(Startup).GetTypeInfo().Assembly;
        //            var assemblies = assembly.GetReferencedAssemblies().Select(x => MetadataReference.CreateFromFile(Assembly.Load(x).Location))
        //            .ToList();
        //            assemblies.Add(MetadataReference.CreateFromFile(Assembly.Load(new AssemblyName("mscorlib")).Location));
        //            assemblies.Add(MetadataReference.CreateFromFile(Assembly.Load(new AssemblyName("System.Private.Corelib")).Location));
        //            assemblies.Add(MetadataReference.CreateFromFile(Assembly.Load(new AssemblyName("Microsoft.AspNetCore.Razor")).Location));
        //            //assemblies.Add(MetadataReference.CreateFromFile(Assembly.Load(new AssemblyName("System.Linq")).Location));
        //            //assemblies.Add(MetadataReference.CreateFromFile(Assembly.Load(new AssemblyName("System.Threading.Tasks")).Location));
        //            //assemblies.Add(MetadataReference.CreateFromFile(Assembly.Load(new AssemblyName("System.Runtime")).Location));
        //            //assemblies.Add(MetadataReference.CreateFromFile(Assembly.Load(new AssemblyName("System.Dynamic.Runtime")).Location));
        //            //assemblies.Add(MetadataReference.CreateFromFile(Assembly.Load(new AssemblyName("Microsoft.AspNetCore.Razor.Runtime")).Location));
        //            //assemblies.Add(MetadataReference.CreateFromFile(Assembly.Load(new AssemblyName("Microsoft.AspNetCore.Mvc")).Location));
        //            //assemblies.Add(MetadataReference.CreateFromFile(Assembly.Load(new AssemblyName("Microsoft.AspNetCore.Razor")).Location));
        //            //assemblies.Add(MetadataReference.CreateFromFile(Assembly.Load(new AssemblyName("Microsoft.AspNetCore.Mvc.Razor")).Location));
        //            //assemblies.Add(MetadataReference.CreateFromFile(Assembly.Load(new AssemblyName("Microsoft.AspNetCore.Html.Abstractions")).Location));
        //            //assemblies.Add(MetadataReference.CreateFromFile(Assembly.Load(new AssemblyName("System.Text.Encodings.Web")).Location));
        //            context.Compilation = context.Compilation.AddReferences(assemblies);
        //        };
        //    });

        //    var startupAssembly = typeof(Startup).GetTypeInfo().Assembly;

        //    // Inject a custom application part manager. 
        //    // Overrides AddMvcCore() because it uses TryAdd().
        //    var manager = new ApplicationPartManager();
        //    manager.ApplicationParts.Add(new AssemblyPart(startupAssembly));
        //    manager.FeatureProviders.Add(new ControllerFeatureProvider());
        //    manager.FeatureProviders.Add(new ViewComponentFeatureProvider());

        //    services.AddSingleton(manager);
        //}
    }
}