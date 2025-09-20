namespace Turnierplan.Adapter.Models;

/// <summary>
/// Represents the relationship between a team which is assigned to a group.
/// </summary>
public sealed record GroupParticipant
{
    /// <summary>
    /// The numerical ID of the team.
    /// </summary>
    public required int TeamId { get; init; }

    /// <summary>
    /// The assigned priority of this team which is used when other means of comparing
    /// team results inside a group result in a draw.
    /// </summary>
    public required int Priority { get; init; }

    /// <summary>
    /// The computed statistics resulting from this team's group matches.
    /// </summary>
    public required TeamGroupStatistics Statistics { get; init; }
}
