using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Turnierplan.Dal.Migrations
{
    /// <inheritdoc />
    public partial class Add_ApplicationChangeLog : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ApplicationChangeLogs",
                schema: "turnierplan",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ApplicationId = table.Column<long>(type: "bigint", nullable: false),
                    Timestamp = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Type = table.Column<int>(type: "integer", nullable: false),
                    OldValue = table.Column<string>(type: "text", nullable: true),
                    NewValue = table.Column<string>(type: "text", nullable: true),
                    Properties = table.Column<IReadOnlyDictionary<string, string>>(type: "jsonb", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ApplicationChangeLogs", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ApplicationChangeLogs_Applications_ApplicationId",
                        column: x => x.ApplicationId,
                        principalSchema: "turnierplan",
                        principalTable: "Applications",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ApplicationChangeLogs_ApplicationId",
                schema: "turnierplan",
                table: "ApplicationChangeLogs",
                column: "ApplicationId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ApplicationChangeLogs",
                schema: "turnierplan");
        }
    }
}
