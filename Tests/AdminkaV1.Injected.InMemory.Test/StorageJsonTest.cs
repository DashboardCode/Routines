using Microsoft.VisualStudio.TestTools.UnitTesting;
using DashboardCode.AdminkaV1.DomTest;
using DashboardCode.Routines;
using DashboardCode.Routines.Storage;
using System;

namespace DashboardCode.AdminkaV1.Injected.InMemory.Test
{
    [TestClass]
    public class StorageJsonTest
    {
        [TestMethod]
        public void TestStorageJson()
        {
            var databaseName = "AdminkaV1_1";
            TestIsland.Reset(databaseName);

            var userContext1 = new UserContext("UnitTest");
            var routine1 = new AdminkaRoutine(new MemberTag(this), userContext1, ZoningSharedSourceManager.GetConfiguration(databaseName), new { input = "Input text" });
            routine1.Handle((state, dataAccess) =>
            {
                var repositoryHandler = dataAccess.CreateRepositoryHandler<TypeRecord>();
                repositoryHandler.Handle((repository, storage) =>
                {
                    var lists = repository.List();
                    //var storageError = storage.Handle(batch => batch.Add(t0));
                    //storageError.ThrowIfNotNull();
                });
            });
        }
    }
}
