using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Turnierplan.Server.Infrastructure.Database.Migrations
{
    /// <inheritdoc />
    public partial class Dev5_ApiKey : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "api_key",
                schema: "turnierplan_v2",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    public_id = table.Column<long>(type: "bigint", nullable: false),
                    principal_id = table.Column<Guid>(type: "uuid", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    name = table.Column<string>(type: "text", nullable: false),
                    description = table.Column<string>(type: "text", nullable: false),
                    secret_hash = table.Column<string>(type: "text", nullable: false),
                    expiry_date = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    is_active = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_api_key", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "api_key_request",
                schema: "turnierplan_v2",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    timestamp = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    path = table.Column<string>(type: "text", nullable: false),
                    api_key_id = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_api_key_request", x => x.id);
                    table.ForeignKey(
                        name: "fk_api_key_request_api_key_api_key_id",
                        column: x => x.api_key_id,
                        principalSchema: "turnierplan_v2",
                        principalTable: "api_key",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "apikey_role_assignment",
                schema: "turnierplan_v2",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    apikey_id = table.Column<long>(type: "bigint", nullable: false),
                    role = table.Column<int>(type: "integer", nullable: false),
                    principal_kind = table.Column<int>(type: "integer", nullable: false),
                    principal_id = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_apikey_role_assignment", x => x.id);
                    table.ForeignKey(
                        name: "fk_apikey_role_assignment_api_key_apikey_id",
                        column: x => x.apikey_id,
                        principalSchema: "turnierplan_v2",
                        principalTable: "api_key",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "ix_api_key_principal_id",
                schema: "turnierplan_v2",
                table: "api_key",
                column: "principal_id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_api_key_public_id",
                schema: "turnierplan_v2",
                table: "api_key",
                column: "public_id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_api_key_request_api_key_id",
                schema: "turnierplan_v2",
                table: "api_key_request",
                column: "api_key_id");

            migrationBuilder.CreateIndex(
                name: "ix_apikey_role_assignment_apikey_id",
                schema: "turnierplan_v2",
                table: "apikey_role_assignment",
                column: "apikey_id");

            migrationBuilder.CreateIndex(
                name: "ix_apikey_role_assignment_principal_id",
                schema: "turnierplan_v2",
                table: "apikey_role_assignment",
                column: "principal_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "api_key_request",
                schema: "turnierplan_v2");

            migrationBuilder.DropTable(
                name: "apikey_role_assignment",
                schema: "turnierplan_v2");

            migrationBuilder.DropTable(
                name: "api_key",
                schema: "turnierplan_v2");
        }
    }
}
