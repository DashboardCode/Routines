using Microsoft.Extensions.Configuration;
using DashboardCode.Routines.Configuration;
using DashboardCode.Routines.Configuration.NETStandard;

namespace DashboardCode.AdminkaV1.Web.MvcCoreApp
{
    public class MvcAppConfiguration : IAppConfiguration
    {
        public readonly IConfigurationRoot ConfigurationRoot;
        public MvcAppConfiguration(IConfigurationRoot configurationRoot) =>
            ConfigurationRoot=configurationRoot;

        public SpecifiableConfigurationContainer GetConfigurationContainer(string @namespace, string @class, string member) =>
            RoutinesConfigurationManager.CreateConfigurationContainer(ConfigurationRoot, @namespace, @class, member);

        public string GetConnectionString() =>
            RoutinesConfigurationManager.GetConnectionString(ConfigurationRoot, "adminka");

        public string GetMigrationAssembly() =>
            null;

        public StorageType GetStorageType() =>
            StorageType.SQLSERVER;
    }
}
