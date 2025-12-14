namespace Turnierplan.App.Models;

public sealed record TeamStatisticsDto
{
    public required int ScoreFor { get; init; }

    public required int ScoreAgainst { get; init; }

    public required int MatchesPlayed { get; init; }

    public required int MatchesWon { get; init; }

    public required int MatchesDrawn { get; init; }

    public required int MatchesLost { get; init; }
}
