using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Linq;
using Vse.AdminkaV1.DomTest;
using Vse.Routines;

namespace Vse.AdminkaV1.Injected.Test
{
    [TestClass]
    public class SerializationWithRecursionTest
    {
        public SerializationWithRecursionTest()
        {
            TestIsland.Reset();
        }

        [TestMethod]
        public void TestDetach()
        {
            var userContext = new UserContext("UnitTest");
            var routine = new AdminkaRoutine(new RoutineTag(this), userContext, new { input = "Input text" });

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
                    var parent = repository.GetQueryable(include).First(e=>e.FieldA== "1_A");
                    repository.Detach(parent, include);
                    InjectedManager.SerializeToJson(parent);
                });
            });
        }

        [TestMethod]
        public void TestSerializtionRecursion()
        {
            var userContext = new UserContext("UnitTest");
            var routine = new AdminkaRoutine(new RoutineTag(this), userContext, new { input = "Input text" });
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
        public void TestProblematicDetachUsage()
        {
            var userContext = new UserContext("UnitTest");
            var routine = new AdminkaRoutine(new RoutineTag(this), userContext, new { input = "Input text" });
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
                throw new ApplicationException("Detach error");
        }

        [TestMethod]
        public void TestXmlSerializeAndDesirialize()
        {
            var userContext = new UserContext("UnitTest");
            var routine = new AdminkaRoutine(new RoutineTag(this), userContext, new { input = "Input text" });
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
            var cloned =  MemberExpressionExtensions.Clone(record, include, MemberExpressionExtensions.SystemTypes);
            if (cloned.ChildRecords == null || cloned.ChildRecords.Count == 0)
                throw new ApplicationException("Clone error");
            var xml = InjectedManager.SerializeToXml(cloned, include);
            var o = InjectedManager.DeserializeXml(xml, include);
            if (o == null)
                throw new ApplicationException("Serialize error");
        }
    }
}
