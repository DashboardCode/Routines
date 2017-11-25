using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

using DashboardCode.Routines.Storage.EfCore;
using DashboardCode.Routines.Storage.EfModelTest;
using DashboardCode.Routines.Storage.EfModelTest.EfCoreTest;
using DashboardCode.Routines.Storage.EfModelTest.EfCore;

namespace DashboardCode.EfCore.NETCore.Sandbox
{
    class Program
    {
        static void Main(string[] args)
        {
            var messages = new List<string>();
            Action<string> verbose = (text) => {
                messages.Add(text);
                Console.WriteLine(text);
                Console.WriteLine();
            };
            var MyLoggerFactory = StatefullLoggerFactoryPool.Instance.Get(verbose, new LoggerProviderConfiguration() { Enabled = true });

            var databaseName = "MyTest_InMemmory";
            Console.Write($"DatabaseName: {databaseName}");
            using (var dbContext = new MyDbContext(BuildOptionsBuilder(databaseName, verbose)))
            {
                TestIsland.Reset(new OrmStorage(dbContext, null, (o)=> { }));
            }
            using (var dbContext = new MyDbContext(BuildOptionsBuilder(databaseName, verbose)))
            {
                StraightEfTests.TestHierarchy(dbContext);
            }
        }

        private static Action<DbContextOptionsBuilder<MyDbContext>> BuildOptionsBuilder(string databaseName, Action<string> verbose)
        {
            return (optionsBuilder) =>
            {
                optionsBuilder.UseInMemoryDatabase(databaseName);
                var loggerFactory = StatefullLoggerFactoryPool.Instance.Get(verbose, new LoggerProviderConfiguration() { Enabled = true });
                optionsBuilder.UseLoggerFactory(loggerFactory);
            };
        }
    }
}