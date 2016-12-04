using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Metadata;

namespace Vse.AdminkaV1.DataAccessEfCore.SqlServer.Installer.Migrations
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
                name: "TestParentRecords",
                schema: "tst",
                columns: table => new
                {
                    TestParentRecordId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    FieldA = table.Column<string>(maxLength: 16, nullable: false),
                    FieldB1 = table.Column<string>(maxLength: 16, nullable: false),
                    FieldB2 = table.Column<string>(maxLength: 16, nullable: false),
                    FieldCA = table.Column<string>(maxLength: 16, nullable: false),
                    FieldCB1 = table.Column<string>(maxLength: 16, nullable: false),
                    FieldCB2 = table.Column<string>(maxLength: 16, nullable: false),
                    FieldNullable = table.Column<int>(nullable: false),
                    RowVersion = table.Column<byte[]>(rowVersion: true, nullable: true),
                    RowVersionAt = table.Column<DateTime>(nullable: false),
                    RowVersionBy = table.Column<string>(maxLength: 126, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TestParentRecords", x => x.TestParentRecordId);
                    table.UniqueConstraint("AK_TestParentRecords_FieldCA", x => x.FieldCA);
                    table.UniqueConstraint("AK_TestParentRecords_FieldCB1_FieldCB2", x => new { x.FieldCB1, x.FieldCB2 });
                });

            migrationBuilder.CreateTable(
                name: "TestTypeRecords",
                schema: "tst",
                columns: table => new
                {
                    TestTypeRecordId = table.Column<string>(maxLength: 4, nullable: false),
                    RowVersion = table.Column<byte[]>(rowVersion: true, nullable: true),
                    RowVersionAt = table.Column<DateTime>(nullable: false),
                    RowVersionBy = table.Column<string>(maxLength: 126, nullable: true),
                    TestTypeRecordName = table.Column<string>(maxLength: 32, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TestTypeRecords", x => x.TestTypeRecordId);
                });

            migrationBuilder.CreateTable(
                name: "GroupsPrivileges",
                columns: table => new
                {
                    GroupId = table.Column<int>(nullable: false),
                    PrivilegeId = table.Column<string>(maxLength: 4, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GroupsPrivileges", x => new { x.GroupId, x.PrivilegeId });
                    table.ForeignKey(
                        name: "FK_GroupsPrivileges_Groups_GroupId",
                        column: x => x.GroupId,
                        principalTable: "Groups",
                        principalColumn: "GroupId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_GroupsPrivileges_Privileges_PrivilegeId",
                        column: x => x.PrivilegeId,
                        principalTable: "Privileges",
                        principalColumn: "PrivilegeId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "GroupsRoles",
                columns: table => new
                {
                    GroupId = table.Column<int>(nullable: false),
                    RoleId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GroupsRoles", x => new { x.GroupId, x.RoleId });
                    table.ForeignKey(
                        name: "FK_GroupsRoles_Groups_GroupId",
                        column: x => x.GroupId,
                        principalTable: "Groups",
                        principalColumn: "GroupId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_GroupsRoles_Roles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "Roles",
                        principalColumn: "RoleId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "RolesPrivileges",
                columns: table => new
                {
                    RoleId = table.Column<int>(nullable: false),
                    PrivilegeId = table.Column<string>(maxLength: 4, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RolesPrivileges", x => new { x.RoleId, x.PrivilegeId });
                    table.ForeignKey(
                        name: "FK_RolesPrivileges_Privileges_PrivilegeId",
                        column: x => x.PrivilegeId,
                        principalTable: "Privileges",
                        principalColumn: "PrivilegeId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_RolesPrivileges_Roles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "Roles",
                        principalColumn: "RoleId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UsersGroups",
                columns: table => new
                {
                    UserId = table.Column<int>(nullable: false),
                    GroupId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UsersGroups", x => new { x.UserId, x.GroupId });
                    table.ForeignKey(
                        name: "FK_UsersGroups_Groups_GroupId",
                        column: x => x.GroupId,
                        principalTable: "Groups",
                        principalColumn: "GroupId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UsersGroups_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UsersPrivileges",
                columns: table => new
                {
                    UserId = table.Column<int>(nullable: false),
                    PrivilegeId = table.Column<string>(maxLength: 4, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UsersPrivileges", x => new { x.UserId, x.PrivilegeId });
                    table.ForeignKey(
                        name: "FK_UsersPrivileges_Privileges_PrivilegeId",
                        column: x => x.PrivilegeId,
                        principalTable: "Privileges",
                        principalColumn: "PrivilegeId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UsersPrivileges_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UsersRoles",
                columns: table => new
                {
                    UserId = table.Column<int>(nullable: false),
                    RoleId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UsersRoles", x => new { x.UserId, x.RoleId });
                    table.ForeignKey(
                        name: "FK_UsersRoles_Roles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "Roles",
                        principalColumn: "RoleId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UsersRoles_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TestChildRecords",
                schema: "tst",
                columns: table => new
                {
                    TestParentRecordId = table.Column<int>(maxLength: 4, nullable: false),
                    TestTypeRecordId = table.Column<string>(maxLength: 4, nullable: false),
                    RowVersion = table.Column<byte[]>(rowVersion: true, nullable: true),
                    RowVersionAt = table.Column<DateTime>(nullable: false),
                    RowVersionBy = table.Column<string>(maxLength: 126, nullable: true),
                    XmlField1 = table.Column<string>(type: "xml", nullable: true),
                    XmlField2 = table.Column<string>(type: "xml", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TestChildRecords", x => new { x.TestParentRecordId, x.TestTypeRecordId });
                    table.ForeignKey(
                        name: "FK_TestChildRecords_TestParentRecords_TestParentRecordId",
                        column: x => x.TestParentRecordId,
                        principalSchema: "tst",
                        principalTable: "TestParentRecords",
                        principalColumn: "TestParentRecordId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TestChildRecords_TestTypeRecords_TestTypeRecordId",
                        column: x => x.TestTypeRecordId,
                        principalSchema: "tst",
                        principalTable: "TestTypeRecords",
                        principalColumn: "TestTypeRecordId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_GroupsPrivileges_PrivilegeId",
                table: "GroupsPrivileges",
                column: "PrivilegeId");

            migrationBuilder.CreateIndex(
                name: "IX_GroupsRoles_RoleId",
                table: "GroupsRoles",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "IX_RolesPrivileges_PrivilegeId",
                table: "RolesPrivileges",
                column: "PrivilegeId");

            migrationBuilder.CreateIndex(
                name: "IX_UsersGroups_GroupId",
                table: "UsersGroups",
                column: "GroupId");

            migrationBuilder.CreateIndex(
                name: "IX_UsersPrivileges_PrivilegeId",
                table: "UsersPrivileges",
                column: "PrivilegeId");

            migrationBuilder.CreateIndex(
                name: "IX_UsersRoles_RoleId",
                table: "UsersRoles",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "IX_TestChildRecords_TestTypeRecordId",
                schema: "tst",
                table: "TestChildRecords",
                column: "TestTypeRecordId");

            migrationBuilder.CreateIndex(
                name: "IX_TestParentRecords_FieldA",
                schema: "tst",
                table: "TestParentRecords",
                column: "FieldA",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_TestParentRecords_FieldB1_FieldB2",
                schema: "tst",
                table: "TestParentRecords",
                columns: new[] { "FieldB1", "FieldB2" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_TestTypeRecords_TestTypeRecordName",
                schema: "tst",
                table: "TestTypeRecords",
                column: "TestTypeRecordName",
                unique: true);

            InitialCustoms.Up(migrationBuilder);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "GroupsPrivileges");

            migrationBuilder.DropTable(
                name: "GroupsRoles");

            migrationBuilder.DropTable(
                name: "RolesPrivileges");

            migrationBuilder.DropTable(
                name: "UsersGroups");

            migrationBuilder.DropTable(
                name: "UsersPrivileges");

            migrationBuilder.DropTable(
                name: "UsersRoles");

            migrationBuilder.DropTable(
                name: "ActivityRecords");

            migrationBuilder.DropTable(
                name: "VerboseRecords");

            migrationBuilder.DropTable(
                name: "TestChildRecords",
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
                name: "TestParentRecords",
                schema: "tst");

            migrationBuilder.DropTable(
                name: "TestTypeRecords",
                schema: "tst");
        }
    }
}
