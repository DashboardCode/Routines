using System.Collections.Generic;
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
                    Entity = new Entity() { Name = typeof(DomAuthentication.Group).Name, Namespace = typeof(DomAuthentication.Group).Namespace},
                    TableName = "dbo.Groups",
                    Key = new Key() { Attributes = new[] { nameof(DomAuthentication.Group.GroupId) }},
                    Uniques = new[] {
                        new Unique { IndexName="IX_Groups_GroupName", Fields = new[] {nameof(DomAuthentication.Group.GroupName)}},
                        new Unique { IndexName="IX_Groups_GroupAdName", Fields = new[] { nameof(DomAuthentication.Group.GroupAdName) } }},
                },

                new StorageModel()
                {
                    Entity = new Entity() { Name = typeof(ParentRecord).Name, Namespace = typeof(ParentRecord).Namespace},
                    TableName = "tst.ParentRecords",
                    Key = new Key() { Attributes = new[] { "ParentRecordId" }},
                    Requireds = new[] {  nameof(ParentRecord.FieldA), nameof(ParentRecord.FieldB1), nameof(ParentRecord.FieldB2), nameof(ParentRecord.FieldCA), nameof(ParentRecord.FieldCB1), nameof(ParentRecord.FieldCB2) },
                    Uniques = new[] {
                        new Unique { IndexName="IX_ParentRecords_FieldA", Fields = new[] { nameof(ParentRecord.FieldA) }  },
                        new Unique { IndexName="IX_ParentRecords_FieldB1_FieldB2", Fields = new[] { nameof(ParentRecord.FieldB1), nameof(ParentRecord.FieldB2) } } ,
                        new Unique { IndexName="AK_ParentRecords_FieldCA", Fields = new[] { nameof(ParentRecord.FieldCA) } },
                        new Unique { IndexName="AK_ParentRecords_FieldCB1_FieldCB2", Fields = new[] { nameof(ParentRecord.FieldCB1), nameof(ParentRecord.FieldCB2) } }
                    },
                },

                new StorageModel()
                {
                    Entity = new Entity() { Name = typeof(ChildRecord).Name, Namespace = typeof(ChildRecord).Namespace},
                    TableName = "tst.TypeRecords",
                    Key = new Key() { Attributes = new[] { nameof(ChildRecord.TypeRecordId), nameof(ChildRecord.ParentRecordId) }}
                },

                new StorageModel()
                {
                    Entity = new Entity() { Name = typeof(HierarchyRecord).Name, Namespace = typeof(HierarchyRecord).Namespace},
                    TableName = "tst.HierarchyRecordMap",
                    Key = new Key() { Attributes = new[] { nameof(HierarchyRecord.ParentHierarchyRecordId), nameof(HierarchyRecord.ParentHierarchyRecordId) }}
                },

                new StorageModel()
                {
                    Entity = new Entity() { Name = typeof(TypeRecord).Name, Namespace = typeof(TypeRecord).Namespace},
                    TableName = "tst.TypeRecords",
                    Key = new Key() { Attributes = new[] { nameof(TypeRecord.TestTypeRecordId) }},
                    Requireds = new[] {  nameof(TypeRecord.TypeRecordName) },
                    Constraints = new[] {
                        new Constraint { Fields = new[] { nameof(TypeRecord.TypeRecordName) },
                                         Message = "Only letters, numbers and space",
                                         Name = "CK_TypeRecords_TypeRecordName" } }
                }
            };
            return list;
        }
    }
}
