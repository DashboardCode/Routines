using System;
using System.Collections.Generic;

using DashboardCode.Routines.Storage.Ef6;
using DashboardCode.Routines.Storage.EfModelTest;

namespace DashboardCode.Ef6.Sandbox
{
    public class Program
    {
        static void Main(string[] args)
        {
            var logger = new List<string>();
            Action<string> verbose = (text) =>
            {
                logger.Add(text);
                Console.WriteLine(text);
            };
            var connectionStringName = "Ef6Test";
            using (var dbContext = new MyDbContext(connectionStringName, verbose))
            {
                TestIsland.Clear(new AdoBatch(dbContext));
                TestIsland.Reset(new OrmStorage(dbContext, null, (o) => { }));
            }
            using (var dbContext = new MyDbContext(connectionStringName, verbose))
            {
                StraightEfTests.TestHierarchy(dbContext);
            }
        }
    }
}
