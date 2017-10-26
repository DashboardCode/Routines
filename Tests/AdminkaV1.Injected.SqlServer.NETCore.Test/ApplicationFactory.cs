using Microsoft.Extensions.Configuration;
using DashboardCode.Routines;
using DashboardCode.Routines.Configuration;
using DashboardCode.Routines.Configuration.NETStandard;

namespace DashboardCode.AdminkaV1.Injected.SqlServer.NETCore.Test
{
    public class ApplicationFactory : IApplicationFactory
    {
        public IConfigurationRoot ConfigurationRoot { get; private set; }
        readonly string connectionStringName;
        readonly IConfigurationManagerLoader configurationManagerLoader;
        public ApplicationFactory(string connectionStringName= "AdminkaConnectionString")
        {
            ConfigurationBuilder configurationBuilder = new ConfigurationBuilder();
            configurationBuilder.AddJsonFile("appsettings.json", false, true); // false indicates file is not optional
            this.ConfigurationRoot = configurationBuilder.Build();
            this.connectionStringName = connectionStringName;
            this.configurationManagerLoader = new ConfigurationManagerLoader(ConfigurationRoot);
        }

        public ConfigurationContainer ComposeSpecify(MemberTag memberTag, string @for) =>
            new ConfigurationContainer(configurationManagerLoader, memberTag, @for);

        public AdminkaStorageConfiguration CreateAdminkaStorageConfiguration()
        {
            var connectionString = configurationManagerLoader.GetConnectionString(connectionStringName);
            return new AdminkaStorageConfiguration(connectionString, null, StorageType.SQLSERVER);
        }
    }
}