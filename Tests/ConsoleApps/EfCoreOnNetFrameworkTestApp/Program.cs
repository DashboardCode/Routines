using System;
using System.Linq;
using System.Collections.Generic;
using System.Configuration;
using Microsoft.EntityFrameworkCore;

using DashboardCode.Routines.Storage.EfModelTest;
using DashboardCode.Routines.Storage.EfCore;
using DashboardCode.Routines.Storage.EfCore.Relational;
using DashboardCode.Routines.Storage.EfModelTest.EfCore;
using System.Threading.Tasks;

namespace DashboardCode.EfCore.NETFramework.Sandbox
{
    class Program
    {
        static void Main(string[] args)
        {
            bool inMemory = false;
            var connectionString = ConfigurationManager.ConnectionStrings["EfCoreTest"].ConnectionString;

            Do(inMemory, connectionString);
            ParallelTest(connectionString, inMemory);
            GC.Collect();
            GC.WaitForPendingFinalizers();
            ParallelTest(connectionString, inMemory);
            GC.Collect();
            GC.WaitForPendingFinalizers();
        }

        private static void Do(bool inMemory, string connectionString)
        {
            var messages = new List<string>();
            Action<string> verbose = (text) => {
                messages.Add(text);
                Console.WriteLine(text);
                Console.WriteLine();
            };

            Console.WriteLine("Check connection string:");
            Console.WriteLine(connectionString);

            using (var dbContext = new MyDbContext(BuildOptionsBuilder(connectionString, inMemory)))
            {
                if (!inMemory)
                {
                    TestIsland.Clear(new AdoBatch(dbContext));
                }
                TestIsland.Reset(new OrmStorage(dbContext, null, (o) => { }));
            }

            int id = 0;
            using (var dbContext = new MyDbContext(BuildOptionsBuilder(connectionString, inMemory), null))
            {
                var parentRecord = dbContext.ParentRecords
                   .Include(e => e.ParentRecordHierarchyRecordMap)
                   .ThenInclude(e => e.HierarchyRecord).First(e => e.FieldA == "1_A");
                id = parentRecord.ParentRecordId;
                //StraightEfTests.TestHierarchy(dbContext);
            }
            
            using (var dbContext = new MyDbContext(BuildOptionsBuilder(connectionString, inMemory), null))
            {
                var storage = new OrmStorage<ParentRecord>(dbContext,
                    (ex) => ExceptionExtensions.Analyze(ex, null),
                    (o) => {; });
                var parentRecord = new ParentRecord { ParentRecordId = id };

                storage.Handle((b) =>
                {
                    b.Modify(parentRecord);
                });
            }

        }

        private static void ParallelTest(string connectionString, bool inMemory)
        {
            var messages = new List<string>();
            Action<string> verbose = (text) => {
                messages.Add(text);
                Console.WriteLine(text);
                Console.WriteLine();
            };

            var seria = Enumerable.Range(1, 64).ToList();
            Parallel.ForEach(seria, t =>
            {
                using (var dbContext = new MyDbContext(BuildOptionsBuilder(connectionString, inMemory), verbose))
                {

                    var parentRecord = dbContext.ParentRecords
                       .Include(e => e.ParentRecordHierarchyRecordMap)
                       .ThenInclude(e => e.HierarchyRecord).First(e => e.FieldA == "1_A");
                    //StraightEfTests.TestHierarchy(dbContext);
                }
            });
        }

        private static Action<DbContextOptionsBuilder> BuildOptionsBuilder(string connectionString, bool inMemory)
        {
            return (optionsBuilder) =>
            {
                if (inMemory)
                    optionsBuilder.UseInMemoryDatabase(
                      "EfCore_NETFramework_Sandbox"
                    );
                else
                    //Assembly.GetAssembly(typeof(Program))
                    optionsBuilder.UseSqlServer(
                            connectionString,
                            sqlServerDbContextOptionsBuilder => sqlServerDbContextOptionsBuilder.MigrationsAssembly("EfCore.NETFramework.Sandbox")
                            );
            };
        }
    }
}