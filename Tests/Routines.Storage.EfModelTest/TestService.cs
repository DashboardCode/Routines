namespace DashboardCode.Routines.Storage.EfModelTest
{
    public static class TestService
    {
        public static void Reset(IOrmStorage storage)
        {
            storage.Handle(
                batch =>
                {
                    var typeRecord1 = new TypeRecord(){
                        TestTypeRecordId = "0000",
                        TypeRecordName = "TestType1"
                    };

                    var typeRecord2 = new TypeRecord(){
                        TestTypeRecordId = "0001",
                        TypeRecordName = "TestType2"
                    };

                    var parentRecord1 = new ParentRecord(){
                        FieldA = "1_A",
                        FieldB1 = "1_B",
                        FieldB2 = "1_C",
                        FieldCA = "1_1",
                        FieldCB1 = "1_2",
                        FieldCB2 = "1_3"
                    };

                    var parentRecord2 = new ParentRecord(){
                        FieldA = "2_A",
                        FieldB1 = "2_B",
                        FieldB2 = "2_C",
                        FieldCA = "2_1",
                        FieldCB1 = "2_2",
                        FieldCB2 = "2_3"
                    };

                    var parentRecord3 = new ParentRecord(){
                        FieldA = "3_A",
                        FieldB1 = "3_B",
                        FieldB2 = "3_C",
                        FieldCA = "3_1",
                        FieldCB1 = "3_2",
                        FieldCB2 = "3_3"
                    };

                    batch.Add(typeRecord1);
                    batch.Add(typeRecord2);

                    batch.Add(parentRecord1);
                    batch.Add(parentRecord2);
                    batch.Add(parentRecord3);

                    var childRecord1 = new ChildRecord()
                    {
                        ParentRecord = parentRecord1,
                        TypeRecord = typeRecord1,
                        XmlField1 = "notxml",
                        XmlField2 = "notxml"
                    };

                    var childRecord2 = new ChildRecord()
                    {
                        ParentRecord = parentRecord1,
                        TypeRecord = typeRecord2,
                        XmlField1 = "<xml></xml>",
                        XmlField2 = "<xml></xml>"
                    };

                    batch.Add(childRecord1);
                    batch.Add(childRecord2);

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

                    batch.Add(hierarchyRecord1);
                    batch.Add(hierarchyRecord2);
                    batch.Add(hierarchyRecord3);
                    batch.Add(hierarchyRecord4);
                    batch.Add(hierarchyRecord5);

                    var parentRecordHierarchyRecord1 = new ParentRecordHierarchyRecord() { HierarchyRecord = hierarchyRecord1, ParentRecord = parentRecord1 };
                    var parentRecordHierarchyRecord2 = new ParentRecordHierarchyRecord() { HierarchyRecord = hierarchyRecord2, ParentRecord = parentRecord1 };
                    var parentRecordHierarchyRecord3 = new ParentRecordHierarchyRecord() { HierarchyRecord = hierarchyRecord3, ParentRecord = parentRecord1 };
                    var parentRecordHierarchyRecord4 = new ParentRecordHierarchyRecord() { HierarchyRecord = hierarchyRecord4, ParentRecord = parentRecord1 };
                    var parentRecordHierarchyRecord5 = new ParentRecordHierarchyRecord() { HierarchyRecord = hierarchyRecord5, ParentRecord = parentRecord1 };

                    batch.Add(parentRecordHierarchyRecord1);
                    batch.Add(parentRecordHierarchyRecord2);
                    batch.Add(parentRecordHierarchyRecord3);
                    batch.Add(parentRecordHierarchyRecord4);
                    batch.Add(parentRecordHierarchyRecord5);

                }
                //dbContext.SaveChanges();
            );
        }

        public static void Clear(IAdoBatch adoBatch)
        {
            adoBatch.RemoveAll<ParentRecordHierarchyRecord>();
            adoBatch.RemoveAll<HierarchyRecord>();
            adoBatch.RemoveAll<ChildRecord>();
            adoBatch.RemoveAll<ParentRecord>();
            adoBatch.RemoveAll<TypeRecord>();
        }
    }
}