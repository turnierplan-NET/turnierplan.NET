using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Turnierplan.Dal.Migrations
{
    /// <inheritdoc />
    public partial class Add_ApplicationFormSession : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "FormSession",
                schema: "turnierplan",
                table: "Applications",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Applications_FormSession",
                schema: "turnierplan",
                table: "Applications",
                column: "FormSession",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Applications_FormSession",
                schema: "turnierplan",
                table: "Applications");

            migrationBuilder.DropColumn(
                name: "FormSession",
                schema: "turnierplan",
                table: "Applications");
        }
    }
}
