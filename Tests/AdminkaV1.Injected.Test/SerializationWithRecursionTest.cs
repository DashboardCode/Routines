using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using Vse.AdminkaV1.DomTest;
using Vse.Routines;
using Vse.Routines.Storage;

namespace Vse.AdminkaV1.Injected.Test
{
    [TestClass]
    public class SerializationWithRecursionTest
    {
        public SerializationWithRecursionTest()
        {
            var userContext = new UserContext("UnitTest");
            var routine = new AdminkaRoutine(new RoutineTag(this), userContext, new { input = "Input text" });
            routine.Handle((state, dataAccess) =>
            {
                dataAccess.CreateRepositoryHandler<TestChildRecord>().Handle((repository, storage) =>
                {
                    storage.Handle(batch =>
                    {
                        var list = repository.ToList();
                        foreach (var e in list)
                            batch.Remove(e);
                    }).Desert();
                });
                dataAccess.CreateRepositoryHandler<TestParentRecord>().Handle((repository, storage) =>
                {
                    storage.Handle(batch =>
                    {
                        var list = repository.ToList();
                        foreach (var e in list)
                            batch.Remove(e);
                    }).Desert();
                });
                dataAccess.CreateRepositoryHandler<TestTypeRecord>().Handle((repository, storage) =>
                {
                    storage.Handle(batch =>
                    {
                        var list = repository.ToList();
                        foreach (var e in list)
                            batch.Remove(e);
                    }).Desert();
                });
            });

            var typeRecord = new TestTypeRecord()
            {
                TestTypeRecordId = "0000",
                TestTypeRecordName = "TestType"
            };

            var parentRecord = new TestParentRecord()
            {
                FieldA = "A",
                FieldB1 = "B",
                FieldB2 = "C",
                FieldCA = "1",
                FieldCB1 = "2",
                FieldCB2 = "3"
            };

            routine.Handle((state, dataAccess) =>
            {
                var repositoryHandler = dataAccess.CreateRepositoryHandler<TestParentRecord>();
                repositoryHandler.Handle((repository, storage) =>
                    storage.Handle(batch => batch.Add(parentRecord))
                        .Desert("Can't add TestParentRecord")
                );
            });

            routine.Handle((state, dataAccess) =>
            {
                var repositoryHandler = dataAccess.CreateRepositoryHandler<TestTypeRecord>();
                repositoryHandler.Handle((repository, storage) =>
                    storage.Handle(batch => batch.Add(typeRecord)).Desert("Can't add TestTypeRecord")
                );
            });

            var childRecord = new TestChildRecord()
            {
                TestParentRecordId = parentRecord.TestParentRecordId,
                TestTypeRecordId = typeRecord.TestTypeRecordId,
                XmlField1 = "notxml",
                XmlField2 = "notxml"
            };

            routine.Handle((state, dataAccess) =>
            {
                var repositoryHandler = dataAccess.CreateRepositoryHandler<TestChildRecord>();
                repositoryHandler.Handle((repository, storage) =>
                    storage.Handle(batch => batch.Add(childRecord)).Desert("Can't add TestChildRecord")
                );
            });
        }

        [TestMethod]
        public void TestSerializtionRecursion()
        {
            var userContext = new UserContext("UnitTest");
            var routine = new AdminkaRoutine(new RoutineTag(this), userContext, new { input = "Input text" });
            var record = routine.Handle((state, dataAccess) =>
            {
                Include<TestTypeRecord> include = includable =>
                       includable.IncludeAll(y => y.TestChildRecords)
                       .ThenInclude(y => y.TestTypeRecord);

                var repositoryHandler = dataAccess.CreateRepositoryHandler<TestTypeRecord>();
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
            Include<TestTypeRecord> include = includable =>
                       includable.IncludeAll(y => y.TestChildRecords)
                       .ThenInclude(y => y.TestTypeRecord);
            var record = routine.Handle((state, dataAccess) =>
            {
                var repositoryHandler = dataAccess.CreateRepositoryHandler<TestTypeRecord>();
                return repositoryHandler.Handle((repository, storage) =>
                {
                    var entity = repository.Find(e => e.TestTypeRecordId == "0000", include);
                    repository.Detach(entity, include);
                    return entity;
                });
            });
            if (record.TestChildRecords != null) // from first sight TestChildRecords should be included, but .ThenInclude(y => y.TestTypeRecord) returns the same object there it is pointed that TestChildRecords should be not included
                throw new ApplicationException("Detach error");
        }

        [TestMethod]
        public void TestXmlSerializeAndDesirialize()
        {
            var userContext = new UserContext("UnitTest");
            var routine = new AdminkaRoutine(new RoutineTag(this), userContext, new { input = "Input text" });
            Include<TestTypeRecord> include = includable =>
                       includable.IncludeAll(y => y.TestChildRecords)
                       .ThenInclude(y => y.TestTypeRecord);
            var record = routine.Handle((state, dataAccess) =>
            {
                var repositoryHandler = dataAccess.CreateRepositoryHandler<TestTypeRecord>();
                return repositoryHandler.Handle((repository, storage) =>
                {
                    var entity = repository.Find(e => e.TestTypeRecordId == "0000", include);
                    return entity;
                });
            });
            var cloned =  MemberExpressionExtensions.Clone(record, include, MemberExpressionExtensions.SystemTypes);
            if (cloned.TestChildRecords == null || cloned.TestChildRecords.Count == 0)
                throw new ApplicationException("Clone error");
            var xml = InjectedManager.SerializeToXml(cloned, include);
            var o = InjectedManager.DeserializeXml(xml, include);
            if (o == null)
                throw new ApplicationException("Serialize error");

        }
    }
}
