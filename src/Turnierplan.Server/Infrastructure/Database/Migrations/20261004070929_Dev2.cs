using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Turnierplan.Server.Infrastructure.Database.Migrations
{
    /// <inheritdoc />
    public partial class Dev2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "organization_role_assignment",
                schema: "turnierplan_v2",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    scope_id = table.Column<long>(type: "bigint", nullable: false),
                    role = table.Column<int>(type: "integer", nullable: false),
                    principal_kind = table.Column<int>(type: "integer", nullable: false),
                    principal_id = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_organization_role_assignment", x => x.id);
                    table.ForeignKey(
                        name: "fk_organization_role_assignment_organization_scope_id",
                        column: x => x.scope_id,
                        principalSchema: "turnierplan_v2",
                        principalTable: "organization",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "ix_organization_role_assignment_principal_id",
                schema: "turnierplan_v2",
                table: "organization_role_assignment",
                column: "principal_id");

            migrationBuilder.CreateIndex(
                name: "ix_organization_role_assignment_scope_id",
                schema: "turnierplan_v2",
                table: "organization_role_assignment",
                column: "scope_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "organization_role_assignment",
                schema: "turnierplan_v2");
        }
    }
}
