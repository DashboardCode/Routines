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

            var userContext = new UserContext("UnitTest");

            var routine = new AdminkaRoutineHandler(
                TestManager.ApplicationSettings.AdminkaStorageConfiguration,
                TestManager.ApplicationSettings.PerformanceCounters,
                TestManager.ApplicationSettings.ConfigurationContainerFactory,
                loggingTransientsFactory,
                new MemberTag(this), userContext, new { input = "Input text" });
            int newParentRecordId = 0;
            byte[] newRowVersion = null;
            routine.HandleOrmFactory(ormHandlerFactory =>
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
                           batch.ModifyRelated(
                               parentRecord,
                               e => e.ParentRecordHierarchyRecordMap,
                               selectedPrivileges,
                               (e1, e2) => e1.ParentRecordId == e2.ParentRecordId
                           );
                       }).ThrowIfFailed("Test failed");
                   newParentRecordId = parentRecord.ParentRecordId;
                   newRowVersion = parentRecord.RowVersion;
               });
           });

            routine.HandleOrmFactory((ormHandlerFactory) =>
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
                            batch.ModifyRelated(
                                entity,
                                (e => e.ParentRecordHierarchyRecordMap),
                                selectedPrivileges,
                                (e1, e2) => e1.ParentRecordId == e2.ParentRecordId);
                        });
                    if (!storageResult.IsOk())
                        throw new Exception("Test failed");
                });
            });
            // Remove
            routine.HandleOrmFactory((ormHandlerFactory) =>
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
            });
        }

        [TestMethod]
        public void TestStoreUpdateRelations()
        {
            var logger = new List<string>();
            var loggingTransientsFactory = InjectedManager.ComposeListMemberLoggerFactory(logger);

            var userContext = new UserContext("UnitTest");
            var routine = new AdminkaRoutineHandler(
                TestManager.ApplicationSettings.AdminkaStorageConfiguration,
                TestManager.ApplicationSettings.PerformanceCounters,
                TestManager.ApplicationSettings.ConfigurationContainerFactory,
                loggingTransientsFactory,
                new MemberTag(this), userContext, new { input = "Input text" });
            Include<ParentRecord> includes
                = includable => includable
                    .IncludeAll(y => y.ParentRecordHierarchyRecordMap)
                        .ThenInclude(y => y.HierarchyRecord);
            routine.HandleOrmFactory((ormHandlerFactory) =>
            {
                var rh = ormHandlerFactory.Create<ParentRecord>();
                rh.Handle(
                    (repository, batch) =>
                    {
                        var parentRecord = repository.Find(e => e.FieldA == "1_A", includes);
                        var count1 = parentRecord.ParentRecordHierarchyRecordMap.Count();
                        var only2 = parentRecord.ParentRecordHierarchyRecordMap.Take(2);
                        var count2 = only2.Count();
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
                    });
                rh.Handle(
                    (repository, batch) =>
                    {
                        var parentRecord2 = repository.Find(e => e.FieldA == "1_A", includes);
                        var count3 = parentRecord2.ParentRecordHierarchyRecordMap.Count();
                        var only1 = parentRecord2.ParentRecordHierarchyRecordMap.Take(1);
                        repository.Clone<ParentRecordHierarchyRecord>().Detach(only1, (i) => i.Include(e => e.RowVersion));

                        try
                        {
                            batch.Handle(
                                (storage) =>
                                {
                                    storage.ModifyRelated(
                                        parentRecord2,
                                        e => e.ParentRecordHierarchyRecordMap,
                                        only1,
                                        (e1, e2) => e1.HierarchyRecordId == e2.HierarchyRecordId);
                                    throw new Exception("Break Transaction");
                                });
                        }
                        catch (Exception ex)
                        {
                            if (ex.Message != "Break Transaction")
                                throw;

                            rh.Handle(
                                (repository2, batch2) =>
                                {
                                    var parentRecord3 = repository2.Find(e => e.FieldA == "1_A", includes);
                                    var count2 = parentRecord3.ParentRecordHierarchyRecordMap.Count();
                                    if (count2 != count3)
                                        throw new Exception("This opperations should not be commited", ex);
                                });
                        }
                    }
               );
            });
        }
    }
}