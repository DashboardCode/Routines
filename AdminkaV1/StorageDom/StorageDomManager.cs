namespace AdminkaV1.StorageDom{
    public static class StorageDomManager
    {
        public static void CreateData(this IExcConnectionsStore store)
        {
           
            store.SetAsync(new ExcConnection
            {
                ExcConnectionId = "1",
                ExcConnectionCode = "ExcConnectionCode",
                ExcConnectionName = "ExcConnectionName",
                ExcConnectionDescription = "ExcConnectionDescription",
                ExcConnectionXMeta = "ExcConnectionXMeta",
                ExcConnectionType = "PARQUET",
                ExcConnectionString = "ExcConnectionString",
                ExcConnectionIsActive=true
            }).Wait();

            store.SetAsync(new ExcConnection
            {
                ExcConnectionId = "2",
                ExcConnectionCode = "1",
                ExcConnectionName = "Test connection",
                ExcConnectionDescription = "Test connection description",
                ExcConnectionXMeta = "Test connection xmeta",
                ExcConnectionType = "SqlServer",
                ExcConnectionString = "Data Source=localhost;Initial Catalog=master;Integrated Security=True",
                ExcConnectionIsActive = true
            }).Wait();
        }
    }
}
