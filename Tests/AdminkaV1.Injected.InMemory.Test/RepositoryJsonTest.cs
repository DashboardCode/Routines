using Microsoft.VisualStudio.TestTools.UnitTesting;
using DashboardCode.AdminkaV1.TestDom;
using DashboardCode.Routines;
using DashboardCode.Routines.Json;

namespace DashboardCode.AdminkaV1.Injected.InMemory.Test
{
    [TestClass]
    public class RepoistoryJsonTest : RepositoryBaseTest
    {
        [TestMethod]
        public void TestStorageJson()
        {
            var adminka = new AdminkaInMemoryTestRoutine(new MemberTag(this), new {}, readonlyDatabaseName);
            adminka.Handle((routine, dataAccess) =>
            {
                dataAccess.Handle<ParentRecord>(
                    (repository, storage, model) => {
                        Include<ParentRecord> include = chain => chain
                            .IncludeAll(e => e.ParentRecordHierarchyRecordMap)
                            .ThenInclude(e => e.HierarchyRecordId);

                        var navigationInclude = model.ExtractNavigations(include);
                        var lists = repository.List(navigationInclude);

                        var serailizeInclude = model.ExtractNavigationsAppendKeyLeafs(include);
                        var formatter = serailizeInclude.ComposeEnumerableFormatter();
                        var json = formatter(lists);
                    }
                );
            });
        }
    }
}