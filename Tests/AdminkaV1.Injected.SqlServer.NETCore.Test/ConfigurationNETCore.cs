using Microsoft.Extensions.Configuration;
using DashboardCode.Routines;
using DashboardCode.Routines.Configuration;
using DashboardCode.Routines.Configuration.NETStandard;

namespace DashboardCode.AdminkaV1.Injected.SqlServer.NETCore.Test
{
    public class ConfigurationNETCore : IAppConfiguration
    {
        public IConfigurationRoot ConfigurationRoot { get; private set; }
        private readonly StorageType storageType;
        private readonly string migrationAssembly;
        private readonly string connectionString;
        private readonly string connectionStringName;
        public ConfigurationNETCore(StorageType storageType= StorageType.SQLSERVER, string migrationAssembly= null, string connectionString=null, string connectionStringName="adminka")
        {
            ConfigurationBuilder configurationBuilder = new ConfigurationBuilder();
            configurationBuilder.AddJsonFile("appsettings.json", false, true); // false indicates file is not optional
            this.ConfigurationRoot = configurationBuilder.Build();
            this.storageType = storageType;
            this.migrationAssembly = migrationAssembly;
            this.connectionString = connectionString;
            this.connectionStringName = connectionStringName;
        }

        public ISpecifiableConfigurationContainer ResolveConfigurationContainer(MemberTag memberTag) =>
            RoutinesConfigurationManager.CreateConfigurationContainer(ConfigurationRoot, memberTag);

        public string ResolveConnectionString() =>
            connectionString??RoutinesConfigurationManager.MakeConnectionString(ConfigurationRoot, connectionStringName);

        public string ResolveMigrationAssembly() =>
            migrationAssembly;

        public StorageType ResolveStorageType() =>
            storageType;
    }
}
