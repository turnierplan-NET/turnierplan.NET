namespace Turnierplan.Adapter.Models;

/// <summary>
/// Value object which holds a single <see cref="Team"/>s statistics in the context
/// of the <see cref="Group"/> to which this team is assigned to.
/// </summary>
public sealed record TeamGroupStatistics
{
    /// <summary>
    /// The position of the team in the group, starting at <c>1</c> for the best team and counting up.
    /// </summary>
    public required int Position { get; init; }

    /// <summary>
    /// The accumulated achieved score of the respective team in all group matches.
    /// </summary>
    public required int ScoreFor { get; init; }

    /// <summary>
    /// The accumulated achieved score of the respective team's opponents in all group matches.
    /// </summary>
    public required int ScoreAgainst { get; init; }

    /// <summary>
    /// The difference between <see cref="ScoreFor"/> and <see cref="ScoreAgainst"/>, calculated as <c>ScoreFor - ScoreAgainst</c>.
    /// </summary>
    public required int ScoreDifference { get; init; }

    /// <summary>
    /// The number of finished group matches that the respective team has played.
    /// </summary>
    public required int MatchesPlayed { get; init; }

    /// <summary>
    /// The number of finished group matches where the respective team has won.
    /// </summary>
    public required int MatchesWon { get; init; }

    /// <summary>
    /// The number of finished group matches of the respective team which have ended in a draw.
    /// </summary>
    public required int MatchesDrawn { get; init; }

    /// <summary>
    /// The number of finished group matches where the respective team has lost.
    /// </summary>
    public required int MatchesLost { get; init; }

    /// <summary>
    /// The number of points awarded for the respective team's group match outcomes.
    /// </summary>
    public required int Points { get; init; }
}
