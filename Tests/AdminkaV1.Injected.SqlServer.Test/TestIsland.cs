using System;
using DashboardCode.AdminkaV1.DomTest;
using DashboardCode.Routines;
using DashboardCode.Routines.Storage;

namespace DashboardCode.AdminkaV1.Injected.SqlServer.Test
{
    public static class TestIsland
    {
        public static void Reset(string databaseName="AdminkaV1")
        {
            Clear(databaseName);

            var routine = new AdminkaRoutine(new RoutineGuid(Guid.NewGuid(), new MemberTag(typeof(TestIsland))), new UserContext("UnitTest"), ZoningSharedSourceManager.GetConfiguration(), new { input = "Input text" });
            routine.Handle((state, dataAccess) =>
            {
                var typeRecord1 = new TypeRecord()
                {
                    TestTypeRecordId = "0000",
                    TypeRecordName = "TestType1"
                };

                var typeRecord2 = new TypeRecord()
                {
                    TestTypeRecordId = "0001",
                    TypeRecordName = "TestType2"
                };

                var parentRecord1 = new ParentRecord()
                {
                    FieldA   = "1_A",
                    FieldB1  = "1_B",
                    FieldB2  = "1_C",
                    FieldCA  = "1_1",
                    FieldCB1 = "1_2",
                    FieldCB2 = "1_3"
                };

                var parentRecord2 = new ParentRecord()
                {
                    FieldA   = "2_A",
                    FieldB1  = "2_B",
                    FieldB2  = "2_C",
                    FieldCA  = "2_1",
                    FieldCB1 = "2_2",
                    FieldCB2 = "2_3"
                };

                var parentRecord3 = new ParentRecord()
                {
                    FieldA   = "3_A",
                    FieldB1  = "3_B",
                    FieldB2  = "3_C",
                    FieldCA  = "3_1",
                    FieldCB1 = "3_2",
                    FieldCB2 = "3_3"
                };

                var parentRecordHandler = dataAccess.CreateRepositoryHandler<ParentRecord>();
                parentRecordHandler.Handle((repository, storage) =>
                    storage.Handle(batch =>
                    {
                        batch.Add(parentRecord1);
                        batch.Add(parentRecord2);
                        batch.Add(parentRecord3);
                    })
                    .ThrowIfNotNull("Can't add TestParentRecord")
                );

                var typeRecordHandler = dataAccess.CreateRepositoryHandler<TypeRecord>();
                typeRecordHandler.Handle((repository, storage) =>
                    storage.Handle(batch => { batch.Add(typeRecord1); batch.Add(typeRecord2); })
                        .ThrowIfNotNull("Can't add TestTypeRecord")
                );

                var childRecord1 = new ChildRecord()
                {
                    ParentRecordId = parentRecord1.ParentRecordId,
                    TypeRecordId = typeRecord1.TestTypeRecordId,
                    XmlField1 = "notxml",
                    XmlField2 = "notxml"
                };

                var childRecord2 = new ChildRecord()
                {
                    ParentRecordId = parentRecord1.ParentRecordId,
                    TypeRecordId = typeRecord2.TestTypeRecordId,
                    XmlField1 = "<xml></xml>",
                    XmlField2 = "<xml></xml>"
                };

                var childRecordHandler = dataAccess.CreateRepositoryHandler<ChildRecord>();
                childRecordHandler.Handle((repository, storage) =>
                    storage.Handle(batch => { batch.Add(childRecord1); batch.Add(childRecord2); })
                        .ThrowIfNotNull("Can't add TestChildRecord")
                );

                var hierarchyRecord1 = new HierarchyRecord()
                {
                    HierarchyRecordTitle = "Chierarchy 1"
                };

                var hierarchyRecord2 = new HierarchyRecord()
                {
                    HierarchyRecordTitle = "Chierarchy 2"
                };

                var hierarchyRecord3 = new HierarchyRecord()
                {
                    HierarchyRecordTitle = "Chierarchy 3"
                };

                var hierarchyRecord4 = new HierarchyRecord()
                {
                    HierarchyRecordTitle = "Chierarchy 4"
                };

                var hierarchyRecord5 = new HierarchyRecord()
                {
                    HierarchyRecordTitle = "Chierarchy 5"
                };

                var hierarchyRecordHandler = dataAccess.CreateRepositoryHandler<HierarchyRecord>();
                hierarchyRecordHandler.Handle((repository, storage) =>
                    storage.Handle(batch => { batch.Add(hierarchyRecord1); batch.Add(hierarchyRecord2); batch.Add(hierarchyRecord3); batch.Add(hierarchyRecord4); batch.Add(hierarchyRecord5); })
                        .ThrowIfNotNull("Can't add HierarchyRecord")
                );
                var parentRecordHierarchyRecord1 = new ParentRecordHierarchyRecord() { HierarchyRecordId = hierarchyRecord1.HierarchyRecordId, ParentRecordId = parentRecord1.ParentRecordId };
                var parentRecordHierarchyRecord2 = new ParentRecordHierarchyRecord() { HierarchyRecordId = hierarchyRecord2.HierarchyRecordId, ParentRecordId = parentRecord1.ParentRecordId };
                var parentRecordHierarchyRecord3 = new ParentRecordHierarchyRecord() { HierarchyRecordId = hierarchyRecord3.HierarchyRecordId, ParentRecordId = parentRecord1.ParentRecordId };
                var parentRecordHierarchyRecord4 = new ParentRecordHierarchyRecord() { HierarchyRecordId = hierarchyRecord4.HierarchyRecordId, ParentRecordId = parentRecord1.ParentRecordId };
                var parentRecordHierarchyRecord5 = new ParentRecordHierarchyRecord() { HierarchyRecordId = hierarchyRecord5.HierarchyRecordId, ParentRecordId = parentRecord1.ParentRecordId };
                var manyManyHandler = dataAccess.CreateRepositoryHandler<ParentRecordHierarchyRecord>();
                manyManyHandler.Handle((repository, storage) =>
                    storage.Handle(batch => {
                        batch.Add(parentRecordHierarchyRecord1); batch.Add(parentRecordHierarchyRecord2);
                        batch.Add(parentRecordHierarchyRecord3); batch.Add(parentRecordHierarchyRecord4);
                        batch.Add(parentRecordHierarchyRecord5);
                    }).ThrowIfNotNull("Can't add ParentRecordHierarchyRecord")
                );
            });
        }

        public static void Clear(string databaseName = "AdminkaV1")
        {
            var routine = new AdminkaRoutine(new RoutineGuid(Guid.NewGuid(), new MemberTag(typeof(TestIsland))), new UserContext("UnitTest"), ZoningSharedSourceManager.GetConfiguration(), new { input = "Input text" });
            routine.Handle((state, dataAccess) =>
            {
                dataAccess.CreateRepositoryHandler<ChildRecord>().Handle((repository, storage) =>
                {
                    storage.Handle(batch =>
                    {
                        var list = repository.List();
                        foreach (var e in list)
                            batch.Remove(e);
                    }).ThrowIfNotNull();
                });
                dataAccess.CreateRepositoryHandler<ParentRecordHierarchyRecord>().Handle((repository, storage) =>
                {
                    storage.Handle(batch =>
                    {
                        var list = repository.List();
                        foreach (var e in list)
                            batch.Remove(e);
                    }).ThrowIfNotNull();
                });
                dataAccess.CreateRepositoryHandler<ParentRecord>().Handle((repository, storage) =>
                {
                    storage.Handle(batch =>
                    {
                        var list = repository.List();
                        foreach (var e in list)
                            batch.Remove(e);
                    }).ThrowIfNotNull();
                });
                dataAccess.CreateRepositoryHandler<TypeRecord>().Handle((repository, storage) =>
                {
                    storage.Handle(batch =>
                    {
                        var list = repository.List();
                        foreach (var e in list)
                            batch.Remove(e);
                    }).ThrowIfNotNull();
                });
                dataAccess.CreateRepositoryHandler<ParentRecordHierarchyRecord>().Handle((repository, storage) =>
                {
                    storage.Handle(batch =>
                    {
                        var list = repository.List();
                        foreach (var e in list)
                            batch.Remove(e);
                    }).ThrowIfNotNull();
                });
            });
        }
    }
}
