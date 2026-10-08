using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Turnierplan.Server.Infrastructure.Database.Migrations
{
    /// <inheritdoc />
    public partial class Dev6_ApiKey : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_apikey_role_assignment_api_key_apikey_id",
                schema: "turnierplan_v2",
                table: "apikey_role_assignment");

            migrationBuilder.DropPrimaryKey(
                name: "pk_apikey_role_assignment",
                schema: "turnierplan_v2",
                table: "apikey_role_assignment");

            migrationBuilder.RenameTable(
                name: "apikey_role_assignment",
                schema: "turnierplan_v2",
                newName: "api_key_role_assignment",
                newSchema: "turnierplan_v2");

            migrationBuilder.RenameColumn(
                name: "apikey_id",
                schema: "turnierplan_v2",
                table: "api_key_role_assignment",
                newName: "api_key_id");

            migrationBuilder.RenameIndex(
                name: "ix_apikey_role_assignment_principal_id",
                schema: "turnierplan_v2",
                table: "api_key_role_assignment",
                newName: "ix_api_key_role_assignment_principal_id");

            migrationBuilder.RenameIndex(
                name: "ix_apikey_role_assignment_apikey_id",
                schema: "turnierplan_v2",
                table: "api_key_role_assignment",
                newName: "ix_api_key_role_assignment_api_key_id");

            migrationBuilder.AddPrimaryKey(
                name: "pk_api_key_role_assignment",
                schema: "turnierplan_v2",
                table: "api_key_role_assignment",
                column: "id");

            migrationBuilder.AddForeignKey(
                name: "fk_api_key_role_assignment_api_key_api_key_id",
                schema: "turnierplan_v2",
                table: "api_key_role_assignment",
                column: "api_key_id",
                principalSchema: "turnierplan_v2",
                principalTable: "api_key",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_api_key_role_assignment_api_key_api_key_id",
                schema: "turnierplan_v2",
                table: "api_key_role_assignment");

            migrationBuilder.DropPrimaryKey(
                name: "pk_api_key_role_assignment",
                schema: "turnierplan_v2",
                table: "api_key_role_assignment");

            migrationBuilder.RenameTable(
                name: "api_key_role_assignment",
                schema: "turnierplan_v2",
                newName: "apikey_role_assignment",
                newSchema: "turnierplan_v2");

            migrationBuilder.RenameColumn(
                name: "api_key_id",
                schema: "turnierplan_v2",
                table: "apikey_role_assignment",
                newName: "apikey_id");

            migrationBuilder.RenameIndex(
                name: "ix_api_key_role_assignment_principal_id",
                schema: "turnierplan_v2",
                table: "apikey_role_assignment",
                newName: "ix_apikey_role_assignment_principal_id");

            migrationBuilder.RenameIndex(
                name: "ix_api_key_role_assignment_api_key_id",
                schema: "turnierplan_v2",
                table: "apikey_role_assignment",
                newName: "ix_apikey_role_assignment_apikey_id");

            migrationBuilder.AddPrimaryKey(
                name: "pk_apikey_role_assignment",
                schema: "turnierplan_v2",
                table: "apikey_role_assignment",
                column: "id");

            migrationBuilder.AddForeignKey(
                name: "fk_apikey_role_assignment_api_key_apikey_id",
                schema: "turnierplan_v2",
                table: "apikey_role_assignment",
                column: "apikey_id",
                principalSchema: "turnierplan_v2",
                principalTable: "api_key",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
