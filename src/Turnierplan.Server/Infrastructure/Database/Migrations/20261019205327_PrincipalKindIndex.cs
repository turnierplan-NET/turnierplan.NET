using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Turnierplan.Server.Infrastructure.Database.Migrations
{
    /// <inheritdoc />
    public partial class PrincipalKindIndex : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "ix_project_role_assignment_principal_kind",
                schema: "turnierplan_v2",
                table: "project_role_assignment",
                column: "principal_kind");

            migrationBuilder.CreateIndex(
                name: "ix_organization_role_assignment_principal_kind",
                schema: "turnierplan_v2",
                table: "organization_role_assignment",
                column: "principal_kind");

            migrationBuilder.CreateIndex(
                name: "ix_api_key_role_assignment_principal_kind",
                schema: "turnierplan_v2",
                table: "api_key_role_assignment",
                column: "principal_kind");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "ix_project_role_assignment_principal_kind",
                schema: "turnierplan_v2",
                table: "project_role_assignment");

            migrationBuilder.DropIndex(
                name: "ix_organization_role_assignment_principal_kind",
                schema: "turnierplan_v2",
                table: "organization_role_assignment");

            migrationBuilder.DropIndex(
                name: "ix_api_key_role_assignment_principal_kind",
                schema: "turnierplan_v2",
                table: "api_key_role_assignment");
        }
    }
}
