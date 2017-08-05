using Microsoft.Extensions.Configuration;
using DashboardCode.Routines.Configuration;
using DashboardCode.Routines.Configuration.NETStandard;
using DashboardCode.Routines;

namespace DashboardCode.AdminkaV1.Web.MvcCoreApp
{
    public class MvcAppConfiguration : IAppConfiguration
    {
        public readonly IConfigurationRoot ConfigurationRoot;
        public MvcAppConfiguration(IConfigurationRoot configurationRoot) =>
            ConfigurationRoot=configurationRoot;

        public ISpecifiableConfigurationContainer ResolveConfigurationContainer(MemberTag memberTag) =>
            RoutinesConfigurationManager.CreateConfigurationContainer(ConfigurationRoot, memberTag);

        public string ResolveConnectionString() =>
            RoutinesConfigurationManager.MakeConnectionString(ConfigurationRoot, "adminka");

        public string ResolveMigrationAssembly() =>
            null;

        public StorageType ResolveStorageType() =>
            StorageType.SQLSERVER;
    }
}