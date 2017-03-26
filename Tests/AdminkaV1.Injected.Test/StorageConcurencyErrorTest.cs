#if NETCOREAPP1_1
    using Xunit;
#else
using Microsoft.VisualStudio.TestTools.UnitTesting;
#endif 
using Vse.AdminkaV1.DomTest;
using Vse.Routines;
using Vse.Routines.Storage;
using System;

namespace Vse.AdminkaV1.Injected.Test
{
#if !NETCOREAPP1_1
    [TestClass]
#endif
    public class StorageConcurencyErrorTest
    {
#if NETCOREAPP1_1
        ConfigurationNETStandard Configuration = new ConfigurationNETStandard();
#else
        ConfigurationNETFramework Configuration = new ConfigurationNETFramework();
#endif

        public StorageConcurencyErrorTest()
        {
            TestIsland.Clear();
        }

#if NETCOREAPP1_1
        [Fact]
#else
        [TestMethod]
#endif
        public void TestConcurencyError()
        {
            var userContext = new UserContext("UnitTest");
            var routine = new AdminkaRoutine(new RoutineTag(this), userContext, Configuration, new { input = "Input text" });
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
                        throw new Exception("Test failed: not correct error. Case 1.");
                });
            });
        }
    }
}
