namespace Turnierplan.Adapter.Models;

/// <summary>
/// A team in the context of a <see cref="Tournament"/>.
/// </summary>
public sealed record Team
{
    /// <summary>
    /// The numerical ID of this team which is unique in the scope of a single tournament.
    /// </summary>
    public required int Id { get; init; }

    /// <summary>
    /// The name of this team.
    /// </summary>
    public required string Name { get; init; }

    /// <summary>
    /// Whether this team plays "out of competition". If it is <c>true</c>, this team will always
    /// be regarded worse compared to any other team when sorting teams in a group, for example.
    /// </summary>
    public required bool OutOfCompetition { get; init; }

    /// <summary>
    /// The computed statistics resulting from all matches this team has participated in.
    /// </summary>
    public required TeamStatistics Statistics { get; init; }
}
