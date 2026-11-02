using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Turnierplan.Server.Infrastructure.Database.Migrations
{
    /// <inheritdoc />
    public partial class Initial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "turnierplan_v2");

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
                name: "organization",
                schema: "turnierplan_v2",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    public_id = table.Column<long>(type: "bigint", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    name = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_organization", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "user",
                schema: "turnierplan_v2",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    principal_id = table.Column<Guid>(type: "uuid", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    user_name = table.Column<string>(type: "text", nullable: false),
                    normalized_user_name = table.Column<string>(type: "text", nullable: false),
                    full_name = table.Column<string>(type: "text", nullable: true),
                    e_mail = table.Column<string>(type: "text", nullable: true),
                    normalized_e_mail = table.Column<string>(type: "text", nullable: true),
                    password_hash = table.Column<string>(type: "text", nullable: false),
                    is_administrator = table.Column<bool>(type: "boolean", nullable: false),
                    allow_create_organization = table.Column<bool>(type: "boolean", nullable: false),
                    last_password_change = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    last_login = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    security_stamp = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_user", x => x.id);
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
                name: "api_key_role_assignment",
                schema: "turnierplan_v2",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    api_key_id = table.Column<long>(type: "bigint", nullable: false),
                    role = table.Column<int>(type: "integer", nullable: false),
                    principal_kind = table.Column<int>(type: "integer", nullable: false),
                    principal_id = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_api_key_role_assignment", x => x.id);
                    table.ForeignKey(
                        name: "fk_api_key_role_assignment_api_key_api_key_id",
                        column: x => x.api_key_id,
                        principalSchema: "turnierplan_v2",
                        principalTable: "api_key",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "organization_role_assignment",
                schema: "turnierplan_v2",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    organization_id = table.Column<long>(type: "bigint", nullable: false),
                    role = table.Column<int>(type: "integer", nullable: false),
                    principal_kind = table.Column<int>(type: "integer", nullable: false),
                    principal_id = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_organization_role_assignment", x => x.id);
                    table.ForeignKey(
                        name: "fk_organization_role_assignment_organization_organization_id",
                        column: x => x.organization_id,
                        principalSchema: "turnierplan_v2",
                        principalTable: "organization",
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
                name: "ix_api_key_role_assignment_api_key_id",
                schema: "turnierplan_v2",
                table: "api_key_role_assignment",
                column: "api_key_id");

            migrationBuilder.CreateIndex(
                name: "ix_api_key_role_assignment_principal_id",
                schema: "turnierplan_v2",
                table: "api_key_role_assignment",
                column: "principal_id");

            migrationBuilder.CreateIndex(
                name: "ix_api_key_role_assignment_principal_kind",
                schema: "turnierplan_v2",
                table: "api_key_role_assignment",
                column: "principal_kind");

            migrationBuilder.CreateIndex(
                name: "ix_organization_public_id",
                schema: "turnierplan_v2",
                table: "organization",
                column: "public_id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_organization_role_assignment_organization_id",
                schema: "turnierplan_v2",
                table: "organization_role_assignment",
                column: "organization_id");

            migrationBuilder.CreateIndex(
                name: "ix_organization_role_assignment_principal_id",
                schema: "turnierplan_v2",
                table: "organization_role_assignment",
                column: "principal_id");

            migrationBuilder.CreateIndex(
                name: "ix_organization_role_assignment_principal_kind",
                schema: "turnierplan_v2",
                table: "organization_role_assignment",
                column: "principal_kind");

            migrationBuilder.CreateIndex(
                name: "ix_user_normalized_e_mail",
                schema: "turnierplan_v2",
                table: "user",
                column: "normalized_e_mail",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_user_normalized_user_name",
                schema: "turnierplan_v2",
                table: "user",
                column: "normalized_user_name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_user_principal_id",
                schema: "turnierplan_v2",
                table: "user",
                column: "principal_id",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "api_key_request",
                schema: "turnierplan_v2");

            migrationBuilder.DropTable(
                name: "api_key_role_assignment",
                schema: "turnierplan_v2");

            migrationBuilder.DropTable(
                name: "organization_role_assignment",
                schema: "turnierplan_v2");

            migrationBuilder.DropTable(
                name: "user",
                schema: "turnierplan_v2");

            migrationBuilder.DropTable(
                name: "api_key",
                schema: "turnierplan_v2");

            migrationBuilder.DropTable(
                name: "organization",
                schema: "turnierplan_v2");
        }
    }
}
