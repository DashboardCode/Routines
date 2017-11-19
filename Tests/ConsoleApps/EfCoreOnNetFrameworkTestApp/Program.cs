using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using DashboardCode.Routines.Storage.EfModelTest;
using DashboardCode.Routines.Storage.EfModelTest.EfCoreTest;
using DashboardCode.Routines.Storage.EfCore;
using DashboardCode.Routines.Storage.EfCore.Relational;

namespace DashboardCode.EfCore.NETFramework.Sandbox
{
    class Program
    {
        private static Action<DbContextOptionsBuilder<MyDbContext>> BuildOptionsBuilder(
            string connectionString,
            bool inMemory)
        {
            return (optionsBuilder) =>
            {
                if (inMemory)
                    optionsBuilder.UseInMemoryDatabase(
                      "EfCoreTest_InMemory"
                    );
                else
                    //Assembly.GetAssembly(typeof(Program))
                    optionsBuilder.UseSqlServer(
                            connectionString,
                            sqlServerDbContextOptionsBuilder => sqlServerDbContextOptionsBuilder.MigrationsAssembly( "EfCore.NETFramework.Sandbox")
                            );
            };
        }

        static void Main(string[] args)
        {
            bool inMemory = false;
            var connectionString = ConfigurationManager.ConnectionStrings["EfCoreTest"].ConnectionString;
            Console.WriteLine("Check connection string:");
            Console.WriteLine(connectionString);

            using (var dbContext = new MyDbContext(BuildOptionsBuilder(connectionString, inMemory)))
            {
                if (!inMemory)
                {
                    TestIsland.Clear(new AdoBatch(dbContext));
                }
                TestIsland.Reset(new OrmStorage(dbContext, null, (o) => { } ));
            }
            var loggerProvider = new MyLoggerProvider();
            var messages = new List<string>();
            loggerProvider.Verbose = (text) => {
                messages.Add(text);
                Console.WriteLine(text);
                Console.WriteLine();
            };
            using (var dbContext = new MyDbContext(BuildOptionsBuilder(connectionString, inMemory), loggerProvider))
            {
                StraightEfTests.TestHierarchy(dbContext);
            }
        }
    }
}
