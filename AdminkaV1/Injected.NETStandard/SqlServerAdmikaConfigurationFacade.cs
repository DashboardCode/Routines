using Microsoft.Extensions.Configuration;

using DashboardCode.Routines;
using DashboardCode.Routines.Configuration;
using DashboardCode.Routines.Configuration.NETStandard;
using DashboardCode.AdminkaV1.DataAccessEfCore;

namespace DashboardCode.AdminkaV1.Injected.NETStandard
{
    public class SqlServerAdmikaConfigurationFacade : IAdmikaConfigurationFacade
    {
        public IConfigurationRoot ConfigurationRoot { get; private set; }
        readonly string connectionStringName;
        readonly string migrationAssembly;
        readonly IConfigurationManagerLoader configurationManagerLoader;

        private static IConfigurationRoot GetDefaultConfigurationRoot()
        {
            ConfigurationBuilder configurationBuilder = new ConfigurationBuilder();
            configurationBuilder.AddJsonFile("appsettings.json", false, true); // false indicates file is not optional
            var configurationRoot = configurationBuilder.Build();
            return configurationRoot;
        }

        public SqlServerAdmikaConfigurationFacade(string connectionStringName = "AdminkaConnectionString", string migrationAssembly = null) : this(
            GetDefaultConfigurationRoot(), connectionStringName, migrationAssembly)
        {

        }

        public SqlServerAdmikaConfigurationFacade(IConfigurationRoot configurationRoot, string connectionStringName = "AdminkaConnectionString", string migrationAssembly = null)
        {
            this.ConfigurationRoot = configurationRoot;
            this.connectionStringName = connectionStringName;
            this.migrationAssembly = migrationAssembly;
            this.configurationManagerLoader = new ConfigurationManagerLoader(ConfigurationRoot);
        }


        public ConfigurationContainer ComposeSpecify(MemberTag memberTag, string @for) =>
            new ConfigurationContainer(configurationManagerLoader, memberTag, @for);

        public AdminkaStorageConfiguration ResolveAdminkaStorageConfiguration()
        {
            var connectionString = configurationManagerLoader.GetConnectionString(connectionStringName);
            return new AdminkaStorageConfiguration(connectionString, null, StorageType.SQLSERVER);
        }
    }
}