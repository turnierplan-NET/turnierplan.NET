using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Turnierplan.Server.Infrastructure.Database.Migrations
{
    /// <inheritdoc />
    public partial class AddProject2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_project_organization_organization_id",
                schema: "turnierplan_v2",
                table: "project");

            migrationBuilder.AddForeignKey(
                name: "fk_project_organizations_organization_id",
                schema: "turnierplan_v2",
                table: "project",
                column: "organization_id",
                principalSchema: "turnierplan_v2",
                principalTable: "organization",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_project_organizations_organization_id",
                schema: "turnierplan_v2",
                table: "project");

            migrationBuilder.AddForeignKey(
                name: "fk_project_organization_organization_id",
                schema: "turnierplan_v2",
                table: "project",
                column: "organization_id",
                principalSchema: "turnierplan_v2",
                principalTable: "organization",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
