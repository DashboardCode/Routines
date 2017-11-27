using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Metadata;

namespace DashboardCode.AdminkaV1.DataAccessEfCore.SqlServer.InstallerApp.Migrations
{
    public partial class Initial : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "tst");

            migrationBuilder.CreateTable(
                name: "Groups",
                columns: table => new
                {
                    GroupId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    GroupAdName = table.Column<string>(maxLength: 126, nullable: false),
                    GroupName = table.Column<string>(maxLength: 64, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Groups", x => x.GroupId);
                });

            migrationBuilder.CreateTable(
                name: "Privileges",
                columns: table => new
                {
                    PrivilegeId = table.Column<string>(maxLength: 4, nullable: false),
                    PrivilegeName = table.Column<string>(maxLength: 64, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Privileges", x => x.PrivilegeId);
                });

            migrationBuilder.CreateTable(
                name: "Roles",
                columns: table => new
                {
                    RoleId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    RoleName = table.Column<string>(maxLength: 64, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Roles", x => x.RoleId);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    UserId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    FirstName = table.Column<string>(maxLength: 64, nullable: true),
                    LoginName = table.Column<string>(maxLength: 126, nullable: false),
                    SecondName = table.Column<string>(maxLength: 32, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.UserId);
                });

            migrationBuilder.CreateTable(
                name: "ActivityRecords",
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
                columns: table => new
                {
                    ActivityRecordId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Application = table.Column<string>(maxLength: 4, nullable: false),
                    CorrelationToken = table.Column<Guid>(maxLength: 32, nullable: false),
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
                columns: table => new
                {
                    GroupId = table.Column<int>(nullable: false),
                    PrivilegeId = table.Column<string>(maxLength: 4, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GroupPrivilegeMap", x => new { x.GroupId, x.PrivilegeId });
                    table.ForeignKey(
                        name: "FK_GroupPrivilegeMap_Groups_GroupId",
                        column: x => x.GroupId,
                        principalTable: "Groups",
                        principalColumn: "GroupId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_GroupPrivilegeMap_Privileges_PrivilegeId",
                        column: x => x.PrivilegeId,
                        principalTable: "Privileges",
                        principalColumn: "PrivilegeId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "GroupRoleMap",
                columns: table => new
                {
                    GroupId = table.Column<int>(nullable: false),
                    RoleId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GroupRoleMap", x => new { x.GroupId, x.RoleId });
                    table.ForeignKey(
                        name: "FK_GroupRoleMap_Groups_GroupId",
                        column: x => x.GroupId,
                        principalTable: "Groups",
                        principalColumn: "GroupId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_GroupRoleMap_Roles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "Roles",
                        principalColumn: "RoleId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "RolePrivilegeMap",
                columns: table => new
                {
                    RoleId = table.Column<int>(nullable: false),
                    PrivilegeId = table.Column<string>(maxLength: 4, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RolePrivilegeMap", x => new { x.RoleId, x.PrivilegeId });
                    table.ForeignKey(
                        name: "FK_RolePrivilegeMap_Privileges_PrivilegeId",
                        column: x => x.PrivilegeId,
                        principalTable: "Privileges",
                        principalColumn: "PrivilegeId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_RolePrivilegeMap_Roles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "Roles",
                        principalColumn: "RoleId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UserGroupMap",
                columns: table => new
                {
                    UserId = table.Column<int>(nullable: false),
                    GroupId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserGroupMap", x => new { x.UserId, x.GroupId });
                    table.ForeignKey(
                        name: "FK_UserGroupMap_Groups_GroupId",
                        column: x => x.GroupId,
                        principalTable: "Groups",
                        principalColumn: "GroupId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UserGroupMap_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UserPrivilegeMap",
                columns: table => new
                {
                    UserId = table.Column<int>(nullable: false),
                    PrivilegeId = table.Column<string>(maxLength: 4, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserPrivilegeMap", x => new { x.UserId, x.PrivilegeId });
                    table.ForeignKey(
                        name: "FK_UserPrivilegeMap_Privileges_PrivilegeId",
                        column: x => x.PrivilegeId,
                        principalTable: "Privileges",
                        principalColumn: "PrivilegeId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UserPrivilegeMap_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UserRoleMap",
                columns: table => new
                {
                    UserId = table.Column<int>(nullable: false),
                    RoleId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserRoleMap", x => new { x.UserId, x.RoleId });
                    table.ForeignKey(
                        name: "FK_UserRoleMap_Roles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "Roles",
                        principalColumn: "RoleId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UserRoleMap_Users_UserId",
                        column: x => x.UserId,
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
                table: "GroupPrivilegeMap",
                column: "PrivilegeId");

            migrationBuilder.CreateIndex(
                name: "IX_GroupRoleMap_RoleId",
                table: "GroupRoleMap",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "IX_RolePrivilegeMap_PrivilegeId",
                table: "RolePrivilegeMap",
                column: "PrivilegeId");

            migrationBuilder.CreateIndex(
                name: "IX_UserGroupMap_GroupId",
                table: "UserGroupMap",
                column: "GroupId");

            migrationBuilder.CreateIndex(
                name: "IX_UserPrivilegeMap_PrivilegeId",
                table: "UserPrivilegeMap",
                column: "PrivilegeId");

            migrationBuilder.CreateIndex(
                name: "IX_UserRoleMap_RoleId",
                table: "UserRoleMap",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "IX_ChildRecords_TypeRecordId",
                schema: "tst",
                table: "ChildRecords",
                column: "TypeRecordId");

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
                name: "IX_ParentRecordHierarchyRecordMap_HierarchyRecordId",
                schema: "tst",
                table: "ParentRecordHierarchyRecordMap",
                column: "HierarchyRecordId");

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
                name: "GroupPrivilegeMap");

            migrationBuilder.DropTable(
                name: "GroupRoleMap");

            migrationBuilder.DropTable(
                name: "RolePrivilegeMap");

            migrationBuilder.DropTable(
                name: "UserGroupMap");

            migrationBuilder.DropTable(
                name: "UserPrivilegeMap");

            migrationBuilder.DropTable(
                name: "UserRoleMap");

            migrationBuilder.DropTable(
                name: "ActivityRecords");

            migrationBuilder.DropTable(
                name: "VerboseRecords");

            migrationBuilder.DropTable(
                name: "ChildRecords",
                schema: "tst");

            migrationBuilder.DropTable(
                name: "ParentRecordHierarchyRecordMap",
                schema: "tst");

            migrationBuilder.DropTable(
                name: "Groups");

            migrationBuilder.DropTable(
                name: "Privileges");

            migrationBuilder.DropTable(
                name: "Roles");

            migrationBuilder.DropTable(
                name: "Users");

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
