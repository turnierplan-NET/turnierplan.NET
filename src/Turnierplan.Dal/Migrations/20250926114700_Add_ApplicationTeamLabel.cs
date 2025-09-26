using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Turnierplan.Dal.Migrations
{
    /// <inheritdoc />
    public partial class Add_ApplicationTeamLabel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Labels",
                schema: "turnierplan",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "text", nullable: false),
                    Description = table.Column<string>(type: "text", nullable: false),
                    ColorCode = table.Column<string>(type: "text", nullable: false),
                    PlanningRealmId = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Labels", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Labels_PlanningRealms_PlanningRealmId",
                        column: x => x.PlanningRealmId,
                        principalSchema: "turnierplan",
                        principalTable: "PlanningRealms",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ApplicationTeamLabel",
                schema: "turnierplan",
                columns: table => new
                {
                    ApplicationTeamId = table.Column<long>(type: "bigint", nullable: false),
                    LabelsId = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ApplicationTeamLabel", x => new { x.ApplicationTeamId, x.LabelsId });
                    table.ForeignKey(
                        name: "FK_ApplicationTeamLabel_ApplicationTeams_ApplicationTeamId",
                        column: x => x.ApplicationTeamId,
                        principalSchema: "turnierplan",
                        principalTable: "ApplicationTeams",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ApplicationTeamLabel_Labels_LabelsId",
                        column: x => x.LabelsId,
                        principalSchema: "turnierplan",
                        principalTable: "Labels",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ApplicationTeamLabel_LabelsId",
                schema: "turnierplan",
                table: "ApplicationTeamLabel",
                column: "LabelsId");

            migrationBuilder.CreateIndex(
                name: "IX_Labels_PlanningRealmId",
                schema: "turnierplan",
                table: "Labels",
                column: "PlanningRealmId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ApplicationTeamLabel",
                schema: "turnierplan");

            migrationBuilder.DropTable(
                name: "Labels",
                schema: "turnierplan");
        }
    }
}
