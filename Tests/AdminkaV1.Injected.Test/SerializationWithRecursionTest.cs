using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Linq;
using DashboardCode.AdminkaV1.DomTest;
using DashboardCode.Routines;

namespace DashboardCode.AdminkaV1.Injected.Test
{
    [TestClass]
    public class SerializationWithRecursionTest
    {
        public SerializationWithRecursionTest()
        {
            TestIsland.Reset();
        }

        [TestMethod]
        public virtual void TestDetach()
        {
            var userContext = new UserContext("UnitTest");
            var routine = new AdminkaRoutine(new MemberGuid(this), userContext, ZoneManager.GetConfiguration(), new { input = "Input text" });

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
                    var parent = repository.MakeQueryable(include).First(e=>e.FieldA== "1_A");
                    repository.Detach(parent, include);
                    InjectedManager.SerializeToJson(parent);
                });
            });
        }

        [TestMethod]
        public virtual void TestSerializtionRecursion()
        {
            var userContext = new UserContext("UnitTest");
            var routine = new AdminkaRoutine(new MemberGuid(this), userContext, ZoneManager.GetConfiguration(), new { input = "Input text" });
            var record = routine.Handle((state, dataAccess) =>
            {
                Include<TypeRecord> include = includable =>
                       includable.IncludeAll(y => y.ChildRecords)
                       .ThenInclude(y => y.TypeRecord);

                var repositoryHandler = dataAccess.CreateRepositoryHandler<TypeRecord>();
                return repositoryHandler.Handle((repository, storage) =>
                {
                    return repository.Find(e => e.TestTypeRecordId == "0000", include);
                });
            });
            var json = InjectedManager.SerializeToJson(record,2,true);
        }

        [TestMethod]
        public virtual void TestProblematicDetachUsage()
        {
            var userContext = new UserContext("UnitTest");
            var routine = new AdminkaRoutine(new MemberGuid(this), userContext, ZoneManager.GetConfiguration(), new { input = "Input text" });
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
        public virtual void TestXmlSerializeAndDesirialize()
        {
            var userContext = new UserContext("UnitTest");
            var routine = new AdminkaRoutine(new MemberGuid(this), userContext, ZoneManager.GetConfiguration(), new { input = "Input text" });
            Include<TypeRecord> include = includable =>
                       includable.IncludeAll(y => y.ChildRecords)
                       .ThenInclude(y => y.TypeRecord);
            var record = routine.Handle((state, dataAccess) =>
            {
                var repositoryHandler = dataAccess.CreateRepositoryHandler<TypeRecord>();
                return repositoryHandler.Handle((repository, storage) =>
                {
                    var entity = repository.Find(e => e.TestTypeRecordId == "0000", include);
                    return entity;
                });
            });
            var cloned =  ObjectExtensions.Clone(record, include, SystemTypesExtensions.SystemTypes);
            if (cloned.ChildRecords == null || cloned.ChildRecords.Count == 0)
                throw new Exception("Clone error");
            var xml = InjectedManager.SerializeToXml(cloned, include);
            var o = InjectedManager.DeserializeXml(xml, include);
            if (o == null)
                throw new Exception("Serialize error");
        }
    }
}
