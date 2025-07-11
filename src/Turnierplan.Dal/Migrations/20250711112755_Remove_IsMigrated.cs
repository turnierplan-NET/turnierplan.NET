using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Turnierplan.Dal.Migrations
{
    /// <inheritdoc />
    public partial class Remove_IsMigrated : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsMigrated",
                schema: "turnierplan",
                table: "Tournaments");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsMigrated",
                schema: "turnierplan",
                table: "Tournaments",
                type: "boolean",
                nullable: false,
                defaultValue: false);
        }
    }
}
