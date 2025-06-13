using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AdminkaV1.StorageDom.EfCore.SqlServer.Migrations
{
    /// <inheritdoc />
    public partial class ExcConnectionIsActive : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "ExcConnectionIsActive",
                table: "ExcConnections",
                type: "bit",
                nullable: false,
                defaultValue: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ExcConnectionIsActive",
                table: "ExcConnections");
        }
    }
}
