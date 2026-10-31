using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Turnierplan.Server.Infrastructure.Database.Migrations
{
    /// <inheritdoc />
    public partial class Tournament3 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_tournament_project_project_id",
                schema: "turnierplan_v2",
                table: "tournament");

            migrationBuilder.AddForeignKey(
                name: "fk_tournament_project_project_id",
                schema: "turnierplan_v2",
                table: "tournament",
                column: "project_id",
                principalSchema: "turnierplan_v2",
                principalTable: "project",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_tournament_project_project_id",
                schema: "turnierplan_v2",
                table: "tournament");

            migrationBuilder.AddForeignKey(
                name: "fk_tournament_project_project_id",
                schema: "turnierplan_v2",
                table: "tournament",
                column: "project_id",
                principalSchema: "turnierplan_v2",
                principalTable: "project",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
