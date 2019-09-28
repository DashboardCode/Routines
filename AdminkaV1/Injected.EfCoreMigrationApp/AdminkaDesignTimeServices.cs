using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.DependencyInjection;

namespace DashboardCode.AdminkaV1.Injected.EfCoreMigrationApp
{
    /// <summary>
    /// Make AdminkaCSharpHelper available for Add-Migration ps command
    /// </summary>
    public class AdminkaDesignTimeServices : IDesignTimeServices
    {
        // TODO ICSharpHelper
        public void ConfigureDesignTimeServices(IServiceCollection serviceCollection)
        {
            //serviceCollection.AddSingleton<ICSharpHelper, AdminkaCSharpHelper>();
        }
    }
}