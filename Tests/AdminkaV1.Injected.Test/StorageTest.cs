using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Vse.AdminkaV1.DomAuthentication;
using Vse.Routines.Storage;
using Vse.Routines;
using Vse.AdminkaV1.DomTest;

namespace Vse.AdminkaV1.Injected.Test
{
    [TestClass]
    public class StorageTest
    {
        public StorageTest()
        {
            TestIsland.Reset();
        }

        [TestMethod]
        public void TestStore()
        {
            var userContext = new UserContext("UnitTest");
            var routine = new AdminkaRoutine(new RoutineTag(this), userContext, new { input = "Input text" });
            int newGroupId = 0;
            routine.Handle((state, dataAccess) =>
            {
                var group = new Group();
                group.GroupName = "TestStore";
                group.GroupAdName = "TestStore\\TestStore";

                var repositoryHandler = dataAccess.CreateRepositoryHandler<Group>();
                // Create
                repositoryHandler.Handle((repository, storage) =>
                {
                    var privilegesList = repository.Rebase<Privilege>().ToList();
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
                            batch.UpdateRelations(
                                group,
                                e => e.GroupPrivilegeMap,
                                selectedPrivileges, 
                                (e1, e2) => e1.GroupId == e2.GroupId
                            );
                        }).Desert("Test failed");
                    newGroupId = group.GroupId;
                });
            });

            routine.Handle((state, dataAccess) =>
            {
                var repositoryHandler = dataAccess.CreateRepositoryHandler<Group>();
                // Update
                var entity = new Group() { GroupId = newGroupId, GroupName = "TestStore2", GroupAdName = "TestStore\\TestStore2" };
                repositoryHandler.Handle((repository, storage) =>
                {
                    var privilegesList = repository.Rebase<Privilege>().ToList();
                    var selectedPrivileges = new List<GroupPrivilege>();
                    var privilegesIdsText = "CFGS";
                    if (!string.IsNullOrEmpty(privilegesIdsText))
                    {
                        var ids = privilegesIdsText.Split(',');
                        privilegesList.Where(e => ids.Any(e2 => e2 == e.PrivilegeId))
                            .ToList()
                            .ForEach(e => selectedPrivileges.Add(new GroupPrivilege() { GroupId = entity.GroupId, PrivilegeId = e.PrivilegeId }));
                    }

                    var storageError = storage.Handle(
                        batch =>
                        {
                            batch.Modify(entity);
                            batch.UpdateRelations(
                                entity,
                                (e => e.GroupPrivilegeMap),
                                selectedPrivileges,
                                (e1, e2) => e1.GroupId == e2.GroupId);
                        });
                    if (storageError?.FieldErrors.Count > 0)
                        throw new ApplicationException("Test failed");
                });
            });
            // Remove
            routine.Handle((state, dataAccess) =>
            {
                var repositoryHandler = dataAccess.CreateRepositoryHandler<Group>();
                repositoryHandler.Handle((repository, storage) =>
                {
                    var group = repository.Find(e => e.GroupId == newGroupId);
                    var storageError = storage.Handle(batch =>
                        batch.Remove(group)
                    );
                    if (storageError?.FieldErrors.Count > 0)
                        throw new ApplicationException("Test failed: includes");
                });
            });
            // Remove 
            routine.Handle((state, dataAccess) =>
            {
                var repositoryHandler = dataAccess.CreateRepositoryHandler<Group>();
                repositoryHandler.Handle((repository, storage) =>
                {
                    var groups = repository.ToList(e => e.GroupName == "TestStore");
                    var storageError = storage.Handle(batch =>
                    {
                        foreach (var group in groups)
                            batch.Remove(group);
                    });
                    if (storageError?.FieldErrors.Count > 0)
                        throw new ApplicationException("Test failed: includes");
                });
            });
        }

        [TestMethod]
        public void TestStoreUpdateRelations()
        {
            var userContext = new UserContext("UnitTest");
            var routine = new AdminkaRoutine(new RoutineTag(this), userContext, new { input = "Input text" });
            Include<ParentRecord> includes
                = includable => includable
                    .IncludeAll(y => y.ParentRecordHierarchyRecordMap)
                        .ThenInclude(y => y.HierarchyRecord);
            routine.Handle((state, dataAccess) =>
            {
                var rh = dataAccess.CreateRepositoryHandler<ParentRecord>();
                rh.Handle(
                    (repository, batch) =>
                    {
                        var parentRecord = repository.Find(e => e.FieldA == "1_A", includes);
                        var count1 = parentRecord.ParentRecordHierarchyRecordMap.Count();
                        var only2 = parentRecord.ParentRecordHierarchyRecordMap.Take(2);
                        var count2 = only2.Count();
                        repository.Rebase<ParentRecordHierarchyRecord>().Detach(only2, (i) => i.Include(e => e.RowVersion));

                        batch.Handle(
                            (storage) =>
                            {
                                storage.UpdateRelations(
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
                        repository.Rebase<ParentRecordHierarchyRecord>().Detach(only1, (i) => i.Include(e => e.RowVersion));

                        try
                        {
                            batch.Handle(
                                (storage) =>
                                {
                                    storage.UpdateRelations(
                                        parentRecord2,
                                        e => e.ParentRecordHierarchyRecordMap,
                                        only1,
                                        (e1, e2) => e1.HierarchyRecordId == e2.HierarchyRecordId);
                                    throw new ApplicationException("Break Transaction");
                                });
                        }
                        catch (ApplicationException ex)
                        {
                            if (ex.Message != "Break Transaction")
                                throw;

                            rh.Handle(
                                (repository2, batch2) =>
                                {
                                    var parentRecord3 = repository2.Find(e => e.FieldA == "1_A", includes);
                                    var count2 = parentRecord3.ParentRecordHierarchyRecordMap.Count();
                                    if (count2 != count3)
                                        throw new ApplicationException("This opperations should not be commited", ex);
                                });
                   }
                    }
               );
            });
        }
    }
}
