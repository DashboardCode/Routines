using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AdminkaV1.StorageDom.EfCore.SqlServer.Migrations
{
    /// <inheritdoc />
    public partial class Initial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ExcConnections",
                columns: table => new
                {
                    ExcConnectionId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ExcConnectionCode = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ExcConnectionName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ExcConnectionDescription = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ExcConnectionXMeta = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ExcConnectionType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ExcConnectionString = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ExcConnections", x => x.ExcConnectionId);
                });

            migrationBuilder.CreateTable(
                name: "ExcTables",
                columns: table => new
                {
                    ExcTableId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ExcTableDescription = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ExcTableXMeta = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ExcTablePath = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ExcTableFields = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ExcConnectionId = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ExcTables", x => x.ExcTableId);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ExcConnections");

            migrationBuilder.DropTable(
                name: "ExcTables");
        }
    }
}
