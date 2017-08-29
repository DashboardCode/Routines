using Microsoft.EntityFrameworkCore.Design;//.Infrastructure;
using System.Configuration;

namespace EfCoreOnNetFrameworkTestApp
{
    public class MyDbContextFactory : /*IDbContextFactory<MyDbContext>,*/ IDesignTimeDbContextFactory<MyDbContext>
    {
        //public MyDbContext Create(DbContextFactoryOptions factoryOptions)
        //{
        //    var connectionString = ConfigurationManager.ConnectionStrings["EfCoreTest"].ConnectionString;
        //    return new MyDbContext(connectionString);
        //}

        public MyDbContext CreateDbContext(string[] args)
        {
            throw new System.NotImplementedException();
        }
    }
}
