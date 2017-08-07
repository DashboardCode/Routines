using Microsoft.VisualStudio.TestTools.UnitTesting;
using DashboardCode.AdminkaV1.DomTest;
using DashboardCode.Routines;
using DashboardCode.Routines.Json;
using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace DashboardCode.AdminkaV1.Injected.SqlServer.Test
{
    [TestClass]
    public class StorageJsonTest
    {
        [TestMethod]
        public void TestStorageJson2()
        {
            var databaseName = "AdminkaV1_1";
            TestIsland.Reset(databaseName);
            var userContext1 = new UserContext("UnitTest");
            var configuration = ZoningSharedSourceManager.GetConfiguration();
            var adminka = new AdminkaRoutineHandler(
                new MemberTag(this),
                userContext1,
                configuration, new { input = "Input text" });
            adminka.Handle((routine, dataAccess) =>
            {
                var db = dataAccess.CreateAdminkaDbContext();
                var list = db.ParentRecords
                    .Include(e => e.ParentRecordHierarchyRecordMap)
                    //.ThenInclude(e => e.HierarchyRecordId)
                    .ToList();
            });
        }

        [TestMethod]
        public void TestStorageJson()
        {
            var databaseName = "AdminkaV1_1";
            TestIsland.Reset(databaseName);

            var userContext1 = new UserContext("UnitTest");
            var configuration = ZoningSharedSourceManager.GetConfiguration();
            var adminka = new AdminkaRoutineHandler(
                new MemberTag(this),
                userContext1,
                configuration, new { input = "Input text" });
            adminka.Handle((routine, dataAccess) =>
            {
                dataAccess.Handle<ParentRecord>(
                    (repository, storage) => {
                        
                        Include<ParentRecord> include = chain => chain.IncludeAll(e => e.ParentRecordHierarchyRecordMap);
                        var lists = repository.List(include);
                        
                        var serailizeInclude = repository.AppendModelFields(include);
                        //var t = serailizeInclude.ListXPaths();
                        //var formatter = serailizeInclude.ComposeFormatter();
                        //var item = lists.First();
                        //var json = formatter(item);
                    }
                );
            });
        }
    }
}
