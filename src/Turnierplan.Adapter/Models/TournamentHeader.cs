using Turnierplan.Adapter.Enums;

namespace Turnierplan.Adapter.Models;

/// <summary>
/// Represents a single tournament but does not include any specific data for brevity.
/// </summary>
public sealed record TournamentHeader
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
}
