using System;
using System.Linq;
using System.Collections.Generic;
using System.Configuration;
using Microsoft.EntityFrameworkCore;

using DashboardCode.Routines.Storage.EfModelTest;
using DashboardCode.Routines.Storage.EfCore;
using DashboardCode.Routines.Storage.EfCore.Relational;
using DashboardCode.Routines.Storage.EfModelTest.EfCore;

namespace DashboardCode.EfCore.NETFramework.Sandbox
{
    class Program
    {
        static void Main(string[] args)
        {
            Do();
            GC.Collect();
            GC.WaitForPendingFinalizers();
        }

        private static void Do()
        {
            var messages = new List<string>();
            Action<string> verbose = (text) => {
                messages.Add(text);
                Console.WriteLine(text);
                Console.WriteLine();
            };

            bool inMemory = false;
            var connectionString = ConfigurationManager.ConnectionStrings["EfCoreTest"].ConnectionString;
            Console.WriteLine("Check connection string:");
            Console.WriteLine(connectionString);

            using (var dbContext = new MyDbContext(BuildOptionsBuilder(connectionString, inMemory, verbose)))
            {
                if (!inMemory)
                {
                    TestIsland.Clear(new AdoBatch(dbContext));
                }
                TestIsland.Reset(new OrmStorage(dbContext, null, (o) => { }));
            }

            int id = 0;
            using (var dbContext = new MyDbContext(BuildOptionsBuilder(connectionString, inMemory, verbose)))
            {

                var parentRecord = dbContext.ParentRecords
                   .Include(e => e.ParentRecordHierarchyRecordMap)
                   .ThenInclude(e => e.HierarchyRecord).First(e => e.FieldA == "1_A");
                id = parentRecord.ParentRecordId;
                //StraightEfTests.TestHierarchy(dbContext);
            }
            
            //MyDbContext.AddLoggerProvider(BuildOptionsBuilder(connectionString, inMemory), loggerProvider);

            using (var dbContext = new MyDbContext(BuildOptionsBuilder(connectionString, inMemory, verbose)))
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
            

            #region repeats
            using (var dbContext = new MyDbContext(BuildOptionsBuilder(connectionString, inMemory, verbose)))
            {

                var parentRecord = dbContext.ParentRecords
                   .Include(e => e.ParentRecordHierarchyRecordMap)
                   .ThenInclude(e => e.HierarchyRecord).First(e => e.FieldA == "1_A");
                //StraightEfTests.TestHierarchy(dbContext);
            }
            
            
            using (var dbContext = new MyDbContext(BuildOptionsBuilder(connectionString, inMemory, verbose)))
            {

                var parentRecord = dbContext.ParentRecords
                   .Include(e => e.ParentRecordHierarchyRecordMap)
                   .ThenInclude(e => e.HierarchyRecord).First(e => e.FieldA == "1_A");
                //StraightEfTests.TestHierarchy(dbContext);
            }
            
            
            using (var dbContext = new MyDbContext(BuildOptionsBuilder(connectionString, inMemory, verbose)))
            {

                var parentRecord = dbContext.ParentRecords
                   .Include(e => e.ParentRecordHierarchyRecordMap)
                   .ThenInclude(e => e.HierarchyRecord).First(e => e.FieldA == "1_A");
                //StraightEfTests.TestHierarchy(dbContext);
            }
            
            
            using (var dbContext = new MyDbContext(BuildOptionsBuilder(connectionString, inMemory, verbose)))
            {

                var parentRecord = dbContext.ParentRecords
                   .Include(e => e.ParentRecordHierarchyRecordMap)
                   .ThenInclude(e => e.HierarchyRecord).First(e => e.FieldA == "1_A");
                //StraightEfTests.TestHierarchy(dbContext);
            }
            
            #endregion
        }

        private static Action<DbContextOptionsBuilder<MyDbContext>> BuildOptionsBuilder(string connectionString, bool inMemory, Action<string> verbose=null)
        {
            return (optionsBuilder) =>
            {
                if (verbose != null)
                {
                    var loggerFactory = StatefullLoggerFactoryPool.Instance.Get(verbose, new LoggerProviderConfiguration() { Enabled = true, CommandBuilderOnly=false });
                    optionsBuilder.UseLoggerFactory(loggerFactory);
                }
                if (inMemory)
                    optionsBuilder.UseInMemoryDatabase(
                      "EfCoreTest_InMemory"
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