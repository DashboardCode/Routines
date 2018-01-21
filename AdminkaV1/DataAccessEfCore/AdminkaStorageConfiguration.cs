namespace DashboardCode.AdminkaV1.DataAccessEfCore
{
    public enum StorageType { SQLSERVER, INMEMORY }
    public class AdminkaStorageConfiguration
    {
        public string ConnectionString { get; private set; }
        public string MigrationAssembly { get; private set; }
        public StorageType StorageType { get; private set; }

        public AdminkaStorageConfiguration(string connectionString, string migrationAssembly, StorageType storageType)
        {
            ConnectionString = connectionString;
            MigrationAssembly = migrationAssembly;
            StorageType = storageType;
        }
    }
}
