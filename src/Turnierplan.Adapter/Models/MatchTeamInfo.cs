namespace Turnierplan.Adapter.Models;

/// <summary>
/// Represents the "home" or "away" team of a match. Depending on the state of the tournament, the specific <see cref="TeamId"/>
/// can be available or not. However, the <see cref="TeamSelector"/> is always available.
/// </summary>
public sealed record MatchTeamInfo
{
    /// <summary>
    /// The team selector for this team.
    /// </summary>
    public required TeamSelector TeamSelector { get; init; }

    /// <summary>
    /// The <see cref="Team.Id"/> of the <see cref="Team"/> or <c>null</c> if the team can currently not be evaluated.
    /// </summary>
    public int? TeamId { get; init; }

    /// <summary>
    /// The score of team participating team or <c>null</c> if this match is not finished yet.
    /// </summary>
    public int? Score { get; init; }
}
