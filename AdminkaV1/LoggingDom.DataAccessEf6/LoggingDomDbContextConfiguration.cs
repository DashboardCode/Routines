using System.Data.Entity;
using System.Data.Entity.Infrastructure;

namespace DashboardCode.AdminkaV1.LoggingDom.DataAccessEf6
{
    public class LoggingDomDbContextConfiguration : DbConfiguration
    {
        public LoggingDomDbContextConfiguration()
        {
            SetExecutionStrategy("System.Data.SqlClient", () => new DefaultExecutionStrategy()); // alternative is SqlAzureExecutionStrategy
            SetDefaultConnectionFactory(new LocalDbConnectionFactory("mssqllocaldb"));
        }
    }
}
