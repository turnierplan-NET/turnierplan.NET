using Turnierplan.Adapter.Enums;

namespace Turnierplan.Adapter.Models;

/// <summary>
/// Represents a single tournament consisting of <see cref="Group"/>s, <see cref="Match"/>es and <see cref="Ranking"/>s
/// </summary>
public sealed record Tournament
{
    /// <summary>
    /// The ID of the tournament which is an 11-character sequence of <c>[A-Za-z0-9_-]</c>.
    /// </summary>
    public required string Id { get; init; }

    /// <summary>
    /// The name of the tournament.
    /// </summary>
    public required string Name { get; init; }

    /// <summary>
    /// The name of the organization which owns the tournament.
    /// </summary>
    public required string OrganizationName { get; init; }

    /// <summary>
    /// The name of the folder to which this tournament is assigned or <c>null</c> if not applicable.
    /// </summary>
    public required string? FolderName { get; init; }

    /// <summary>
    /// The visibility of the tournament.
    /// </summary>
    public required Visibility Visibility { get; init; }

    /// <summary>
    /// The number of page views i.e. how often this tournament was viewed via its public URL.
    /// </summary>
    public required int PublicPageViews { get; init; }

    /// <summary>
    /// The teams of this tournament.
    /// </summary>
    public required Team[] Teams { get; init; }

    /// <summary>
    /// The groups of this tournament.
    /// </summary>
    public required Group[] Groups { get; init; }

    /// <summary>
    /// The matches of this tournament.
    /// </summary>
    public required Match[] Matches { get; init; }

    /// <summary>
    /// The rankings of this tournament.
    /// </summary>
    public required Ranking[] Rankings { get; init; }
}
