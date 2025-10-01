using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Turnierplan.Dal.Migrations
{
    /// <inheritdoc />
    public partial class Add_ApplicationNotesHistory : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string[]>(
                name: "NotesHistory",
                schema: "turnierplan",
                table: "Applications",
                type: "text[]",
                nullable: false,
                defaultValue: new string[0]);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "NotesHistory",
                schema: "turnierplan",
                table: "Applications");
        }
    }
}
