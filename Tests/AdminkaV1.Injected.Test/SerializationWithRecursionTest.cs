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
            var json = IoCManager.SerializeObject(record,2,true);
        }
    }
}
