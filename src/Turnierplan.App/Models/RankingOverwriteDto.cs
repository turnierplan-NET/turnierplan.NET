namespace Turnierplan.App.Models;

public sealed record RankingOverwriteDto
{
    public required int PlacementRank { get; init; }

    public required bool HideRanking { get; init; }

    public required int? AssignTeamId { get; init; }
}
