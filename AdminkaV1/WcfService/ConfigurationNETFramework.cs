using DashboardCode.Routines;
using DashboardCode.Routines.Configuration;
using DashboardCode.Routines.Configuration.NETFramework;

namespace DashboardCode.AdminkaV1.Wcf.Messaging
{
    public class ConfigurationNETFramework : IAppConfiguration
    {
        public SpecifiableConfigurationContainer ResolveConfigurationContainer(MemberTag memberTag)=>
            RoutinesConfigurationManager.GetConfigurationContainer(memberTag);

        public string ResolveConnectionString() =>
            RoutinesConfigurationManager.GetConnectionString("adminka");

        public string ResolveMigrationAssembly()
            => null;

        public StorageType ResolveStorageType() =>
            StorageType.INMEMORY;
    }
}