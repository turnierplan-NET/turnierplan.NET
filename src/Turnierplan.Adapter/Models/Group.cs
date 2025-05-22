namespace Turnierplan.Adapter.Models;

/// <summary>
/// Represents a single group in the context of a <see cref="Tournament"/>.
/// </summary>
public sealed record Group
{
    /// <summary>
    /// The numerical ID of this group which is unique in the scope of a single tournament.
    /// </summary>
    public required int Id { get; init; }

    /// <summary>
    /// The alphabetical ID of this group. This is always an upper-case character between <c>A</c> and <c>Z</c>.
    /// </summary>
    public required char AlphabeticalId { get; init; }

    /// <summary>
    /// The display name of this group which is either a custom display name configured for this group
    /// or the default group name containing the <see cref="AlphabeticalId"/>.
    /// </summary>
    /// <seealso cref="HasCustomDisplayName"/>
    public required string DisplayName { get; init; }

    /// <summary>
    /// Whether this group has a custom display name.
    /// </summary>
    public required bool HasCustomDisplayName { get; init; }

    /// <summary>
    /// An array of all participants assigned to this group.
    /// </summary>
    public required GroupParticipant[] Participants { get; init; }
}
