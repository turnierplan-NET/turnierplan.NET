namespace Turnierplan.Adapter.Models;

/// <summary>
/// Represents a team's statistics in the context of an entire tournament.
/// </summary>
public sealed record TeamStatistics
{
    /// <summary>
    /// The accumulated achieved score of the respective team in all matches.
    /// </summary>
    public required int ScoreFor { get; init; }

    /// <summary>
    /// The accumulated achieved score of the respective team's opponents in all matches.
    /// </summary>
    public required int ScoreAgainst { get; init; }

    /// <summary>
    /// The number of finished matches that the respective team has played.
    /// </summary>
    public required int MatchesPlayed { get; init; }

    /// <summary>
    /// The number of finished matches where the respective team has won.
    /// </summary>
    public required int MatchesWon { get; init; }

    /// <summary>
    /// The number of finished matches of the respective team which have ended in a draw.
    /// </summary>
    public required int MatchesDrawn { get; init; }

    /// <summary>
    /// The number of finished matches where the respective team has lost.
    /// </summary>
    public required int MatchesLost { get; init; }
}
