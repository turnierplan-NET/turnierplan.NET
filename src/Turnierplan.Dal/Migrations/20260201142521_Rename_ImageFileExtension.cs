using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Turnierplan.Dal.Migrations
{
    /// <inheritdoc />
    public partial class Rename_ImageFileExtension : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "FileType",
                schema: "turnierplan",
                table: "Images",
                newName: "FileExtension");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "FileExtension",
                schema: "turnierplan",
                table: "Images",
                newName: "FileType");
        }
    }
}
