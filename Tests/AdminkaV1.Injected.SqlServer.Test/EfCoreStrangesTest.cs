using System;
using System.Linq;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using DashboardCode.Routines;
using DashboardCode.Routines.Storage;
using DashboardCode.AdminkaV1.TestDom;

namespace DashboardCode.AdminkaV1.Injected.SqlServer.Test
{
    [TestClass]
    public class EfCoreStrangesTest
    {
        [TestMethod]
        public void EfCoreTestStoreUpdateRelationsErrorNoTracking()
        {
            TestIsland.Reset(Guid.NewGuid().ToString());

            var userContext = new UserContext("UnitTest");

            var logger = new List<string>();
            var loggingTransientsFactory = InjectedManager.ComposeListMemberLogger(logger);

            var routine = new AdminkaRoutineHandler(
                InjectedManager.GetConfiguration(),
                InjectedManager.GetConfigurationFactory(),
                loggingTransientsFactory,
                new MemberTag(this), userContext,
                new { input = "Input text" });
            Include<ParentRecord> includes
                = includable => includable
                    .IncludeAll(y => y.ParentRecordHierarchyRecordMap)
                        .ThenInclude(y => y.HierarchyRecord);
            routine.HandleOrmFactory((ormHandlerFactory) =>
            {
                var rh = ormHandlerFactory.Create<ParentRecord>(false);
                rh.Handle(
                    (repository, batch) =>
                    {
                        var parentRecord = repository.Find(e => e.FieldA == "1_A", includes);
                        var count1 = parentRecord.ParentRecordHierarchyRecordMap.Count(); // 5
                        var only2 = parentRecord.ParentRecordHierarchyRecordMap.Take(2);
                        var count2 = only2.Count(); // 2
                        repository.Clone<ParentRecordHierarchyRecord>().Detach(only2, (i) => i.Include(e => e.RowVersion));

                        batch.Handle(
                            (storage) =>
                            {
                                storage.ModifyRelated(
                                    parentRecord,
                                    e => e.ParentRecordHierarchyRecordMap,
                                    only2,
                                    (e1, e2) => e1.HierarchyRecordId == e2.HierarchyRecordId);
                            });

                        var parentRecord2 = repository.Find(e => e.FieldA == "1_A", includes);
                        var count3 = parentRecord2.ParentRecordHierarchyRecordMap.Count();
                        // count3 should be 2 but...
                        if (count3 != 4)
                        {
                            //it contains 4 elements (2 correct elements but they are included twice)
                            throw new Exception();
                        }
                    }
               );
            });
        }

        [TestMethod]
        public void EfCoreTestStoreUpdateRelationsErrorTracking()
        {
            var logger = new List<string>();
            var loggingTransientsFactory = InjectedManager.ComposeListMemberLogger(logger);

            TestIsland.Reset(Guid.NewGuid().ToString());

            var userContext = new UserContext("UnitTest");
            var routine = new AdminkaRoutineHandler(
                InjectedManager.GetConfiguration(),
                InjectedManager.GetConfigurationFactory(),
                loggingTransientsFactory,
                new MemberTag(this), userContext,
                new { input = "Input text" });
            Include<ParentRecord> includes
                = includable => includable
                    .IncludeAll(y => y.ParentRecordHierarchyRecordMap)
                        .ThenInclude(y => y.HierarchyRecord);
            routine.HandleOrmFactory(ormHandlerFactory =>
            {
                var rh = ormHandlerFactory.Create<ParentRecord>(false);
                rh.Handle(
                    (repository, batch) =>
                    {
                        var parentRecord = repository.Find(e => e.FieldA == "1_A", includes);
                        var count1 = parentRecord.ParentRecordHierarchyRecordMap.Count(); // 5
                        var only2 = parentRecord.ParentRecordHierarchyRecordMap.Take(2);
                        var count2 = only2.Count(); // 2
                        repository.Clone<ParentRecordHierarchyRecord>().Detach(only2, (i) => i.Include(e => e.RowVersion));

                        batch.Handle(
                            (storage) =>
                            {
                                storage.ModifyRelated(
                                    parentRecord,
                                    e => e.ParentRecordHierarchyRecordMap,
                                    only2,
                                    (e1, e2) => e1.HierarchyRecordId == e2.HierarchyRecordId);
                            });

                        var parentRecord2 = repository.Find(e => e.FieldA == "1_A", includes);
                        var count3 = parentRecord2.ParentRecordHierarchyRecordMap.Count();
                        // count3 should be 2 but...
                        if (count3 != 4)
                        {
                            //it contains 4 elements (2 correct elements but they are included twice)
                            throw new Exception();
                        }
                    }
               );
            });
        }
    }
}