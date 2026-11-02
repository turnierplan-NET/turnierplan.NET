using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Turnierplan.Server.Infrastructure.Database.Migrations
{
    /// <inheritdoc />
    public partial class AddTournament1 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_project_organizations_organization_id",
                schema: "turnierplan_v2",
                table: "project");

            migrationBuilder.CreateTable(
                name: "tournament",
                schema: "turnierplan_v2",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    project_id = table.Column<long>(type: "bigint", nullable: false),
                    public_id = table.Column<long>(type: "bigint", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    name = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_tournament", x => x.id);
                    table.ForeignKey(
                        name: "fk_tournament_project_project_id",
                        column: x => x.project_id,
                        principalSchema: "turnierplan_v2",
                        principalTable: "project",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "tournament_role_assignment",
                schema: "turnierplan_v2",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    tournament_id = table.Column<long>(type: "bigint", nullable: false),
                    role = table.Column<int>(type: "integer", nullable: false),
                    principal_kind = table.Column<int>(type: "integer", nullable: false),
                    principal_id = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_tournament_role_assignment", x => x.id);
                    table.ForeignKey(
                        name: "fk_tournament_role_assignment_tournament_tournament_id",
                        column: x => x.tournament_id,
                        principalSchema: "turnierplan_v2",
                        principalTable: "tournament",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "ix_tournament_project_id",
                schema: "turnierplan_v2",
                table: "tournament",
                column: "project_id");

            migrationBuilder.CreateIndex(
                name: "ix_tournament_public_id",
                schema: "turnierplan_v2",
                table: "tournament",
                column: "public_id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_tournament_role_assignment_principal_id",
                schema: "turnierplan_v2",
                table: "tournament_role_assignment",
                column: "principal_id");

            migrationBuilder.CreateIndex(
                name: "ix_tournament_role_assignment_principal_kind",
                schema: "turnierplan_v2",
                table: "tournament_role_assignment",
                column: "principal_kind");

            migrationBuilder.CreateIndex(
                name: "ix_tournament_role_assignment_tournament_id",
                schema: "turnierplan_v2",
                table: "tournament_role_assignment",
                column: "tournament_id");

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

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_project_organization_organization_id",
                schema: "turnierplan_v2",
                table: "project");

            migrationBuilder.DropTable(
                name: "tournament_role_assignment",
                schema: "turnierplan_v2");

            migrationBuilder.DropTable(
                name: "tournament",
                schema: "turnierplan_v2");

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
    }
}
