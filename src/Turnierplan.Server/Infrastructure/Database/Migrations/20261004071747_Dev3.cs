using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Turnierplan.Server.Infrastructure.Database.Migrations
{
    /// <inheritdoc />
    public partial class Dev3 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_organization_role_assignment_organization_scope_id",
                schema: "turnierplan_v2",
                table: "organization_role_assignment");

            migrationBuilder.RenameColumn(
                name: "scope_id",
                schema: "turnierplan_v2",
                table: "organization_role_assignment",
                newName: "organization_id");

            migrationBuilder.RenameIndex(
                name: "ix_organization_role_assignment_scope_id",
                schema: "turnierplan_v2",
                table: "organization_role_assignment",
                newName: "ix_organization_role_assignment_organization_id");

            migrationBuilder.AddForeignKey(
                name: "fk_organization_role_assignment_organization_organization_id",
                schema: "turnierplan_v2",
                table: "organization_role_assignment",
                column: "organization_id",
                principalSchema: "turnierplan_v2",
                principalTable: "organization",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_organization_role_assignment_organization_organization_id",
                schema: "turnierplan_v2",
                table: "organization_role_assignment");

            migrationBuilder.RenameColumn(
                name: "organization_id",
                schema: "turnierplan_v2",
                table: "organization_role_assignment",
                newName: "scope_id");

            migrationBuilder.RenameIndex(
                name: "ix_organization_role_assignment_organization_id",
                schema: "turnierplan_v2",
                table: "organization_role_assignment",
                newName: "ix_organization_role_assignment_scope_id");

            migrationBuilder.AddForeignKey(
                name: "fk_organization_role_assignment_organization_scope_id",
                schema: "turnierplan_v2",
                table: "organization_role_assignment",
                column: "scope_id",
                principalSchema: "turnierplan_v2",
                principalTable: "organization",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
