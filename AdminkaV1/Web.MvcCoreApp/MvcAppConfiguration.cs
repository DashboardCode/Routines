using Microsoft.Extensions.Configuration;
using DashboardCode.Routines.Configuration;
using DashboardCode.Routines.Configuration.NETStandard;
using DashboardCode.Routines;

namespace DashboardCode.AdminkaV1.Web.MvcCoreApp
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