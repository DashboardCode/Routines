using System.Collections.Generic;
using System.Text.RegularExpressions;
using Vse.AdminkaV1.DomTest;
using Vse.Routines.Storage;

namespace Vse.AdminkaV1.DataAccessEfCore.Services
{
    public class StorageMetaService
    {
        readonly string connectionString;
        public StorageMetaService(string connectionString)
        {
            this.connectionString= connectionString;
        }

        public string GetConnectionString()
        {
            return connectionString;
        }
        public List<StorageModel> GetStorageModels()
        {
            //TODO: 
            // a) should be integrated with configuration files 
            // b) should be partly generated from DB 
            var list = new List<StorageModel> {
                new StorageModel()
                {
                    Entity = new Entity() { Name = typeof(Group).Name, Namespace = typeof(Group).Namespace},
                    TableName = "dbo.Groups",
                    Key = new Key() { Attributes = new[] { "GroupId" }},
                    Uniques = new[] {
                        new Unique { IndexName="IX_Groups_GroupName", Fields = new[] { "GroupName"}},
                        new Unique { IndexName="IX_Groups_GroupAdName", Fields = new[] { "GroupAdName"} }},
                },

                new StorageModel()
                {
                    Entity = new Entity() { Name = typeof(TestParentRecord).Name, Namespace = typeof(TestParentRecord).Namespace},
                    TableName = "tst.TestParentRecords",
                    Key = new Key() { Attributes = new[] { "TestParentRecordId" }},
                    Requireds = new[] {  nameof(TestParentRecord.FieldA), nameof(TestParentRecord.FieldB1), nameof(TestParentRecord.FieldB2), nameof(TestParentRecord.FieldCA), nameof(TestParentRecord.FieldCB1), nameof(TestParentRecord.FieldCB2) },
                    Uniques = new[] {
                        new Unique { IndexName="IX_TestParentRecords_FieldA", Fields = new[] { nameof(TestParentRecord.FieldA) }  },
                        new Unique { IndexName="IX_TestParentRecords_FieldB1_FieldB2", Fields = new[] { nameof(TestParentRecord.FieldB1), nameof(TestParentRecord.FieldB2) } } ,
                        new Unique { IndexName="AK_TestParentRecords_FieldCA", Fields = new[] { nameof(TestParentRecord.FieldCA) } },
                        new Unique { IndexName="AK_TestParentRecords_FieldCB1_FieldCB2", Fields = new[] { nameof(TestParentRecord.FieldCB1), nameof(TestParentRecord.FieldCB2) } }
                    },
                },

                new StorageModel()
                {
                    Entity = new Entity() { Name = typeof(TestChildRecord).Name, Namespace = typeof(TestChildRecord).Namespace},
                    TableName = "tst.TestTypeRecords",
                    Key = new Key() { Attributes = new[] { nameof(TestChildRecord.TestTypeRecordId), nameof(TestChildRecord.TestParentRecordId) }}
                },

                new StorageModel()
                {
                    Entity = new Entity() { Name = typeof(TestTypeRecord).Name, Namespace = typeof(TestTypeRecord).Namespace},
                    TableName = "tst.TestTypeRecords",
                    Key = new Key() { Attributes = new[] { nameof(TestTypeRecord.TestTypeRecordId) }},
                    Requireds = new[] {  nameof(TestTypeRecord.TestTypeRecordName) },
                    Constraints = new[] {
                        new Constraint { Fields = new[] { nameof(TestTypeRecord.TestTypeRecordName) },
                                         Message = "Only letters, numbers and space",
                                         Name = "CK_TestTypeRecords_TestTypeRecordName" } }
                }
            };
            return list;
        }
    }
}
