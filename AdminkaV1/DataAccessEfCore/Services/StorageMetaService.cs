using System.Collections.Generic;
using DashboardCode.AdminkaV1.TestDom;
using DashboardCode.Routines.Storage;

namespace DashboardCode.AdminkaV1.DataAccessEfCore.Services
{
    public class StorageMetaService
    {
        public List<StorageModel> GetStorageModels()
        {
            //TODO: 
            // a) should be integrated with configuration files 
            // b) should be partly generated from DB 
            var list = new List<StorageModel> {
                new StorageModel()
                {
                    Entity = new Entity() { Name = typeof(AuthenticationDom.Group).Name, Namespace = typeof(AuthenticationDom.Group).Namespace},
                    SchemaName ="scr", TableName = "Groups",
                    Key = new Key() { Attributes = new[] { nameof(AuthenticationDom.Group.GroupId) }},
                    Uniques = new[] {
                        new Unique { IndexName="IX_scr_Groups_GroupName", Fields = new[] {nameof(AuthenticationDom.Group.GroupName)}},
                        new Unique { IndexName="IX_scr_Groups_GroupAdName", Fields = new[] { nameof(AuthenticationDom.Group.GroupAdName) } }},
                    Constraints = new[]
                    {
                        new Constraint { Name="CK_scr_Groups_GroupName",  Message=@"[^a-z0-9 ]",   Fields = new[] { "GroupName" }, Body=@"CHECK(GroupName NOT LIKE '%[^a-z0-9 ]%')" },
                        new Constraint { Name="CK_scr_Groups_GroupAdName", Message=@"[^a-z!.!-!_!\!@]", Fields = new[] { "GroupAdName" }, Body=@"CHECK(GroupAdName NOT LIKE '%[^a-z!.!-!_!\!@]%' ESCAPE '!')"  }
                    }
                },

                new StorageModel()
                {
                    Entity = new Entity() { Name = typeof(AuthenticationDom.Role).Name, Namespace = typeof(AuthenticationDom.Role).Namespace},
                    SchemaName ="scr", TableName = "Roles",
                    Key = new Key() { Attributes = new[] { nameof(AuthenticationDom.Role.RoleId) }},
                    Uniques = new[] {
                        new Unique { IndexName="IX_scr_Roles_RoleName", Fields = new[] {nameof(AuthenticationDom.Role.RoleName)}}
                        },
                    Constraints = new[]
                    {
                        new Constraint { Name="CK_scr_Roles_RoleName", Message=@"[^a-z0-9 ]", Fields = new[] { "RoleName" }, Body=@"CHECK(RoleName NOT LIKE '%[^a-z0-9 ]%')" }
                    }
                },


                new StorageModel()
                {
                    Entity = new Entity() { Name = typeof(AuthenticationDom.User).Name, Namespace = typeof(AuthenticationDom.User).Namespace},
                    SchemaName ="scr", TableName = "Users",
                    Key = new Key() { Attributes = new[] { nameof(AuthenticationDom.User.UserId) }},
                    Uniques = new[] {
                        new Unique { IndexName="IX_scr_Users_LoginName", Fields = new[] {nameof(AuthenticationDom.User.LoginName) }}
                        },
                    Constraints = new[]
                    {
                        new Constraint { Name="CK_scr_Users_LoginName", Message=@"[^a-z!.!-!_!\!@] ESCAPE '!'", Fields = new[] { "LoginName" }, Body=@"CHECK(LoginName NOT LIKE '%[^a-z!.!-!_!\!@]%' ESCAPE '!')" },
                        new Constraint { Name="CK_scr_Users_SecondName", Message=@"[^a-z '']", Fields = new[] { "SecondName" }, Body=@"CHECK(SecondName NOT LIKE '%[^a-z '']%')" },
                        new Constraint { Name="CK_scr_Users_FirstName", Message=@"[^a-z ]", Fields = new[] { "FirstName" }, Body=@"CHECK(FirstName NOT LIKE '%[^a-z ]%')" }
                    }
                },

                new StorageModel()
                {
                    Entity = new Entity() { Name = typeof(AuthenticationDom.Privilege).Name, Namespace = typeof(AuthenticationDom.Privilege).Namespace},
                    SchemaName ="scr", TableName = "Privileges",
                    Key = new Key() { Attributes = new[] { nameof(AuthenticationDom.Privilege.PrivilegeId) }},
                    Uniques = new[] {
                        new Unique { IndexName="IX_scr_Privileges_PrivilegeName", Fields = new[] {nameof(AuthenticationDom.Privilege.PrivilegeName) }}
                        },
                    Constraints = new[]
                    {
                        new Constraint { Name="CK_scr_Privileges_PrivilegeId", Message=@"[^a-z0-9]", Fields = new[] { "PrivilegeId" }, Body=@"CHECK(PrivilegeId NOT LIKE '%[^a-z0-9]%')" }
                    }
                },

                new StorageModel()
                {
                    Entity = new Entity() { Name = typeof(ParentRecord).Name, Namespace = typeof(ParentRecord).Namespace},
                    SchemaName ="tst", TableName = "tst.ParentRecords",
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
                    SchemaName ="tst", TableName = "TypeRecords",
                    Key = new Key() { Attributes = new[] { nameof(ChildRecord.TypeRecordId), nameof(ChildRecord.ParentRecordId) }}
                },

                new StorageModel()
                {
                    Entity = new Entity() { Name = typeof(HierarchyRecord).Name, Namespace = typeof(HierarchyRecord).Namespace},
                    SchemaName ="tst", TableName = "HierarchyRecordMap",
                    Key = new Key() { Attributes = new[] { nameof(HierarchyRecord.ParentHierarchyRecordId), nameof(HierarchyRecord.ParentHierarchyRecordId) }}
                },

                new StorageModel()
                {
                    Entity = new Entity() { Name = typeof(TypeRecord).Name, Namespace = typeof(TypeRecord).Namespace},
                    SchemaName ="tst", TableName = "TypeRecords",
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
