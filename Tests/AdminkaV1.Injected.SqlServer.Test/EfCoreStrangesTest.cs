﻿using Microsoft.VisualStudio.TestTools.UnitTesting;

using System.Linq;
using DashboardCode.Routines.Storage;
using DashboardCode.Routines;
using DashboardCode.AdminkaV1.DomTest;
using System;


namespace DashboardCode.AdminkaV1.Injected.SqlServer.Test
{
    [TestClass]
    public class EfCoreStrangesTest
    {

        public EfCoreStrangesTest()
        {
            TestIsland.Reset();
        }

        [TestMethod]
        public void EfCoreTestStoreUpdateRelationsErrorTracking()
        {
            var userContext = new UserContext("UnitTest");
            var routine = new AdminkaRoutine(new MemberTag(this), userContext, ZoningSharedSourceManager.GetConfiguration(), new { input = "Input text" });
            Include<ParentRecord> includes
                = includable => includable
                    .IncludeAll(y => y.ParentRecordHierarchyRecordMap)
                        .ThenInclude(y => y.HierarchyRecord);
            routine.Handle((state, dataAccess) =>
            {
                var rh = dataAccess.CreateRepositoryHandler<ParentRecord>(true);
                rh.Handle(
                    (repository,batch) =>
                    {
                        var parentRecord = repository.Find(e => e.FieldA == "1_A", includes);
                        var count1 = parentRecord.ParentRecordHierarchyRecordMap.Count(); // 5
                        var only2 = parentRecord.ParentRecordHierarchyRecordMap.Take(2);
                        var count2 = only2.Count(); // 2
                        repository.Sprout<ParentRecordHierarchyRecord>().Detach(only2, (i)=>i.Include(e=>e.RowVersion));

                        batch.Handle(
                            (storage) =>
                            {
                                storage.UpdateRelations(
                                    parentRecord,
                                    e => e.ParentRecordHierarchyRecordMap,
                                    only2,
                                    (e1, e2) => e1.HierarchyRecordId == e2.HierarchyRecordId);
                            });
                        
                        var parentRecord2 = repository.Find(e => e.FieldA == "1_A", includes);
                        var count3 = parentRecord2.ParentRecordHierarchyRecordMap.Count();
                        // count3 should be 2 but...
                        if (count3!=4)
                        {
                            //it contains 4 elements (2 correct elements but they are included twice)
                            throw new Exception();
                        }
                    }
               );
            });
        }
    }

    [TestClass]
    public class EfCoreStrangesTest2
    {
        public EfCoreStrangesTest2()
        {
            TestIsland.Reset();
        }

        [TestMethod]
        public void EfCoreTestStoreUpdateRelationsErrorNoTracking()
        {
            var userContext = new UserContext("UnitTest");
            var routine = new AdminkaRoutine(new MemberTag(this), userContext, ZoningSharedSourceManager.GetConfiguration(), new { input = "Input text" });
            Include<ParentRecord> includes
                = includable => includable
                    .IncludeAll(y => y.ParentRecordHierarchyRecordMap)
                        .ThenInclude(y => y.HierarchyRecord);
            routine.Handle((state, dataAccess) =>
            {
                var rh = dataAccess.CreateRepositoryHandler<ParentRecord>(false);
                rh.Handle(
                    (repository, batch) =>
                    {
                        var parentRecord = repository.Find(e => e.FieldA == "1_A", includes);
                        var count1 = parentRecord.ParentRecordHierarchyRecordMap.Count(); // 5
                        var only2 = parentRecord.ParentRecordHierarchyRecordMap.Take(2);
                        var count2 = only2.Count(); // 2
                        repository.Sprout<ParentRecordHierarchyRecord>().Detach(only2, (i) => i.Include(e => e.RowVersion));

                        batch.Handle(
                            (storage) =>
                            {
                                storage.UpdateRelations(
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