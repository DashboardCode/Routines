using System;
using System.Collections.Generic;

namespace Ef6Test
{
    public static class TestIsland
    {
        public static void Reset(string connectionString)
        {
            Clear(connectionString);
            
            using (var dbContext = new MyDbContext(connectionString, null))
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
                    FieldA = "1_A",
                    FieldB1 = "1_B",
                    FieldB2 = "1_C",
                    FieldCA = "1_1",
                    FieldCB1 = "1_2",
                    FieldCB2 = "1_3"
                };

                var parentRecord2 = new ParentRecord()
                {
                    FieldA = "2_A",
                    FieldB1 = "2_B",
                    FieldB2 = "2_C",
                    FieldCA = "2_1",
                    FieldCB1 = "2_2",
                    FieldCB2 = "2_3"
                };

                var parentRecord3 = new ParentRecord()
                {
                    FieldA = "3_A",
                    FieldB1 = "3_B",
                    FieldB2 = "3_C",
                    FieldCA = "3_1",
                    FieldCB1 = "3_2",
                    FieldCB2 = "3_3"
                };

                dbContext.TypeRecords.Add(typeRecord1);
                dbContext.TypeRecords.Add(typeRecord2);

                dbContext.ParentRecords.Add(parentRecord1);
                dbContext.ParentRecords.Add(parentRecord2);
                dbContext.ParentRecords.Add(parentRecord3);

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

                dbContext.ChildRecords.Add(childRecord1);
                dbContext.ChildRecords.Add(childRecord2);

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

                dbContext.HierarchyRecords.Add(hierarchyRecord1);
                dbContext.HierarchyRecords.Add(hierarchyRecord2);
                dbContext.HierarchyRecords.Add(hierarchyRecord3);
                dbContext.HierarchyRecords.Add(hierarchyRecord4);
                dbContext.HierarchyRecords.Add(hierarchyRecord5);
                dbContext.SaveChanges();

                var parentRecordHierarchyRecord1 = new ParentRecordHierarchyRecord() { HierarchyRecordId = hierarchyRecord1.HierarchyRecordId, ParentRecordId = parentRecord1.ParentRecordId };
                var parentRecordHierarchyRecord2 = new ParentRecordHierarchyRecord() { HierarchyRecordId = hierarchyRecord2.HierarchyRecordId, ParentRecordId = parentRecord1.ParentRecordId };
                var parentRecordHierarchyRecord3 = new ParentRecordHierarchyRecord() { HierarchyRecordId = hierarchyRecord3.HierarchyRecordId, ParentRecordId = parentRecord1.ParentRecordId };
                var parentRecordHierarchyRecord4 = new ParentRecordHierarchyRecord() { HierarchyRecordId = hierarchyRecord4.HierarchyRecordId, ParentRecordId = parentRecord1.ParentRecordId };
                var parentRecordHierarchyRecord5 = new ParentRecordHierarchyRecord() { HierarchyRecordId = hierarchyRecord5.HierarchyRecordId, ParentRecordId = parentRecord1.ParentRecordId };

                dbContext.ParentRecordHierarchyRecords.Add(parentRecordHierarchyRecord1);
                dbContext.ParentRecordHierarchyRecords.Add(parentRecordHierarchyRecord2);
                dbContext.ParentRecordHierarchyRecords.Add(parentRecordHierarchyRecord3);
                dbContext.ParentRecordHierarchyRecords.Add(parentRecordHierarchyRecord4);
                dbContext.ParentRecordHierarchyRecords.Add(parentRecordHierarchyRecord5);

                dbContext.SaveChanges();
            }
        }
        public static void Clear(string connectionString)
        {
            using (var dbContext = new MyDbContext(connectionString, null))
            {
                //dbContext.Database.Migrate();

                dbContext.Database.ExecuteSqlCommand("DELETE FROM tst.ParentRecordHierarchyRecordMap");
                dbContext.Database.ExecuteSqlCommand("DELETE FROM tst.HierarchyRecords");
                dbContext.Database.ExecuteSqlCommand("DELETE FROM tst.ChildRecords");
                dbContext.Database.ExecuteSqlCommand("DELETE FROM tst.ParentRecords");
                dbContext.Database.ExecuteSqlCommand("DELETE FROM tst.TypeRecords");
            }
        }
    }
}
