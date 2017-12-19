using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace DashboardCode.AdminkaV1.DataAccessEfCore.SqlServer.InstallerApp.Migrations
{
    public partial class InitialCreate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "scr");

            migrationBuilder.EnsureSchema(
                name: "log");

            migrationBuilder.EnsureSchema(
                name: "tst");

            migrationBuilder.CreateTable(
                name: "ElectrodeRemelt",
                columns: table => new
                {
                    ElectrodeId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    ArchivedAt = table.Column<DateTime>(nullable: true),
                    ArchivedBy = table.Column<string>(nullable: true),
                    FurnaceTypeId = table.Column<string>(nullable: true),
                    InpsectionsPercent = table.Column<int>(nullable: false),
                    MeltPositionId = table.Column<int>(nullable: false),
                    ReadyAt = table.Column<DateTime>(nullable: false),
                    ReadyBy = table.Column<string>(nullable: true),
                    RemeltStatusId = table.Column<string>(nullable: true),
                    RowVersion = table.Column<byte[]>(rowVersion: true, nullable: true),
                    RowVersionAt = table.Column<DateTime>(nullable: false),
                    RowVersionBy = table.Column<string>(maxLength: 126, nullable: true),
                    SetupPercent = table.Column<int>(nullable: false),
                    StatusAt = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ElectrodeRemelt", x => x.ElectrodeId);
                });

            migrationBuilder.CreateTable(
                name: "MeltPosition",
                columns: table => new
                {
                    MeltPositionId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    MeltPositionName = table.Column<string>(nullable: true),
                    RowVersion = table.Column<byte[]>(rowVersion: true, nullable: true),
                    RowVersionAt = table.Column<DateTime>(nullable: false),
                    RowVersionBy = table.Column<string>(maxLength: 126, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MeltPosition", x => x.MeltPositionId);
                });

            migrationBuilder.CreateTable(
                name: "Operator",
                columns: table => new
                {
                    OperatorId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    FirstName = table.Column<string>(nullable: true),
                    Initials = table.Column<string>(nullable: true),
                    SecondName = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Operator", x => x.OperatorId);
                });

            migrationBuilder.CreateTable(
                name: "ActivityRecords",
                schema: "log",
                columns: table => new
                {
                    ActivityRecordId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    ActivityRecordLoggedAt = table.Column<DateTime>(nullable: false),
                    Application = table.Column<string>(maxLength: 4, nullable: false),
                    CorrelationToken = table.Column<Guid>(nullable: false),
                    DurationTicks = table.Column<long>(nullable: false),
                    FullActionName = table.Column<string>(maxLength: 32, nullable: false),
                    Successed = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ActivityRecords", x => x.ActivityRecordId);
                });

            migrationBuilder.CreateTable(
                name: "VerboseRecords",
                schema: "log",
                columns: table => new
                {
                    ActivityRecordId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Application = table.Column<string>(maxLength: 4, nullable: false),
                    CorrelationToken = table.Column<Guid>(nullable: false),
                    FullActionName = table.Column<string>(maxLength: 32, nullable: false),
                    VerboseRecordLoggedAt = table.Column<DateTime>(nullable: false),
                    VerboseRecordMessage = table.Column<string>(nullable: true),
                    VerboseRecordTypeId = table.Column<string>(maxLength: 4, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VerboseRecords", x => x.ActivityRecordId);
                });

            migrationBuilder.CreateTable(
                name: "Groups",
                schema: "scr",
                columns: table => new
                {
                    GroupId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    GroupAdName = table.Column<string>(maxLength: 126, nullable: false),
                    GroupName = table.Column<string>(maxLength: 64, nullable: false),
                    RowVersion = table.Column<byte[]>(rowVersion: true, nullable: true),
                    RowVersionAt = table.Column<DateTime>(nullable: false),
                    RowVersionBy = table.Column<string>(maxLength: 126, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Groups", x => x.GroupId);
                });

            migrationBuilder.CreateTable(
                name: "Privileges",
                schema: "scr",
                columns: table => new
                {
                    PrivilegeId = table.Column<string>(maxLength: 4, nullable: false),
                    PrivilegeName = table.Column<string>(maxLength: 64, nullable: false),
                    RowVersion = table.Column<byte[]>(rowVersion: true, nullable: true),
                    RowVersionAt = table.Column<DateTime>(nullable: false),
                    RowVersionBy = table.Column<string>(maxLength: 126, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Privileges", x => x.PrivilegeId);
                });

            migrationBuilder.CreateTable(
                name: "Roles",
                schema: "scr",
                columns: table => new
                {
                    RoleId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    RoleName = table.Column<string>(maxLength: 64, nullable: false),
                    RowVersion = table.Column<byte[]>(rowVersion: true, nullable: true),
                    RowVersionAt = table.Column<DateTime>(nullable: false),
                    RowVersionBy = table.Column<string>(maxLength: 126, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Roles", x => x.RoleId);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                schema: "scr",
                columns: table => new
                {
                    UserId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    FirstName = table.Column<string>(maxLength: 64, nullable: true),
                    LoginName = table.Column<string>(maxLength: 126, nullable: false),
                    RowVersion = table.Column<byte[]>(rowVersion: true, nullable: true),
                    RowVersionAt = table.Column<DateTime>(nullable: false),
                    RowVersionBy = table.Column<string>(maxLength: 126, nullable: true),
                    SecondName = table.Column<string>(maxLength: 32, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.UserId);
                });

            migrationBuilder.CreateTable(
                name: "HierarchyRecords",
                schema: "tst",
                columns: table => new
                {
                    HierarchyRecordId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    HierarchyRecordTitle = table.Column<string>(maxLength: 128, nullable: false),
                    ParentHierarchyRecordId = table.Column<byte[]>(nullable: true),
                    RowVersion = table.Column<byte[]>(rowVersion: true, nullable: true),
                    RowVersionAt = table.Column<DateTime>(nullable: false),
                    RowVersionBy = table.Column<string>(maxLength: 126, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HierarchyRecords", x => x.HierarchyRecordId);
                });

            migrationBuilder.CreateTable(
                name: "ParentRecords",
                schema: "tst",
                columns: table => new
                {
                    ParentRecordId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    FieldA = table.Column<string>(maxLength: 16, nullable: false),
                    FieldB1 = table.Column<string>(maxLength: 16, nullable: false),
                    FieldB2 = table.Column<string>(maxLength: 16, nullable: false),
                    FieldCA = table.Column<string>(maxLength: 16, nullable: false),
                    FieldCB1 = table.Column<string>(maxLength: 16, nullable: false),
                    FieldCB2 = table.Column<string>(maxLength: 16, nullable: false),
                    FieldNotNull = table.Column<int>(nullable: false),
                    RowVersion = table.Column<byte[]>(rowVersion: true, nullable: true),
                    RowVersionAt = table.Column<DateTime>(nullable: false),
                    RowVersionBy = table.Column<string>(maxLength: 126, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ParentRecords", x => x.ParentRecordId);
                    table.UniqueConstraint("AK_ParentRecords_FieldCA", x => x.FieldCA);
                    table.UniqueConstraint("AK_ParentRecords_FieldCB1_FieldCB2", x => new { x.FieldCB1, x.FieldCB2 });
                });

            migrationBuilder.CreateTable(
                name: "TypeRecords",
                schema: "tst",
                columns: table => new
                {
                    TestTypeRecordId = table.Column<string>(maxLength: 4, nullable: false),
                    RowVersion = table.Column<byte[]>(rowVersion: true, nullable: true),
                    RowVersionAt = table.Column<DateTime>(nullable: false),
                    RowVersionBy = table.Column<string>(maxLength: 126, nullable: true),
                    TypeRecordName = table.Column<string>(maxLength: 32, nullable: false),
                    TypeRecordTestTypeRecordId = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TypeRecords", x => x.TestTypeRecordId);
                    table.ForeignKey(
                        name: "FK_TypeRecords_TypeRecords_TypeRecordTestTypeRecordId",
                        column: x => x.TypeRecordTestTypeRecordId,
                        principalSchema: "tst",
                        principalTable: "TypeRecords",
                        principalColumn: "TestTypeRecordId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ElectrodeRemeltInspectionsCheckList",
                columns: table => new
                {
                    ElectrodeId = table.Column<int>(nullable: false),
                    CheckListXml = table.Column<string>(type: "xml", nullable: true),
                    RowVersion = table.Column<byte[]>(rowVersion: true, nullable: true),
                    RowVersionAt = table.Column<DateTime>(nullable: false),
                    RowVersionBy = table.Column<string>(maxLength: 126, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ElectrodeRemeltInspectionsCheckList", x => x.ElectrodeId);
                    table.ForeignKey(
                        name: "FK_ElectrodeRemeltInspectionsCheckList_ElectrodeRemelt_ElectrodeId",
                        column: x => x.ElectrodeId,
                        principalTable: "ElectrodeRemelt",
                        principalColumn: "ElectrodeId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "InspectionGroup",
                columns: table => new
                {
                    InspectionGroupId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    InspectionGroupName = table.Column<string>(nullable: true),
                    MeltPositionId = table.Column<int>(nullable: false),
                    RowVersion = table.Column<byte[]>(rowVersion: true, nullable: true),
                    RowVersionAt = table.Column<DateTime>(nullable: false),
                    RowVersionBy = table.Column<string>(maxLength: 126, nullable: true),
                    SeqNumber = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InspectionGroup", x => x.InspectionGroupId);
                    table.ForeignKey(
                        name: "FK_InspectionGroup_MeltPosition_MeltPositionId",
                        column: x => x.MeltPositionId,
                        principalTable: "MeltPosition",
                        principalColumn: "MeltPositionId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "GroupPrivilegeMap",
                schema: "scr",
                columns: table => new
                {
                    GroupId = table.Column<int>(nullable: false),
                    PrivilegeId = table.Column<string>(maxLength: 4, nullable: false),
                    RowVersion = table.Column<byte[]>(rowVersion: true, nullable: true),
                    RowVersionAt = table.Column<DateTime>(nullable: false),
                    RowVersionBy = table.Column<string>(maxLength: 126, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GroupPrivilegeMap", x => new { x.GroupId, x.PrivilegeId });
                    table.ForeignKey(
                        name: "FK_GroupPrivilegeMap_Groups_GroupId",
                        column: x => x.GroupId,
                        principalSchema: "scr",
                        principalTable: "Groups",
                        principalColumn: "GroupId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_GroupPrivilegeMap_Privileges_PrivilegeId",
                        column: x => x.PrivilegeId,
                        principalSchema: "scr",
                        principalTable: "Privileges",
                        principalColumn: "PrivilegeId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "GroupRoleMap",
                schema: "scr",
                columns: table => new
                {
                    GroupId = table.Column<int>(nullable: false),
                    RoleId = table.Column<int>(nullable: false),
                    RowVersion = table.Column<byte[]>(rowVersion: true, nullable: true),
                    RowVersionAt = table.Column<DateTime>(nullable: false),
                    RowVersionBy = table.Column<string>(maxLength: 126, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GroupRoleMap", x => new { x.GroupId, x.RoleId });
                    table.ForeignKey(
                        name: "FK_GroupRoleMap_Groups_GroupId",
                        column: x => x.GroupId,
                        principalSchema: "scr",
                        principalTable: "Groups",
                        principalColumn: "GroupId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_GroupRoleMap_Roles_RoleId",
                        column: x => x.RoleId,
                        principalSchema: "scr",
                        principalTable: "Roles",
                        principalColumn: "RoleId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "RolePrivilegeMap",
                schema: "scr",
                columns: table => new
                {
                    RoleId = table.Column<int>(nullable: false),
                    PrivilegeId = table.Column<string>(maxLength: 4, nullable: false),
                    RowVersion = table.Column<byte[]>(rowVersion: true, nullable: true),
                    RowVersionAt = table.Column<DateTime>(nullable: false),
                    RowVersionBy = table.Column<string>(maxLength: 126, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RolePrivilegeMap", x => new { x.RoleId, x.PrivilegeId });
                    table.ForeignKey(
                        name: "FK_RolePrivilegeMap_Privileges_PrivilegeId",
                        column: x => x.PrivilegeId,
                        principalSchema: "scr",
                        principalTable: "Privileges",
                        principalColumn: "PrivilegeId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_RolePrivilegeMap_Roles_RoleId",
                        column: x => x.RoleId,
                        principalSchema: "scr",
                        principalTable: "Roles",
                        principalColumn: "RoleId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UserGroupMap",
                schema: "scr",
                columns: table => new
                {
                    UserId = table.Column<int>(nullable: false),
                    GroupId = table.Column<int>(nullable: false),
                    RowVersion = table.Column<byte[]>(rowVersion: true, nullable: true),
                    RowVersionAt = table.Column<DateTime>(nullable: false),
                    RowVersionBy = table.Column<string>(maxLength: 126, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserGroupMap", x => new { x.UserId, x.GroupId });
                    table.ForeignKey(
                        name: "FK_UserGroupMap_Groups_GroupId",
                        column: x => x.GroupId,
                        principalSchema: "scr",
                        principalTable: "Groups",
                        principalColumn: "GroupId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UserGroupMap_Users_UserId",
                        column: x => x.UserId,
                        principalSchema: "scr",
                        principalTable: "Users",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UserPrivilegeMap",
                schema: "scr",
                columns: table => new
                {
                    UserId = table.Column<int>(nullable: false),
                    PrivilegeId = table.Column<string>(maxLength: 4, nullable: false),
                    RowVersion = table.Column<byte[]>(rowVersion: true, nullable: true),
                    RowVersionAt = table.Column<DateTime>(nullable: false),
                    RowVersionBy = table.Column<string>(maxLength: 126, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserPrivilegeMap", x => new { x.UserId, x.PrivilegeId });
                    table.ForeignKey(
                        name: "FK_UserPrivilegeMap_Privileges_PrivilegeId",
                        column: x => x.PrivilegeId,
                        principalSchema: "scr",
                        principalTable: "Privileges",
                        principalColumn: "PrivilegeId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UserPrivilegeMap_Users_UserId",
                        column: x => x.UserId,
                        principalSchema: "scr",
                        principalTable: "Users",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UserRoleMap",
                schema: "scr",
                columns: table => new
                {
                    UserId = table.Column<int>(nullable: false),
                    RoleId = table.Column<int>(nullable: false),
                    RowVersion = table.Column<byte[]>(rowVersion: true, nullable: true),
                    RowVersionAt = table.Column<DateTime>(nullable: false),
                    RowVersionBy = table.Column<string>(maxLength: 126, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserRoleMap", x => new { x.UserId, x.RoleId });
                    table.ForeignKey(
                        name: "FK_UserRoleMap_Roles_RoleId",
                        column: x => x.RoleId,
                        principalSchema: "scr",
                        principalTable: "Roles",
                        principalColumn: "RoleId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UserRoleMap_Users_UserId",
                        column: x => x.UserId,
                        principalSchema: "scr",
                        principalTable: "Users",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ParentRecordHierarchyRecordMap",
                schema: "tst",
                columns: table => new
                {
                    ParentRecordId = table.Column<int>(nullable: false),
                    HierarchyRecordId = table.Column<int>(nullable: false),
                    RowVersion = table.Column<byte[]>(rowVersion: true, nullable: true),
                    RowVersionAt = table.Column<DateTime>(nullable: false),
                    RowVersionBy = table.Column<string>(maxLength: 126, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ParentRecordHierarchyRecordMap", x => new { x.ParentRecordId, x.HierarchyRecordId });
                    table.ForeignKey(
                        name: "FK_ParentRecordHierarchyRecordMap_HierarchyRecords_HierarchyRecordId",
                        column: x => x.HierarchyRecordId,
                        principalSchema: "tst",
                        principalTable: "HierarchyRecords",
                        principalColumn: "HierarchyRecordId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ParentRecordHierarchyRecordMap_ParentRecords_ParentRecordId",
                        column: x => x.ParentRecordId,
                        principalSchema: "tst",
                        principalTable: "ParentRecords",
                        principalColumn: "ParentRecordId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ChildRecords",
                schema: "tst",
                columns: table => new
                {
                    ParentRecordId = table.Column<int>(maxLength: 4, nullable: false),
                    TypeRecordId = table.Column<string>(maxLength: 4, nullable: false),
                    RowVersion = table.Column<byte[]>(rowVersion: true, nullable: true),
                    RowVersionAt = table.Column<DateTime>(nullable: false),
                    RowVersionBy = table.Column<string>(maxLength: 126, nullable: true),
                    XmlField1 = table.Column<string>(type: "xml", nullable: true),
                    XmlField2 = table.Column<string>(type: "xml", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ChildRecords", x => new { x.ParentRecordId, x.TypeRecordId });
                    table.ForeignKey(
                        name: "FK_ChildRecords_ParentRecords_ParentRecordId",
                        column: x => x.ParentRecordId,
                        principalSchema: "tst",
                        principalTable: "ParentRecords",
                        principalColumn: "ParentRecordId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ChildRecords_TypeRecords_TypeRecordId",
                        column: x => x.TypeRecordId,
                        principalSchema: "tst",
                        principalTable: "TypeRecords",
                        principalColumn: "TestTypeRecordId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Inspection",
                columns: table => new
                {
                    InspectionId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    InspectionGroupId = table.Column<int>(nullable: false),
                    InspectionTitle = table.Column<string>(nullable: true),
                    IsMeasured = table.Column<bool>(nullable: false),
                    IsRejectionTrigger = table.Column<bool>(nullable: false),
                    RowVersion = table.Column<byte[]>(rowVersion: true, nullable: true),
                    RowVersionAt = table.Column<DateTime>(nullable: false),
                    RowVersionBy = table.Column<string>(maxLength: 126, nullable: true),
                    SeqNumberInGroup = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Inspection", x => x.InspectionId);
                    table.ForeignKey(
                        name: "FK_Inspection_InspectionGroup_InspectionGroupId",
                        column: x => x.InspectionGroupId,
                        principalTable: "InspectionGroup",
                        principalColumn: "InspectionGroupId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ElectrodeRemeltInspection",
                columns: table => new
                {
                    ElectrodeId = table.Column<int>(nullable: false),
                    InspectionId = table.Column<int>(nullable: false),
                    CompletedAt = table.Column<DateTime>(nullable: false),
                    CompletedByOperatorId = table.Column<int>(nullable: false),
                    InspectionArchived = table.Column<string>(nullable: true),
                    IsCompleted = table.Column<bool>(nullable: false),
                    Notes = table.Column<string>(nullable: true),
                    RowVersion = table.Column<byte[]>(rowVersion: true, nullable: true),
                    RowVersionAt = table.Column<DateTime>(nullable: false),
                    RowVersionBy = table.Column<string>(maxLength: 126, nullable: true),
                    Value = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ElectrodeRemeltInspection", x => new { x.ElectrodeId, x.InspectionId });
                    table.ForeignKey(
                        name: "FK_ElectrodeRemeltInspection_Operator_CompletedByOperatorId",
                        column: x => x.CompletedByOperatorId,
                        principalTable: "Operator",
                        principalColumn: "OperatorId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ElectrodeRemeltInspection_ElectrodeRemelt_ElectrodeId",
                        column: x => x.ElectrodeId,
                        principalTable: "ElectrodeRemelt",
                        principalColumn: "ElectrodeId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ElectrodeRemeltInspection_Inspection_InspectionId",
                        column: x => x.InspectionId,
                        principalTable: "Inspection",
                        principalColumn: "InspectionId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "FurnaceType",
                columns: table => new
                {
                    FurnaceTypeId = table.Column<string>(nullable: false),
                    InspectionId = table.Column<int>(nullable: true),
                    RowVersion = table.Column<byte[]>(rowVersion: true, nullable: true),
                    RowVersionAt = table.Column<DateTime>(nullable: false),
                    RowVersionBy = table.Column<string>(maxLength: 126, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FurnaceType", x => x.FurnaceTypeId);
                    table.ForeignKey(
                        name: "FK_FurnaceType_Inspection_InspectionId",
                        column: x => x.InspectionId,
                        principalTable: "Inspection",
                        principalColumn: "InspectionId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ElectrodeRemeltInspection_CompletedByOperatorId",
                table: "ElectrodeRemeltInspection",
                column: "CompletedByOperatorId");

            migrationBuilder.CreateIndex(
                name: "IX_ElectrodeRemeltInspection_InspectionId",
                table: "ElectrodeRemeltInspection",
                column: "InspectionId");

            migrationBuilder.CreateIndex(
                name: "IX_FurnaceType_InspectionId",
                table: "FurnaceType",
                column: "InspectionId");

            migrationBuilder.CreateIndex(
                name: "IX_Inspection_InspectionGroupId",
                table: "Inspection",
                column: "InspectionGroupId");

            migrationBuilder.CreateIndex(
                name: "IX_InspectionGroup_MeltPositionId",
                table: "InspectionGroup",
                column: "MeltPositionId");

            migrationBuilder.CreateIndex(
                name: "IX_GroupPrivilegeMap_PrivilegeId",
                schema: "scr",
                table: "GroupPrivilegeMap",
                column: "PrivilegeId");

            migrationBuilder.CreateIndex(
                name: "IX_GroupRoleMap_RoleId",
                schema: "scr",
                table: "GroupRoleMap",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "IX_RolePrivilegeMap_PrivilegeId",
                schema: "scr",
                table: "RolePrivilegeMap",
                column: "PrivilegeId");

            migrationBuilder.CreateIndex(
                name: "IX_UserGroupMap_GroupId",
                schema: "scr",
                table: "UserGroupMap",
                column: "GroupId");

            migrationBuilder.CreateIndex(
                name: "IX_UserPrivilegeMap_PrivilegeId",
                schema: "scr",
                table: "UserPrivilegeMap",
                column: "PrivilegeId");

            migrationBuilder.CreateIndex(
                name: "IX_UserRoleMap_RoleId",
                schema: "scr",
                table: "UserRoleMap",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "IX_ChildRecords_TypeRecordId",
                schema: "tst",
                table: "ChildRecords",
                column: "TypeRecordId");

            migrationBuilder.CreateIndex(
                name: "IX_ParentRecordHierarchyRecordMap_HierarchyRecordId",
                schema: "tst",
                table: "ParentRecordHierarchyRecordMap",
                column: "HierarchyRecordId");

            migrationBuilder.CreateIndex(
                name: "IX_ParentRecords_FieldA",
                schema: "tst",
                table: "ParentRecords",
                column: "FieldA",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ParentRecords_FieldB1_FieldB2",
                schema: "tst",
                table: "ParentRecords",
                columns: new[] { "FieldB1", "FieldB2" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_TypeRecords_TypeRecordName",
                schema: "tst",
                table: "TypeRecords",
                column: "TypeRecordName",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_TypeRecords_TypeRecordTestTypeRecordId",
                schema: "tst",
                table: "TypeRecords",
                column: "TypeRecordTestTypeRecordId");

            InitialCustoms.Up(migrationBuilder);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ElectrodeRemeltInspection");

            migrationBuilder.DropTable(
                name: "ElectrodeRemeltInspectionsCheckList");

            migrationBuilder.DropTable(
                name: "FurnaceType");

            migrationBuilder.DropTable(
                name: "ActivityRecords",
                schema: "log");

            migrationBuilder.DropTable(
                name: "VerboseRecords",
                schema: "log");

            migrationBuilder.DropTable(
                name: "GroupPrivilegeMap",
                schema: "scr");

            migrationBuilder.DropTable(
                name: "GroupRoleMap",
                schema: "scr");

            migrationBuilder.DropTable(
                name: "RolePrivilegeMap",
                schema: "scr");

            migrationBuilder.DropTable(
                name: "UserGroupMap",
                schema: "scr");

            migrationBuilder.DropTable(
                name: "UserPrivilegeMap",
                schema: "scr");

            migrationBuilder.DropTable(
                name: "UserRoleMap",
                schema: "scr");

            migrationBuilder.DropTable(
                name: "ChildRecords",
                schema: "tst");

            migrationBuilder.DropTable(
                name: "ParentRecordHierarchyRecordMap",
                schema: "tst");

            migrationBuilder.DropTable(
                name: "Operator");

            migrationBuilder.DropTable(
                name: "ElectrodeRemelt");

            migrationBuilder.DropTable(
                name: "Inspection");

            migrationBuilder.DropTable(
                name: "Groups",
                schema: "scr");

            migrationBuilder.DropTable(
                name: "Privileges",
                schema: "scr");

            migrationBuilder.DropTable(
                name: "Roles",
                schema: "scr");

            migrationBuilder.DropTable(
                name: "Users",
                schema: "scr");

            migrationBuilder.DropTable(
                name: "TypeRecords",
                schema: "tst");

            migrationBuilder.DropTable(
                name: "HierarchyRecords",
                schema: "tst");

            migrationBuilder.DropTable(
                name: "ParentRecords",
                schema: "tst");

            migrationBuilder.DropTable(
                name: "InspectionGroup");

            migrationBuilder.DropTable(
                name: "MeltPosition");
        }
    }
}
