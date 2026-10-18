using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Turnierplan.Server.Infrastructure.Database.Migrations
{
    /// <inheritdoc />
    public partial class AddProject : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "project",
                schema: "turnierplan_v2",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    public_id = table.Column<long>(type: "bigint", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    name = table.Column<string>(type: "text", nullable: false),
                    organization_id = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_project", x => x.id);
                    table.ForeignKey(
                        name: "fk_project_organization_organization_id",
                        column: x => x.organization_id,
                        principalSchema: "turnierplan_v2",
                        principalTable: "organization",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "project_role_assignment",
                schema: "turnierplan_v2",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    project_id = table.Column<long>(type: "bigint", nullable: false),
                    role = table.Column<int>(type: "integer", nullable: false),
                    principal_kind = table.Column<int>(type: "integer", nullable: false),
                    principal_id = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_project_role_assignment", x => x.id);
                    table.ForeignKey(
                        name: "fk_project_role_assignment_project_project_id",
                        column: x => x.project_id,
                        principalSchema: "turnierplan_v2",
                        principalTable: "project",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "ix_project_organization_id",
                schema: "turnierplan_v2",
                table: "project",
                column: "organization_id");

            migrationBuilder.CreateIndex(
                name: "ix_project_public_id",
                schema: "turnierplan_v2",
                table: "project",
                column: "public_id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_project_role_assignment_principal_id",
                schema: "turnierplan_v2",
                table: "project_role_assignment",
                column: "principal_id");

            migrationBuilder.CreateIndex(
                name: "ix_project_role_assignment_project_id",
                schema: "turnierplan_v2",
                table: "project_role_assignment",
                column: "project_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "project_role_assignment",
                schema: "turnierplan_v2");

            migrationBuilder.DropTable(
                name: "project",
                schema: "turnierplan_v2");
        }
    }
}
