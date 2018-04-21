using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace DashboardCode.AdminkaV1.Injected.NETStandard.EfCoreMigrationApp.Migrations
{
    public partial class MigrationName : Migration
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
                    GroupName = table.Column<string>(maxLength: 32, nullable: false),
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
                name: "IX_scr_Groups_GroupAdName",
                schema: "scr",
                table: "Groups",
                column: "GroupAdName",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_scr_Groups_GroupName",
                schema: "scr",
                table: "Groups",
                column: "GroupName",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_scr_Privileges_PrivilegeName",
                schema: "scr",
                table: "Privileges",
                column: "PrivilegeName",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_RolePrivilegeMap_PrivilegeId",
                schema: "scr",
                table: "RolePrivilegeMap",
                column: "PrivilegeId");

            migrationBuilder.CreateIndex(
                name: "IX_scr_Roles_RoleName",
                schema: "scr",
                table: "Roles",
                column: "RoleName",
                unique: true);

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
                name: "IX_scr_Users_LoginName",
                schema: "scr",
                table: "Users",
                column: "LoginName",
                unique: true);

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

            InitialCustoms.Up(migrationBuilder, TargetModel);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
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
        }
    }
}
