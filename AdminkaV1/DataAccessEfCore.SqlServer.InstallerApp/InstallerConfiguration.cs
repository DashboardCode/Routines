using Microsoft.Extensions.Configuration;

using DashboardCode.Routines;
using DashboardCode.Routines.Configuration;
using DashboardCode.Routines.Configuration.NETStandard;

namespace DashboardCode.AdminkaV1.DataAccessEfCore.SqlServer.InstallerApp
{
    public class InstallerApplicationFactory : IApplicationFactory
    {
        public IConfigurationRoot ConfigurationRoot  { get; private set;}
        readonly IConfigurationManagerLoader configurationManagerLoader;
        public InstallerApplicationFactory()
        {
            ConfigurationBuilder configurationBuilder = new ConfigurationBuilder();
            configurationBuilder.AddJsonFile("appsettings.json", false, true); // false indicates file is not optional
            this.ConfigurationRoot = configurationBuilder.Build();
            configurationManagerLoader = new ConfigurationManagerLoader(ConfigurationRoot);
        }

        public ConfigurationContainer ComposeSpecify(MemberTag memberTag, string @for) =>
            new ConfigurationContainer(configurationManagerLoader, memberTag, @for);

        public AdminkaStorageConfiguration CreateAdminkaStorageConfiguration()
        {
            var connectionString = configurationManagerLoader.GetConnectionString("AdminkaConnectionString");
            var migrationAssembly = "DashboardCode.AdminkaV1.DataAccessEfCore.SqlServer.InstallerApp";
            var storageType = StorageType.SQLSERVER;
            return new AdminkaStorageConfiguration(connectionString, migrationAssembly, storageType);
        }
    }
}