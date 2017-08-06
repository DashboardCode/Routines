using DashboardCode.Routines;
using DashboardCode.Routines.Configuration;
using DashboardCode.Routines.Configuration.NETFramework;

namespace DashboardCode.AdminkaV1.Injected.SqlServer.NETFramework.Test
{
    public class ConfigurationNETFramework : IApplicationFactory
    {
        private readonly string connectionStringName;
        readonly IConfigurationManagerLoader configurationManagerLoader;
        public ConfigurationNETFramework(string connectionStringName = "adminka")
        {
            this.connectionStringName = connectionStringName;
            configurationManagerLoader = new ConfigurationManagerLoader();
        }

        public ConfigurationContainer ComposeSpecify(MemberTag memberTag, string @for) =>
            new ConfigurationContainer(configurationManagerLoader,memberTag, @for);

        public AdminkaStorageConfiguration CreateAdminkaStorageConfiguration()
        {
            var connectionString = configurationManagerLoader.GetConnectionString(connectionStringName);
            return new AdminkaStorageConfiguration(connectionString, null, StorageType.SQLSERVER);
        }
    }
}