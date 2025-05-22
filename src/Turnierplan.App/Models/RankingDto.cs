namespace Turnierplan.App.Models;

public sealed record RankingDto
{
    public required int PlacementRank { get; init; }

    public required bool IsDefined { get; init; }

    public required int? TeamId { get; init; }
}
