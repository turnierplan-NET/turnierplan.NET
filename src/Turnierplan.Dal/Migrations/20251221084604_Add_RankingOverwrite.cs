using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Turnierplan.Dal.Migrations
{
    /// <inheritdoc />
    public partial class Add_RankingOverwrite : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "RankingOverwrites",
                schema: "turnierplan",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false),
                    TournamentId = table.Column<long>(type: "bigint", nullable: false),
                    PlacementRank = table.Column<int>(type: "integer", nullable: false),
                    HideRanking = table.Column<bool>(type: "boolean", nullable: false),
                    AssignTeamTournamentId = table.Column<long>(type: "bigint", nullable: true),
                    AssignTeamId = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RankingOverwrites", x => new { x.TournamentId, x.Id });
                    table.ForeignKey(
                        name: "FK_RankingOverwrites_Teams_AssignTeamTournamentId_AssignTeamId",
                        columns: x => new { x.AssignTeamTournamentId, x.AssignTeamId },
                        principalSchema: "turnierplan",
                        principalTable: "Teams",
                        principalColumns: new[] { "TournamentId", "Id" },
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_RankingOverwrites_Tournaments_TournamentId",
                        column: x => x.TournamentId,
                        principalSchema: "turnierplan",
                        principalTable: "Tournaments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_RankingOverwrites_AssignTeamTournamentId_AssignTeamId",
                schema: "turnierplan",
                table: "RankingOverwrites",
                columns: new[] { "AssignTeamTournamentId", "AssignTeamId" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "RankingOverwrites",
                schema: "turnierplan");
        }
    }
}
