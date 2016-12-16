using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Metadata;

namespace EfCoreTest.Migrations
{
    public partial class Initial : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "tst");

            migrationBuilder.CreateTable(
                name: "HierarchyRecords",
                schema: "tst",
                columns: table => new
                {
                    HierarchyRecordId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    HierarchyRecordTitle = table.Column<string>(maxLength: 128, nullable: false),
                    ParentHierarchyRecordId = table.Column<byte[]>(nullable: true)
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
                    FieldNotNull = table.Column<int>(nullable: false)
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
                name: "ParentRecordHierarchyRecordMap",
                schema: "tst",
                columns: table => new
                {
                    ParentRecordId = table.Column<int>(nullable: false),
                    HierarchyRecordId = table.Column<int>(nullable: false)
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
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ChildRecords",
                schema: "tst");

            migrationBuilder.DropTable(
                name: "ParentRecordHierarchyRecordMap",
                schema: "tst");

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
