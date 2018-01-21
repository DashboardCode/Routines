using Microsoft.Extensions.Configuration;

using DashboardCode.Routines;
using DashboardCode.Routines.Configuration;
using DashboardCode.Routines.Configuration.NETStandard;
using DashboardCode.AdminkaV1.DataAccessEfCore;

namespace DashboardCode.AdminkaV1.Injected.NETStandard
{
    public class InMemoryAdmikaConfigurationFacade : IAdmikaConfigurationFacade
    {
        public IConfigurationRoot ConfigurationRoot { get; private set; }
        private readonly string databaseName;
        readonly IConfigurationManagerLoader configurationManagerLoader;
        public InMemoryAdmikaConfigurationFacade(string databaseName)
        {
            ConfigurationBuilder configurationBuilder = new ConfigurationBuilder();
            configurationBuilder.AddJsonFile("appsettings.json", false, true); // false indicates file is not optional
            this.ConfigurationRoot = configurationBuilder.Build();
            this.databaseName = databaseName;
            configurationManagerLoader = new ConfigurationManagerLoader(ConfigurationRoot);
        }

        public AdminkaStorageConfiguration ResolveAdminkaStorageConfiguration() =>
            new AdminkaStorageConfiguration(databaseName, null, StorageType.INMEMORY);

        public ConfigurationContainer ComposeSpecify(MemberTag memberTag, string @for) =>
            new ConfigurationContainer(configurationManagerLoader, memberTag, @for);
    }
}