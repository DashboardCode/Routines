﻿using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using DashboardCode.Routines;
using DashboardCode.Routines.Json;
using DashboardCode.AdminkaV1.TestDom;

namespace DashboardCode.AdminkaV1.Injected.InMemory.Test
{
    [TestClass]
    public class RepoistoryJsonTest : RepositoryTestBase
    {
        [TestMethod]
        public void TestStorageJson()
        {
            var logger = new List<string>();
            var routine = new AdminkaInMemoryTestRoutine(logger, hasVerboseLoggingPrivilege: true, new MemberTag(this), new { }, readonlyDatabaseName);
            routine.Handle( (container,closure) => container.ResolveTestDomDbContextHandler().HandleOrmFactory(ormHandlersFactory =>
            {
                var ormHandler = ormHandlersFactory.Create<ParentRecord>();
                ormHandler.Handle(
                    (repository, storage, schemaAdapter) =>
                    {
                        static void include(Chain<ParentRecord> chain) => chain
                            .IncludeAll(e => e.ParentRecordHierarchyRecordMap)
                            .ThenInclude(e => e.HierarchyRecordId);

                        var navigationInclude = schemaAdapter.ExtractNavigations(include);
                        var lists = repository.List(navigationInclude);

                        var serailizeInclude = schemaAdapter.ExtractNavigationsAppendKeyLeafs(include);
                        var formatter = serailizeInclude.ComposeEnumerableFormatter();
                        var json = formatter(lists);
                    }
                );
            }));
        }
    }
}