using Turnierplan.Adapter.Enums;

namespace Turnierplan.Adapter.Models;

/// <summary>
/// Information about a tournament, excluding any game-specific information for brevity.
/// </summary>
public sealed record TournamentHeader
{
    /// <summary>
    /// The id of the tournament.
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
}
