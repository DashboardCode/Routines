﻿using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using DashboardCode.AdminkaV1.TestDom;
using DashboardCode.Routines;
using DashboardCode.Routines.Storage;

namespace DashboardCode.AdminkaV1.Injected.SqlServer.Test
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
            var routine = new AdminkaRoutineHandler(new MemberTag(this), userContext, ZoningSharedSourceManager.GetConfiguration(), new { input = "Input text" });
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
                    storageError.ThrowIfNotNull();
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
                    if (storageError.Count() != 1 || !storageError.ContainsLike("", "The record you are attempted to edit is currently being"))
                        throw new Exception("Test failed: not correct error. Case 1.");
                });
            });
        }
    }
}
