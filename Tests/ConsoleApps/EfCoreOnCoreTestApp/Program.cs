using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

using DashboardCode.Routines.Storage.EfCore;
using DashboardCode.Routines.Storage.EfModelTest;
using DashboardCode.Routines.Storage.EfModelTest.EfCoreTest;

namespace DashboardCode.EfCore.NETCore.Sandbox
{
    class Program
    {
        static void Main(string[] args)
        {
            var databaseName = "MyTest_InMemmory";
            Console.Write($"DatabaseName: {databaseName}");
            using (var dbContext = new MyDbContext(BuildOptionsBuilder(databaseName)))
            {
                TestIsland.Reset(new OrmStorage(dbContext, null, (o)=> { }));
            }
            var loggerProvider = new MyLoggerProvider();
            var messages = new List<string>();
            loggerProvider.Verbose = (text) => {
                messages.Add(text);
                Console.WriteLine(text);
                Console.WriteLine();
            };
            using (var dbContext = new MyDbContext(BuildOptionsBuilder(databaseName), loggerProvider))
            {
                StraightEfTests.TestHierarchy(dbContext);
            }
        }

        private static Action<DbContextOptionsBuilder<MyDbContext>> BuildOptionsBuilder(string databaseName)
        {
            return (optionsBuilder) =>
            {
                optionsBuilder.UseInMemoryDatabase(databaseName);
            };
        }
    }
}