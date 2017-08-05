using DashboardCode.Routines.Configuration;
using DashboardCode.Routines.Configuration.NETFramework;
using DashboardCode.Routines;

namespace DashboardCode.AdminkaV1.Injected.InMemory.NETFramework.Test
{
    public class ConfigurationNETFramework : IAppConfiguration
    {
        private readonly StorageType storageType;
        private readonly string migrationAssembly;
        private readonly string connectionString;
        private readonly string connectionStringName;
        public ConfigurationNETFramework(StorageType storageType = StorageType.SQLSERVER, string migrationAssembly = null, string connectionString = null, string connectionStringName = "adminka")
        {
            this.storageType = storageType;
            this.connectionString = connectionString;
            this.connectionStringName = connectionStringName;
            this.migrationAssembly = migrationAssembly;
        }

        public ISpecifiableConfigurationContainer ResolveConfigurationContainer(MemberTag memberTag) =>
            RoutinesConfigurationManager.GetConfigurationContainer(memberTag);

        public string ResolveConnectionString() =>
            connectionString??RoutinesConfigurationManager.GetConnectionString("adminka");

        public string ResolveMigrationAssembly() =>
            migrationAssembly;

        public StorageType ResolveStorageType() =>
            storageType;
    }
}