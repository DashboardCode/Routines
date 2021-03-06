﻿namespace DashboardCode.AdminkaV1
{
    public enum StorageType { SQLSERVER, INMEMORY }
    public class AdminkaStorageConfiguration
    {
        public string ConnectionString { get; private set; }
        public string MigrationAssembly { get; private set; }
        public StorageType StorageType { get; private set; }
        public int? TimeOutSec { get; private set; }

        public AdminkaStorageConfiguration(string connectionString, string migrationAssembly, StorageType storageType, int? timeOutSec)
        {
            ConnectionString = connectionString;
            MigrationAssembly = migrationAssembly;
            StorageType = storageType;
            TimeOutSec = timeOutSec;
        }
    }
}