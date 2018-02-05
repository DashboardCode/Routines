using System;
using System.Linq;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using DashboardCode.Routines;
using DashboardCode.Routines.Storage;
using DashboardCode.AdminkaV1.TestDom;
using DashboardCode.AdminkaV1.AuthenticationDom;

namespace DashboardCode.AdminkaV1.Injected.SqlServer.Test
{
    [TestClass]
    public class StorageTest
    {
        ZoningSharedSourceManager zoningSharedSourceManager = new ZoningSharedSourceManager();

        public StorageTest()
        {
            TestIsland.Reset();
        }

        [TestMethod]
        public void TestStore()
        {
            var logger = new List<string>();
            var loggingTransientsFactory = InjectedManager.ComposeListLoggingTransients(logger);

            var userContext = new UserContext("UnitTest");
            
            var routine = new AdminkaRoutineHandler(
                zoningSharedSourceManager.GetConfiguration(),
                zoningSharedSourceManager.GetConfigurationFactory(),
                loggingTransientsFactory,
                new MemberTag(this), userContext, new { input = "Input text" });
            int newGroupId = 0;
            routine.HandleOrmFactory((ormHandlerFactory) =>
            {
                var group = new Group{
                    GroupName = "TestStore",
                    GroupAdName = "TestStore\\TestStore"
                };

                var repositoryHandler = ormHandlerFactory.Create<Group>();
                // Create
                repositoryHandler.Handle((repository, storage) =>
                {
                    var privilegesList = repository.Clone<Privilege>().List();
                    var selectedPrivileges = new List<GroupPrivilege>();
                    var privilegesIdsText = "CFGS,VLOG";
                    if (!string.IsNullOrEmpty(privilegesIdsText))
                    {
                        var ids = privilegesIdsText.Split(',');
                        privilegesList.Where(e => ids.Any(e2 => e2 == e.PrivilegeId))
                            .ToList()
                            .ForEach(e => selectedPrivileges.Add(new GroupPrivilege() { Group = group, PrivilegeId = e.PrivilegeId }));
                    }

                    storage.Handle(
                        batch =>
                        {
                            batch.Add(group);
                            batch.ModifyRelated(
                                group,
                                e => e.GroupPrivilegeMap,
                                selectedPrivileges, 
                                (e1, e2) => e1.GroupId == e2.GroupId
                            );
                        }).ThrowIfFailed("Test failed");
                    newGroupId = group.GroupId;
                });
            });

            routine.HandleOrmFactory((ormHandlerFactory) =>
            {
                var repositoryHandler = ormHandlerFactory.Create<Group>();
                // Update
                var entity = new Group() { GroupId = newGroupId, GroupName = "TestStore2", GroupAdName = "TestStore\\TestStore2" };
                repositoryHandler.Handle((repository, storage) =>
                {
                    var privilegesList = repository.Clone<Privilege>().List();
                    var selectedPrivileges = new List<GroupPrivilege>();
                    var privilegesIdsText = "CFGS";
                    if (!string.IsNullOrEmpty(privilegesIdsText))
                    {
                        var ids = privilegesIdsText.Split(',');
                        privilegesList.Where(e => ids.Any(e2 => e2 == e.PrivilegeId))
                            .ToList()
                            .ForEach(e => selectedPrivileges.Add(new GroupPrivilege() { GroupId = entity.GroupId, PrivilegeId = e.PrivilegeId }));
                    }
                    
                    var storageResult = storage.Handle(
                        batch =>
                        {
                            batch.Modify(entity);
                            batch.ModifyRelated(
                                entity,
                                (e => e.GroupPrivilegeMap),
                                selectedPrivileges,
                                (e1, e2) => e1.GroupId == e2.GroupId);
                        });
                    if (!storageResult.IsOk())
                        throw new Exception("Test failed");
                });
            });
            // Remove
            routine.HandleOrmFactory((ormHandlerFactory) =>
            {
                var repositoryHandler = ormHandlerFactory.Create<Group>();
                repositoryHandler.Handle((repository, storage) =>
                {
                    var group = repository.Find(e => e.GroupId == newGroupId);
                    var storageResult = storage.Handle(batch =>
                        batch.Remove(group)
                    );
                    if (!storageResult.IsOk())
                        throw new Exception("Test failed: includes");
                });
            });
            // Remove 
            routine.HandleOrmFactory((ormHandlerFactory) =>
            {
                var repositoryHandler = ormHandlerFactory.Create<Group>();
                repositoryHandler.Handle((repository, storage) =>
                {
                    var groups = repository.List(e => e.GroupName == "TestStore");
                    var storageResult = storage.Handle(batch =>
                    {
                        foreach (var group in groups)
                            batch.Remove(group);
                    });
                    if (!storageResult.IsOk())
                        throw new Exception("Test failed: includes");
                });
            });
        }

        [TestMethod]
        public void TestStoreUpdateRelations()
        {
            var logger = new List<string>();
            var loggingTransientsFactory = InjectedManager.ComposeListLoggingTransients(logger);

            var userContext = new UserContext("UnitTest");
            var routine = new AdminkaRoutineHandler(
                zoningSharedSourceManager.GetConfiguration(),
                zoningSharedSourceManager.GetConfigurationFactory(),
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