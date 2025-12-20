using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

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
                name: "RankingOverwrite",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    PlacementRank = table.Column<int>(type: "integer", nullable: false),
                    HideRanking = table.Column<bool>(type: "boolean", nullable: false),
                    AssignTeamTournamentId = table.Column<long>(type: "bigint", nullable: true),
                    AssignTeamId = table.Column<int>(type: "integer", nullable: true),
                    TournamentId = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RankingOverwrite", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RankingOverwrite_Teams_AssignTeamTournamentId_AssignTeamId",
                        columns: x => new { x.AssignTeamTournamentId, x.AssignTeamId },
                        principalSchema: "turnierplan",
                        principalTable: "Teams",
                        principalColumns: new[] { "TournamentId", "Id" });
                    table.ForeignKey(
                        name: "FK_RankingOverwrite_Tournaments_TournamentId",
                        column: x => x.TournamentId,
                        principalSchema: "turnierplan",
                        principalTable: "Tournaments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_RankingOverwrite_AssignTeamTournamentId_AssignTeamId",
                table: "RankingOverwrite",
                columns: new[] { "AssignTeamTournamentId", "AssignTeamId" });

            migrationBuilder.CreateIndex(
                name: "IX_RankingOverwrite_TournamentId",
                table: "RankingOverwrite",
                column: "TournamentId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "RankingOverwrite");
        }
    }
}
