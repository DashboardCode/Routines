#if NETCOREAPP1_1
    using Xunit;
#else
using Microsoft.VisualStudio.TestTools.UnitTesting;
#endif 
using Vse.AdminkaV1.DomTest;
using Vse.Routines;
using Vse.Routines.Storage;

namespace Vse.AdminkaV1.Injected.Test
{
#if !NETCOREAPP1_1
    [TestClass]
#endif
    public class StorageModelErrorTest
    {
#if NETCOREAPP1_1
        ConfigurationNETStandard Configuration = new ConfigurationNETStandard();
#else
        ConfigurationNETFramework Configuration = new ConfigurationNETFramework();
#endif

        public StorageModelErrorTest()
        {
            TestIsland.Clear();
        }
#if NETCOREAPP1_1
        [Fact]
#else
        [TestMethod]
#endif
        public void TestDatabaseFieldRequiredError() 
        {
            var userContext = new UserContext("UnitTest");
            var routine = new AdminkaRoutine(new RoutineTag(this), userContext, Configuration, new { input = "Input text" });
            routine.Handle((state, dataAccess) =>
            {
                var repositoryHandler = dataAccess.CreateRepositoryHandler<ParentRecord>();
                repositoryHandler.Handle((repository, storage) =>
                {
                    var t0 = new ParentRecord() { };
                    var storageError = storage.Handle(batch => batch.Add(t0));
                    storageError.Assert(1, "", "ID or alternate id has no value", "Case 1");
                });
            });

            routine.Handle((state, dataAccess) =>
            {
                var repositoryHandler = dataAccess.CreateRepositoryHandler<ParentRecord>();
                repositoryHandler.Handle((repository, storage) =>
                {
                    var t0 = new ParentRecord() { FieldCA="1", FieldCB1 = "2", FieldCB2 = "3" };
                    var storageError = storage.Handle(batch =>batch.Add(t0));
                    storageError.Assert(1, "FieldA", "Is required!", "Case 2");
                });
            });

            var parentRecord = new ParentRecord()
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
                var repositoryHandler = dataAccess.CreateRepositoryHandler<ParentRecord>();
                repositoryHandler.Handle((repository, storage) =>
                {
                    var storageError = storage.Handle(batch => batch.Add(parentRecord));
                    storageError.Desert("Add failed 1");
                });
            });

            routine.Handle((state, dataAccess) =>
            {
                var repositoryHandler = dataAccess.CreateRepositoryHandler<ParentRecord>();
                repositoryHandler.Handle((repository, storage) =>
                {
                    var t2 = new ParentRecord() { FieldA = "A", FieldB1 = "Ba", FieldB2 = "Ca",
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
                var repositoryHandler = dataAccess.CreateRepositoryHandler<ParentRecord>();
                repositoryHandler.Handle((repository, storage) =>
                {
                    var t2 = new ParentRecord()
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
                var repositoryHandler = dataAccess.CreateRepositoryHandler<ParentRecord>();
                repositoryHandler.Handle((repository, storage) =>
                {
                    var t2 = new ParentRecord() {
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
                var repositoryHandler = dataAccess.CreateRepositoryHandler<ParentRecord>();
                repositoryHandler.Handle((repository, storage) =>
                {
                    var t2 = new ParentRecord() {
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

            var typeRecord = new TypeRecord()
            {
                TestTypeRecordId="0000",
                TypeRecordName="TestType"
            };

            routine.Handle((state, dataAccess) =>
            {
                var repositoryHandler = dataAccess.CreateRepositoryHandler<TypeRecord>();
                repositoryHandler.Handle((repository, storage) =>
                {
                    storage.Handle(batch => batch.Add(typeRecord)).Desert("Can't add TestTypeRecord"); 
                });
            });

            var childRecord = new ChildRecord()
            {
                ParentRecordId = parentRecord.ParentRecordId,
                TypeRecordId = typeRecord.TestTypeRecordId,
                XmlField1 = "notxml",
                XmlField2 = "notxml"
            }; 

            routine.Handle((state, dataAccess) =>
            {
                var repositoryHandler = dataAccess.CreateRepositoryHandler<ChildRecord>();
                repositoryHandler.Handle((repository, storage) =>
                {
                    storage.Handle(batch => batch.Add(childRecord)).Desert("Can't add TestChildRecord");
                });
            });

            routine.Handle((state, dataAccess) =>
            {
                var t0 = new TypeRecord()
                {
                    TestTypeRecordId = "0000",
                    TypeRecordName = "TestType2"
                };
                var repositoryHandler = dataAccess.CreateRepositoryHandler<TypeRecord>();
                repositoryHandler.Handle((repository, storage) =>
                {
                    var storageError = storage.Handle(batch => batch.Add(t0));
                    storageError.Assert(1, nameof(TypeRecord.TestTypeRecordId), null, "Case 7");
                });
            });

            // string that exceed its length limit
            routine.Handle((state, dataAccess) =>
            {
                var t0 = new TypeRecord()
                {
                    TestTypeRecordId = "0001x",
                    TypeRecordName = "TestType2"
                };
                var repositoryHandler = dataAccess.CreateRepositoryHandler<TypeRecord>();
                repositoryHandler.Handle((repository, storage) =>
                {
                    var storageError = storage.Handle(batch => batch.Add(t0));
                    storageError.Assert(1, "", null, "Case 8");
                });
            });

            // check constraint on INSERT
            routine.Handle((state, dataAccess) =>
            {
                var t0 = new TypeRecord()
                {
                    TestTypeRecordId = "0001",
                    TypeRecordName = "TestType2,,.."
                };
                var repositoryHandler = dataAccess.CreateRepositoryHandler<TypeRecord>();
                repositoryHandler.Handle((repository, storage) =>
                {
                    var storageError = storage.Handle(batch => batch.Add(t0));
                    storageError.Assert(1, nameof(TypeRecord.TypeRecordName), null, "Case 9");
                });
            });

            // check constraint on UPDATE
            routine.Handle((state, dataAccess) =>
            {
                var repositoryHandler = dataAccess.CreateRepositoryHandler<TypeRecord>();
                repositoryHandler.Handle((repository, storage) =>
                {
                    var t1 = repository.Find(e => e.TestTypeRecordId == "0000");
                    t1.TypeRecordName = "TestType2,,..";
                    var storageError = storage.Handle(batch => batch.Modify(t1));
                    storageError.Assert(1, nameof(TypeRecord.TypeRecordName), null, "Case 10");
                });
            });

            // check NULL on UPDATE
            routine.Handle((state, dataAccess) =>
            {
                var repositoryHandler = dataAccess.CreateRepositoryHandler<TypeRecord>();
                repositoryHandler.Handle((repository, storage) =>
                {
                    var t1 = repository.Find(e => e.TestTypeRecordId == "0000");
                    t1.TypeRecordName = null;
                    var storageError = storage.Handle(batch => batch.Modify(t1));
                    storageError.Assert(1, nameof(TypeRecord.TypeRecordName), null, "Case 11");
                });
            });
        }
    }
}
