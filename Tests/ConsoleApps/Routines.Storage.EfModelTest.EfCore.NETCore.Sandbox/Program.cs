namespace DashboardCode.Routines.Storage.EfModelTest.EfCore.Sandbox
{
    class Program
    {
        static void Main(string[] args)
        {
            var configurationRoot = ConfigurationManager.ResolveConfigurationRoot();
            var connectionString = configurationRoot.GetSection("ConnectionString").Value;

            DbContextTests.SqlServerNavigation(connectionString);

            DbContextTests.ParallelTest(connectionString);
            DbContextTests.SqlServerTest(connectionString);
            DbContextTests.InMemoryTest();
        }
    }
}