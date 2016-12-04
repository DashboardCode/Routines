using Microsoft.VisualStudio.TestTools.UnitTesting;
using Vse.AdminkaV1.DomTest;
using Vse.Routines;
using Vse.Routines.Storage;

namespace Vse.AdminkaV1.Injected.Test
{
    [TestClass]
    public class StorageModelTest
    {
        public StorageModelTest()
        {
            var userContext = new UserContext("UnitTest");
            var routine = new AdminkaRoutine(new RoutineTag(this), userContext, new { input = "Input text" });
            routine.Handle((state, dataAccess) =>
            {
                dataAccess.CreateRepositoryHandler<TestChildRecord>().Handle((repository, storage) =>
                {
                    storage.Handle(batch => {
                        var list = repository.ToList();
                        foreach (var e in list)
                            batch.Remove(e);
                    }).Desert();
                });
                dataAccess.CreateRepositoryHandler<TestParentRecord>().Handle((repository, storage) =>
                {
                    storage.Handle(batch => {
                        var list = repository.ToList();
                        foreach(var e in list)
                            batch.Remove(e);
                    }).Desert();
                });
                dataAccess.CreateRepositoryHandler<TestTypeRecord>().Handle((repository, storage) =>
                {
                    storage.Handle(batch => {
                        var list = repository.ToList();
                        foreach (var e in list)
                            batch.Remove(e);
                    }).Desert();
                });
            });
        }
        [TestMethod]
        public void TestDatabaseFieldRequiredError() 
        {
            var userContext = new UserContext("UnitTest");
            var routine = new AdminkaRoutine(new RoutineTag(this), userContext, new { input = "Input text" });
            routine.Handle((state, dataAccess) =>
            {
                var repositoryHandler = dataAccess.CreateRepositoryHandler<TestParentRecord>();
                repositoryHandler.Handle((repository, storage) =>
                {
                    var t0 = new TestParentRecord() { };
                    var storageError = storage.Handle(batch => batch.Add(t0));
                    storageError.Assert(1, "", "ID or alternate id has no value", "Case 1");
                });
            });

            routine.Handle((state, dataAccess) =>
            {
                var repositoryHandler = dataAccess.CreateRepositoryHandler<TestParentRecord>();
                repositoryHandler.Handle((repository, storage) =>
                {
                    var t0 = new TestParentRecord() { FieldCA="1", FieldCB1 = "2", FieldCB2 = "3" };
                    var storageError = storage.Handle(batch =>batch.Add(t0));
                    storageError.Assert(1, "FieldA", "Is required!", "Case 2");
                });
            });

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
                {
                    var storageError = storage.Handle(batch => batch.Add(parentRecord));
                    storageError.Desert("Add failed 1");
                });
            });

            routine.Handle((state, dataAccess) =>
            {
                var repositoryHandler = dataAccess.CreateRepositoryHandler<TestParentRecord>();
                repositoryHandler.Handle((repository, storage) =>
                {
                    var t2 = new TestParentRecord() { FieldA = "A", FieldB1 = "Ba", FieldB2 = "Ca",
                        FieldCA = "1a",
                        FieldCB1 = "2a",
                        FieldCB2 = "3a"
                    };
                    var storageError = storage.Handle(batch => batch.Add(t2));
                    storageError.Assert(1, "FieldA", "Allready used", "Case 3.");
                });
            });

            routine.Handle((state, dataAccess) =>
            {
                var repositoryHandler = dataAccess.CreateRepositoryHandler<TestParentRecord>();
                repositoryHandler.Handle((repository, storage) =>
                {
                    var t2 = new TestParentRecord()
                    {
                        FieldA =  "Aa",
                        FieldB1 = "Ba",
                        FieldB2 = "Ca",
                        FieldCA =  "1a",
                        FieldCB1 = "2",
                        FieldCB2 = "3"
                    };
                    var storageError = storage.Handle(batch => batch.Add(t2));
                    storageError.Assert(2, new[] { "FieldCB1", "FieldCB2" }, null, "Case 4.");
                });
            });

            routine.Handle((state, dataAccess) =>
            {
                var repositoryHandler = dataAccess.CreateRepositoryHandler<TestParentRecord>();
                repositoryHandler.Handle((repository, storage) =>
                {
                    var t2 = new TestParentRecord() {
                        FieldA = "Aa",
                        FieldB1 = "B",
                        FieldB2 = "C",
                        FieldCA = "1a",
                        FieldCB1 = "2a",
                        FieldCB2 = "3a"
                    };
                    var storageError = storage.Handle(batch => batch.Add(t2));
                    storageError.Assert(2, new[] { "FieldB1", "FieldB2" }, null, "Case 5.");
                });
            });

            routine.Handle((state, dataAccess) =>
            {
                var repositoryHandler = dataAccess.CreateRepositoryHandler<TestParentRecord>();
                repositoryHandler.Handle((repository, storage) =>
                {
                    var t2 = new TestParentRecord() {
                        FieldA = "Aa",
                        FieldB1 = "Ba",
                        FieldB2 = "Ca",
                        FieldCA = "1",
                        FieldCB1 = "2a",
                        FieldCB2 = "3a"
                    };
                    var storageError = storage.Handle(batch => batch.Add(t2));
                    storageError.Assert(1, "FieldCA", null, "Case 6.");
                });
            });

            var typeRecord = new TestTypeRecord()
            {
                TestTypeRecordId="0000",
                TestTypeRecordName="TestType"
            };

            routine.Handle((state, dataAccess) =>
            {
                var repositoryHandler = dataAccess.CreateRepositoryHandler<TestTypeRecord>();
                repositoryHandler.Handle((repository, storage) =>
                {
                    storage.Handle(batch => batch.Add(typeRecord)).Desert("Can't add TestTypeRecord"); 
                });
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
                {
                    storage.Handle(batch => batch.Add(childRecord)).Desert("Can't add TestChildRecord");
                });
            });

            routine.Handle((state, dataAccess) =>
            {
                var t0 = new TestTypeRecord()
                {
                    TestTypeRecordId = "0000",
                    TestTypeRecordName = "TestType2"
                };
                var repositoryHandler = dataAccess.CreateRepositoryHandler<TestTypeRecord>();
                repositoryHandler.Handle((repository, storage) =>
                {
                    var storageError = storage.Handle(batch => batch.Add(t0));
                    storageError.Assert(1, nameof(TestTypeRecord.TestTypeRecordId), null, "Case 7");
                });
            });

            // string that exceed its length limit
            routine.Handle((state, dataAccess) =>
            {
                var t0 = new TestTypeRecord()
                {
                    TestTypeRecordId = "0001x",
                    TestTypeRecordName = "TestType2"
                };
                var repositoryHandler = dataAccess.CreateRepositoryHandler<TestTypeRecord>();
                repositoryHandler.Handle((repository, storage) =>
                {
                    var storageError = storage.Handle(batch => batch.Add(t0));
                    storageError.Assert(1, "", null, "Case 8");
                });
            });

            // check constraint on INSERT
            routine.Handle((state, dataAccess) =>
            {
                var t0 = new TestTypeRecord()
                {
                    TestTypeRecordId = "0001",
                    TestTypeRecordName = "TestType2,,.."
                };
                var repositoryHandler = dataAccess.CreateRepositoryHandler<TestTypeRecord>();
                repositoryHandler.Handle((repository, storage) =>
                {
                    var storageError = storage.Handle(batch => batch.Add(t0));
                    storageError.Assert(1, nameof(TestTypeRecord.TestTypeRecordName), null, "Case 9");
                });
            });

            // check constraint on UPDATE
            routine.Handle((state, dataAccess) =>
            {
                var repositoryHandler = dataAccess.CreateRepositoryHandler<TestTypeRecord>();
                repositoryHandler.Handle((repository, storage) =>
                {
                    var t1 = repository.Find(e => e.TestTypeRecordId == "0000");
                    t1.TestTypeRecordName = "TestType2,,..";
                    var storageError = storage.Handle(batch => batch.Modify(t1));
                    storageError.Assert(1, nameof(TestTypeRecord.TestTypeRecordName), null, "Case 10");
                });
            });

            // check NULL on UPDATE
            routine.Handle((state, dataAccess) =>
            {
                var repositoryHandler = dataAccess.CreateRepositoryHandler<TestTypeRecord>();
                repositoryHandler.Handle((repository, storage) =>
                {
                    var t1 = repository.Find(e => e.TestTypeRecordId == "0000");
                    t1.TestTypeRecordName = null;
                    var storageError = storage.Handle(batch => batch.Modify(t1));
                    storageError.Assert(1, nameof(TestTypeRecord.TestTypeRecordName), null, "Case 11");
                });
            });
        }
    }
}
