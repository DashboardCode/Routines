using System;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using DashboardCode.AdminkaV1.DomTest;
using DashboardCode.Routines;
using DashboardCode.Routines.Json;

namespace DashboardCode.AdminkaV1.Injected.InMemory.Test
{
    [TestClass]
    class RepositoryTest : RepositoryBaseTest
    {
        [TestMethod]
        public void TestRepositoryInMemory()
        {
            var adminka = new AdminkaInMemoryTestRoutine(new MemberTag(this), new { }, readonlyDatabaseName);
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
        public virtual void TestProblematicDetachUsage()
        {
            var routine = new AdminkaInMemoryTestRoutine(new MemberTag(this), new { }, readonlyDatabaseName);
            Include<TypeRecord> include = includable =>
                       includable.IncludeAll(y => y.ChildRecords)
                       .ThenInclude(y => y.TypeRecord);
            var record = routine.Handle((state, dataAccess) =>
            {
                var repositoryHandler = dataAccess.CreateRepositoryHandler<TypeRecord>();
                return repositoryHandler.Handle((repository, storage) =>
                {
                    var entity = repository.Find(e => e.TestTypeRecordId == "0000", include);
                    repository.Detach(entity, include);
                    return entity;
                });
            });
            if (record.ChildRecords != null) // from first sight TestChildRecords should be included, but .ThenInclude(y => y.TestTypeRecord) returns the same object there it is pointed that TestChildRecords should be not included
                throw new Exception("Detach error");
        }

        [TestMethod]
        public virtual void TestDetach()
        {
            var routine = new AdminkaInMemoryTestRoutine(new MemberTag(this), new { }, readonlyDatabaseName);

            Include<ParentRecord> include = includable =>
                       includable
                       .IncludeAll(y => y.ParentRecordHierarchyRecordMap)
                            .ThenInclude(y => y.HierarchyRecord)
                       .IncludeAll(y => y.ChildRecords)
                            .ThenInclude(y => y.TypeRecord);
            routine.Handle((state, dataAccess) =>
            {
                var repositoryHandler = dataAccess.CreateRepositoryHandler<ParentRecord>();
                repositoryHandler.Handle((repository, storage) =>
                {
                    var parent = repository.MakeQueryable(include).First(e => e.FieldA == "1_A");
                    repository.Detach(parent, include);
                    InjectedManager.SerializeToJson(parent);
                });
            });
        }

    }
}
