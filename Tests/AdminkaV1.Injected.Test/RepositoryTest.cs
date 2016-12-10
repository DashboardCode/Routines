using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Vse.AdminkaV1.DomAuthentication;
using Vse.Routines.Storage;
using Vse.Routines;

namespace Vse.AdminkaV1.Injected.Test
{
    [TestClass]
    public class RepositoryTest
    {
        [TestMethod]
        public void TestDetach()
        {
            var userContext = new UserContext("UnitTest");
            var routine = new AdminkaRoutine(new RoutineTag(this), userContext, new { input = "Input text" });

            Include<Group> include = includable =>
                       includable
                       .IncludeAll(y => y.GroupsPrivileges)
                            .ThenInclude(y => y.Privilege)
                       .IncludeAll(y => y.UsersGroups)
                            .ThenInclude(y => y.User);
            routine.Handle((state, dataAccess) =>
            {
                var repositoryHandler = dataAccess.CreateRepositoryHandler<Group>();
                repositoryHandler.Handle((repository, storage) =>
                {
                    var group = repository.GetQueryable(include).First();
                    repository.Detach(group, include);
                    InjectedManager.SerializeToJson(group);
                });
            });
        }

        [TestMethod]
        public void TestStore()
        {
            var userContext = new UserContext("UnitTest");
            var routine = new AdminkaRoutine(new RoutineTag(this), userContext, new { input = "Input text" });

            Action<Includable<Group>> includes = includable =>
                       includable.IncludeAll(y => y.GroupsPrivileges)
                       .ThenInclude(y => y.Privilege)
                       .IncludeAll(y => y.UsersGroups)
                       .ThenInclude(y => y.User);
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
                    var selectedPrivileges = new List<GroupsPrivileges>();
                    var privilegesIdsText = "CFGS,VLOG";
                    if (!string.IsNullOrEmpty(privilegesIdsText))
                    {
                        var ids = privilegesIdsText.Split(',');
                        privilegesList.Where(e => ids.Any(e2 => e2 == e.PrivilegeId))
                            .ToList()
                            .ForEach(e => selectedPrivileges.Add(new GroupsPrivileges() { Group = group, PrivilegeId = e.PrivilegeId }));
                    }

                    storage.Handle(
                        batch =>
                        {
                            batch.Add(group);
                            batch.UpdateRelations(
                                group,
                                e => e.GroupsPrivileges,
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
                    var selectedPrivileges = new List<GroupsPrivileges>();
                    var privilegesIdsText = "CFGS";
                    if (!string.IsNullOrEmpty(privilegesIdsText))
                    {
                        var ids = privilegesIdsText.Split(',');
                        privilegesList.Where(e => ids.Any(e2 => e2 == e.PrivilegeId))
                            .ToList()
                            .ForEach(e => selectedPrivileges.Add(new GroupsPrivileges() { GroupId = entity.GroupId, PrivilegeId = e.PrivilegeId }));
                    }

                    var storageError = storage.Handle(
                        batch =>
                        {
                            batch.Modify(entity);
                            batch.UpdateRelations(
                                entity,
                                (e => e.GroupsPrivileges),
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
        }
        [TestMethod]
        public void TestClearStore()
        {
            var userContext = new UserContext("UnitTest");
            var routine = new AdminkaRoutine(new RoutineTag(this), userContext, new { input = "Input text" });
            routine.Handle((state, dataAccess) =>
            {
                var repositoryHandler = dataAccess.CreateRepositoryHandler<Group>();
                repositoryHandler.Handle((repository, storage) =>
                {
                    var groups = repository.ToList(e=>e.GroupName== "TestStore");
                    var storageError = storage.Handle(batch =>
                    {
                        foreach(var group in groups)
                            batch.Remove(group);
                    });
                    if (storageError?.FieldErrors.Count > 0)
                        throw new ApplicationException("Test failed: includes");
                });
            });
        }
    }
}
