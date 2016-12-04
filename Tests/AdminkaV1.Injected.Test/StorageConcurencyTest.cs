using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using Vse.AdminkaV1.DomTest;
using Vse.Routines;
using Vse.Routines.Storage;

namespace Vse.AdminkaV1.Injected.Test
{
    [TestClass]
    public class StorageConcurencyTest
    {
        public StorageConcurencyTest()
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
        }

        [TestMethod]
        public void TestConcurency()
        {
            var userContext = new UserContext("UnitTest");
            var routine = new AdminkaRoutine(new RoutineTag(this), userContext, new { input = "Input text" });
            // check constraint on UPDATE
            routine.Handle((state, dataAccess) =>
            {
                var t0 = new TestTypeRecord()
                {
                    TestTypeRecordId = "0000",
                    TestTypeRecordName = "TestType"
                };
                var repositoryHandler = dataAccess.CreateRepositoryHandler<TestTypeRecord>();
                repositoryHandler.Handle((repository, storage) =>
                {
                    var storageError = storage.Handle(batch => batch.Add(t0));
                    storageError.Desert();
                });
            });

            routine.Handle((state, dataAccess) =>
            {
                var t1 = new TestTypeRecord()
                {
                    TestTypeRecordId   = "0000",
                    TestTypeRecordName = "TestType2"
                };
                var repositoryHandler = dataAccess.CreateRepositoryHandler<TestTypeRecord>();
                repositoryHandler.Handle((repository, storage) =>
                {
                    var storageError = storage.Handle(batch => batch.Modify(t1));
                    //storageError.Desert();
                    if (storageError.Count() != 1 || !storageError.ContainsLike("", "The record you are attempted to edit is currently being"))
                        throw new ApplicationException("Test failed: not correct error. Case 1.");
                });
            });
        }
    }
}
