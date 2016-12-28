﻿using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using Vse.AdminkaV1.DomTest;
using Vse.Routines;
using Vse.Routines.Storage;

namespace Vse.AdminkaV1.Injected.Test
{
    [TestClass]
    public class StorageConcurencyErrorTest
    {
        public StorageConcurencyErrorTest()
        {
            TestIsland.Clear();
        }

        [TestMethod]
        public void TestConcurencyError()
        {
            var userContext = new UserContext("UnitTest");
            var routine = new AdminkaRoutine(new RoutineTag(this), userContext, new { input = "Input text" });
            // check constraint on UPDATE
            routine.Handle((state, dataAccess) =>
            {
                var t0 = new TypeRecord()
                {
                    TestTypeRecordId = "0000",
                    TypeRecordName = "TestType"
                };
                var repositoryHandler = dataAccess.CreateRepositoryHandler<TypeRecord>();
                repositoryHandler.Handle((repository, storage) =>
                {
                    var storageError = storage.Handle(batch => batch.Add(t0));
                    storageError.Desert();
                });
            });

            routine.Handle((state, dataAccess) =>
            {
                var t1 = new TypeRecord()
                {
                    TestTypeRecordId   = "0000",
                    TypeRecordName = "TestType2"
                };
                var repositoryHandler = dataAccess.CreateRepositoryHandler<TypeRecord>();
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