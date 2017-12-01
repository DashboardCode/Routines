using Microsoft.Extensions.Configuration;

using DashboardCode.Routines;
using DashboardCode.Routines.Configuration;
using DashboardCode.Routines.Configuration.NETStandard;

namespace DashboardCode.AdminkaV1.Injected.AspCore.MvcApp
{
    public class MvcApplicationFactory : IApplicationFactory
    {
        readonly IConfigurationManagerLoader configurationManagerLoader;
        readonly AdminkaStorageConfiguration adminkaStorageConfiguration;
        public MvcApplicationFactory(IConfigurationRoot configurationRoot)
        {
            configurationManagerLoader = new ConfigurationManagerLoader(configurationRoot);
            adminkaStorageConfiguration =
                new AdminkaStorageConfiguration(configurationManagerLoader.GetConnectionString("AdminkaConnectionString"), 
                default(string), StorageType.SQLSERVER);
        }

        public ConfigurationContainer ComposeSpecify(MemberTag memberTag, string @for) =>
            new ConfigurationContainer(configurationManagerLoader, memberTag, @for);

        public AdminkaStorageConfiguration CreateAdminkaStorageConfiguration() =>
            adminkaStorageConfiguration;
    }
}