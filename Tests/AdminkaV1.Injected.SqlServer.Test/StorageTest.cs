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
    public class StorageTest
    {
        public StorageTest()
        {
            TestManager.Reset();
        }

        [TestMethod]
        public void TestStoreManyToMany()
        {
            var logger = new List<string>();
            var loggingTransientsFactory = InjectedManager.ComposeListMemberLoggerFactory(logger);

            var routine = new AdminkaAnonymousRoutineHandler(
                TestManager.ApplicationSettings,
                loggingTransientsFactory,
                hasVerboseLoggingPrivilege: true,
                new MemberTag(this), "UnitTest", new { input = "Input text" });
            int newParentRecordId = 0;
            byte[] newRowVersion = null;
            routine.Handle((container, closure) => container.ResolveTestDomDbContextHandler().HandleOrmFactory((ormHandlerFactory) =>
            {
               var parentRecord = new ParentRecord
               {
                   FieldA = "MMTA",
                   FieldB1 = "MMTB1",
                   FieldB2 = "MMTB2",
                   FieldCA = "MMTCA",
                   FieldCB1 = "MMTCB1",
                   FieldCB2 = "MMTCB2",
                   FieldNotNull = 0
               };

               var repositoryHandler = ormHandlerFactory.Create<ParentRecord>();
                // Create
                repositoryHandler.Handle((repository, storage) =>
               {
                   var privilegesList = repository.Clone<HierarchyRecord>().List();
                   var selectedPrivileges = new List<ParentRecordHierarchyRecord>();
                   var privilegesIdsText = "1,2";
                   if (!string.IsNullOrEmpty(privilegesIdsText))
                   {
                       var ids = privilegesIdsText.Split(',').Select(e => int.Parse(e)).ToList();
                       privilegesList.Where(e => ids.Any(e2 => e2 == e.HierarchyRecordId))
                           .ToList()
                           .ForEach(e => selectedPrivileges.Add(new ParentRecordHierarchyRecord() { ParentRecord = parentRecord, HierarchyRecordId = e.HierarchyRecordId }));
                   }

                   storage.Handle(
                       batch =>
                       {
                           batch.Add(parentRecord);
                           batch.LoadAndModifyRelated(
                               parentRecord,
                               e => e.ParentRecordHierarchyRecordMap,
                               selectedPrivileges,
                               (e1, e2) => e1.ParentRecordId == e2.ParentRecordId
                           );
                       }).ThrowIfFailed("Test failed");
                   newParentRecordId = parentRecord.ParentRecordId;
                   newRowVersion = parentRecord.RowVersion;
               });
           }));

            routine.Handle((container, closure) => container.ResolveTestDomDbContextHandler().HandleOrmFactory((ormHandlerFactory) =>
            {
                var repositoryHandler = ormHandlerFactory.Create<ParentRecord>();
                // Update
                var entity = new ParentRecord()
                {
                    ParentRecordId = newParentRecordId,
                    FieldA = "MMTA",
                    FieldB1 = "MMTB1",
                    FieldB2 = "MMTB2",
                    FieldCA = "MMTCA",
                    FieldCB1 = "MMTCB1",
                    FieldCB2 = "MMTCB2",
                    FieldNotNull = 0,
                    RowVersion = newRowVersion,
                };
                repositoryHandler.Handle((repository, storage) =>
                {
                    var privilegesList = repository.Clone<HierarchyRecord>().List();
                    var selectedPrivileges = new List<ParentRecordHierarchyRecord>();
                    var privilegesIdsText = "2,3";
                    if (!string.IsNullOrEmpty(privilegesIdsText))
                    {
                        var ids = privilegesIdsText.Split(',').Select(e => int.Parse(e)).ToList();
                        privilegesList.Where(e => ids.Any(e2 => e2 == e.HierarchyRecordId))
                            .ToList()
                            .ForEach(e => selectedPrivileges.Add(new ParentRecordHierarchyRecord() { ParentRecordId = entity.ParentRecordId, HierarchyRecordId = e.HierarchyRecordId }));
                    }

                    var storageResult = storage.Handle(
                        batch =>
                        {
                            batch.Modify(entity);
                            batch.LoadAndModifyRelated(
                                entity,
                                (e => e.ParentRecordHierarchyRecordMap),
                                selectedPrivileges,
                                (e1, e2) => e1.ParentRecordId == e2.ParentRecordId);
                        });
                    if (!storageResult.IsOk())
                        throw new Exception("Test failed");
                });
            }));
            // Remove
            routine.Handle((container, closure) => container.ResolveTestDomDbContextHandler().HandleOrmFactory((ormHandlerFactory) =>
            {
                var repositoryHandler = ormHandlerFactory.Create<ParentRecord>();
                repositoryHandler.Handle((repository, storage) =>
                {
                    var group = repository.Find(e => e.ParentRecordId == newParentRecordId);
                    var storageResult = storage.Handle(batch =>
                        batch.Remove(group)
                    );
                    if (!storageResult.IsOk())
                        throw new Exception("Test failed: includes");
                });
            }));
        }

        [TestMethod]
        public void TestStoreUpdateRelations()
        {
            var logger = new List<string>();
            var loggingTransientsFactory = InjectedManager.ComposeListMemberLoggerFactory(logger);

            var routine = new AdminkaAnonymousRoutineHandler(
                TestManager.ApplicationSettings,
                loggingTransientsFactory,
                hasVerboseLoggingPrivilege: true,
                new MemberTag(this), "UnitTest", new { input = "Input text" });
            Include<ParentRecord> includes
                = includable => includable
                    .IncludeAll(y => y.ParentRecordHierarchyRecordMap)
                        .ThenInclude(y => y.HierarchyRecord);
            routine.Handle((container, closure) => container.ResolveTestDomDbContextHandler().HandleOrmFactory((ormHandlerFactory) =>
            {
                var rh = ormHandlerFactory.Create<ParentRecord>();
                rh.Handle(
                    (repository, batch) =>
                    {
                        var parentRecord = repository.Find(e => e.FieldA == "1_A", includes);
                        var count1 = parentRecord.ParentRecordHierarchyRecordMap.Count();
                        var only2 = parentRecord.ParentRecordHierarchyRecordMap.Take(2);
                        var count2 = only2.Count();
                        var detached = new List<ParentRecordHierarchyRecord>();
                        ObjectExtensions.CopyAll<IEnumerable<ParentRecordHierarchyRecord>, ParentRecordHierarchyRecord>(only2, detached,
                            (i) => i.Include(e => e.RowVersion).Include(e => e.HierarchyRecordId).Include(e => e.ParentRecordId));

                        batch.Handle(
                            (storage) =>
                            {
                                storage.LoadAndModifyRelated(
                                    parentRecord,
                                    e => e.ParentRecordHierarchyRecordMap,
                                    detached,
                                    (e1, e2) => e1.HierarchyRecordId == e2.HierarchyRecordId);
                            });

                        var count1b = parentRecord.ParentRecordHierarchyRecordMap.Count();
                        if (count1b != 2)
                            throw new Exception("This is strange. Only two should be left");

                        var parentRecordB = repository.Find(e => e.FieldA == "1_A", includes);
                        var count1c = parentRecordB.ParentRecordHierarchyRecordMap.Count();
                        if (count1c != 2)
                            throw new Exception("This is strange. Only two should be left");

                    });


                rh.Handle(
                    (repository, batch) =>
                    {
                        var parentRecordF = repository.Find(e => e.FieldA == "1_A", includes);
                        var countF = parentRecordF.ParentRecordHierarchyRecordMap.Count();
                        var only1 = parentRecordF.ParentRecordHierarchyRecordMap.Take(1);
                        var detached = new List<ParentRecordHierarchyRecord>();
                        ObjectExtensions.CopyAll<IEnumerable<ParentRecordHierarchyRecord>, ParentRecordHierarchyRecord>(only1, detached,
                            (i) => i.Include(e => e.RowVersion).Include(e => e.HierarchyRecordId).Include(e => e.ParentRecordId));

                        try
                        {
                            batch.Handle(
                                (storage) =>
                                {
                                    storage.LoadAndModifyRelated(
                                        parentRecordF,
                                        e => e.ParentRecordHierarchyRecordMap,
                                        detached,
                                        (e1, e2) => e1.HierarchyRecordId == e2.HierarchyRecordId);
                                    throw new Exception("Break Transaction");
                                });
                        }
                        catch (Exception ex)
                        {
                            if (ex.Message != "Break Transaction")
                                throw;

                            var postCount = parentRecordF.ParentRecordHierarchyRecordMap.Count();
                            if(postCount != 1)
                              throw new Exception("This is strange. I expect there value 1 number elements in the collection. When db contains 2 elements", ex);

                            // this demonstrates EF Core problem and incorect behaviour after transaction not commited: count will be 2 (even if in db is 1)
                            rh.Handle(
                                (repository2, batch2) =>
                                {
                                    var parentRecordF2 = repository2.Find(e => e.FieldA == "1_A", includes);
                                    var countF2 = parentRecordF2.ParentRecordHierarchyRecordMap.Count();
                                    if (countF2 != 1)
                                        throw new Exception("This is strange. I expect there value 1 number elements in the collection. When db contains 2 elements", ex);
                                });

                            // this demonstrates EF Core problem and incorect behaviour after transaction not commited: count will be 2 (even if in db is 1)
                            var rhA = ormHandlerFactory.Create<ParentRecord>();
                            rhA.Handle(
                                (repository3, batch3) =>
                                {
                                    var parentRecordF3 = repository3.Find(e => e.FieldA == "1_A", includes);
                                    var countF3 = parentRecordF3.ParentRecordHierarchyRecordMap.Count();
                                    if (countF3 != 1)
                                        throw new Exception("This is strange. I expect there value 1 number elements in the collection. When db contains 2 elements", ex);
                                });
                        }
                    }
               );
            }));
        }

        [TestMethod]
        public void TestToDemonstateDetachedProblems()
        {
            var logger = new List<string>();
            var loggingTransientsFactory = InjectedManager.ComposeListMemberLoggerFactory(logger);

            var routine = new AdminkaAnonymousRoutineHandler(
                TestManager.ApplicationSettings,
                loggingTransientsFactory,
                hasVerboseLoggingPrivilege: true,
                new MemberTag(this), "UnitTest", new { input = "Input text" });
            Include<ParentRecord> includes
                = includable => includable
                    .IncludeAll(y => y.ParentRecordHierarchyRecordMap)
                        .ThenInclude(y => y.HierarchyRecord);
            routine.Handle( (container, closure) => container.ResolveTestDomDbContextHandler().HandleOrmFactory((ormHandlerFactory) =>
            {
                var rh = ormHandlerFactory.Create<ParentRecord>();
                rh.Handle(
                    (repository, batch) =>
                    {
                        var parentRecord = repository.Find(e => e.FieldA == "1_A", includes);
                        var count1 = parentRecord.ParentRecordHierarchyRecordMap.Count();
                        var only2 = parentRecord.ParentRecordHierarchyRecordMap.Take(2).ToList(); // without ToList this we get detach exception
                        var count2 = only2.Count();
                        // detach 2 elements with the goal "emulate" new collection
                        // so I expect to have only two child elements in result
                        repository.Clone<ParentRecordHierarchyRecord>().Detach(only2, 
                            (i) => i.Include(e => e.RowVersion).Include(e => e.HierarchyRecordId).Include(e => e.ParentRecordId));
                        
                        batch.Handle(
                            (storage) =>
                            {
                                storage.LoadAndModifyRelated(
                                    parentRecord,
                                    e => e.ParentRecordHierarchyRecordMap,
                                    only2,
                                    (e1, e2) => e1.HierarchyRecordId == e2.HierarchyRecordId);
                            });

                        var count1b = parentRecord.ParentRecordHierarchyRecordMap.Count();
                        if (count1b != 2)
                            throw new Exception("This is strange. Only two should be left");
                        
                        // now I "find" the same entity
                        var parentRecordB = repository.Find(e => e.FieldA == "1_A", includes);
                        var count1c = parentRecordB.ParentRecordHierarchyRecordMap.Count();
                        // and I found there 4 elements (when should be 2, e.g. in db you will find two elements)
                        // that is because elements that was detached was not fully removed from child collections trackers
                        if (count1c != 4)
                            throw new Exception("This is strange. EF Core have changed something");

                    });
            }));
        }
    }
}