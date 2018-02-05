using System;
using System.Linq;
using System.Collections.Generic;

using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using DashboardCode.AdminkaV1.TestDom;
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
            var logger = new List<string>();
            var routine = new AdminkaInMemoryTestRoutine(logger, new MemberTag(this), new { }, readonlyDatabaseName);
            routine.HandleDbContext((dbContext, closure) =>
            {
                var list = dbContext.ParentRecords
                    .Include(e => e.ParentRecordHierarchyRecordMap)
                    //.ThenInclude(e => e.HierarchyRecordId)
                    .ToList();
            });
        }

        [TestMethod]
        public virtual void TestProblematicDetachUsage()
        {
            var logger = new List<string>();
            var routine = new AdminkaInMemoryTestRoutine(logger, new MemberTag(this), new { }, readonlyDatabaseName);
            Include<TypeRecord> include = includable =>
                       includable.IncludeAll(y => y.ChildRecords)
                       .ThenInclude(y => y.TypeRecord);
            var record = routine.HandleOrmFactory((ormHandlerFactory) =>
            {
                var repositoryHandler = ormHandlerFactory.Create<TypeRecord>();
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
            var logger = new List<string>();
            var routine = new AdminkaInMemoryTestRoutine(logger, new MemberTag(this), new { }, readonlyDatabaseName);

            Include<ParentRecord> include = includable =>
                       includable
                       .IncludeAll(y => y.ParentRecordHierarchyRecordMap)
                            .ThenInclude(y => y.HierarchyRecord)
                       .IncludeAll(y => y.ChildRecords)
                            .ThenInclude(y => y.TypeRecord);
            routine.HandleOrmFactory((ormHandlerFactory) =>
            {
                var repositoryHandler = ormHandlerFactory.Create<ParentRecord>();
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
