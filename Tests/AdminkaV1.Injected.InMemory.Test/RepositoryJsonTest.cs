﻿using Microsoft.VisualStudio.TestTools.UnitTesting;
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
            var routine = new AdminkaInMemoryTestRoutine(new MemberTag(this), new {}, readonlyDatabaseName);
            routine.HandleOrmFactory(ormHandlersFactory =>
            {
                var ormHandler = ormHandlersFactory.Create<ParentRecord>();
                ormHandler.Handle(
                    (repository, storage, schemaAdapter) =>
                    {
                        Include<ParentRecord> include = chain => chain
                            .IncludeAll(e => e.ParentRecordHierarchyRecordMap)
                            .ThenInclude(e => e.HierarchyRecordId);

                        var navigationInclude = schemaAdapter.ExtractNavigations(include);
                        var lists = repository.List(navigationInclude);

                        var serailizeInclude = schemaAdapter.ExtractNavigationsAppendKeyLeafs(include);
                        var formatter = serailizeInclude.ComposeEnumerableFormatter();
                        var json = formatter(lists);
                    }
                );
            });
        }
    }
}