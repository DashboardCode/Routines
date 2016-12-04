using Microsoft.EntityFrameworkCore.Infrastructure;
using System.Configuration;

namespace EfCoreTest
{
    public class MyDbContextFactory : IDbContextFactory<MyDbContext>
    {
        public MyDbContext Create(DbContextFactoryOptions factoryOptions)
        {
            var connectionString = ConfigurationManager.ConnectionStrings["EfCoreTest"].ConnectionString;
            return new MyDbContext(connectionString);
        }
    }
}
