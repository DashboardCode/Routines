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
        public void EfCoreTestStoreUpdateRelationsErrorTracking()
        {
            var logger = new List<string>();
            var loggingTransientsFactory = InjectedManager.ComposeListMemberLoggerFactory(logger);

            TestManager.Reset();

            var routine = new AdminkaAnonymousRoutineHandler(
                TestManager.ApplicationSettings,
                loggingTransientsFactory,
                hasVerboseLoggingPrivilege: true,
                new MemberTag(this), "UnitTest",
                new { input = "Input text" });

            Include<ParentRecord> includes= (includable) => includable
                    .IncludeAll(y => y.ParentRecordHierarchyRecordMap)
                        .ThenInclude(y => y.HierarchyRecord);
            routine.Handle((container, closure) => container.ResolveTestDomDbContextHandler().HandleOrmFactory((ormHandlerFactory) =>
            {
                var rh = ormHandlerFactory.Create<ParentRecord>(false); // true = not trackable doesn't work with LoadAndModifyRelated scenario
                rh.Handle(
                    (repository, batch) =>
                    {
                        var parentRecord = repository.Find(e => e.FieldA == "1_A", includes);
                        var count1 = parentRecord.ParentRecordHierarchyRecordMap.Count(); // 5
                        var only2 = parentRecord.ParentRecordHierarchyRecordMap.Take(2).ToList();
                        var count2 = only2.Count(); // 2
                        //repository.Clone<ParentRecordHierarchyRecord>().Detach(only2, (i) => i.Include(e => e.RowVersion));
                        var cloned2 = ObjectExtensions.CloneAll<List<ParentRecordHierarchyRecord>, ParentRecordHierarchyRecord>
                           (only2, rule => rule.Include(e => e.RowVersion));

                        batch.Handle(
                            (storage) =>
                            {
                                storage.LoadAndModifyRelated(
                                    
                                    parentRecord,
                                    e => e.ParentRecordHierarchyRecordMap,
                                    cloned2,
                                    (e1, e2) => e1.HierarchyRecordId == e2.HierarchyRecordId);
                            });

                        var parentRecord2 = repository.Find(e => e.FieldA == "1_A", includes);
                        var count3 = parentRecord2.ParentRecordHierarchyRecordMap.Count();
                        // count3 should be 2 but...
                        if (count3 != 2)
                        {
                            //it contains 4 elements (2 correct elements but they are included twice)
                            throw new Exception();
                        }
                    }
               );
            }));
        }
    }
}