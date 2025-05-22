using System.Diagnostics.CodeAnalysis;

namespace Turnierplan.Adapter.Models;

/// <summary>
/// Represents a single ranking in the context of a <see cref="Tournament"/>. There are
/// equally many rankings in a tournament as there are teams.
/// </summary>
public sealed record Ranking
{
    /// <summary>
    /// The placement rank, starting at <c>1</c> for the first place and counting up.
    /// </summary>
    public required int PlacementRank { get; init; }

    /// <summary>
    /// Whether this ranking is currently defined or not.
    /// </summary>
    [MemberNotNullWhen(true, nameof(TeamId))]
    public required bool IsDefined { get; init; }

    /// <summary>
    /// The <see cref="Team.Id"/> of the <see cref="Team"/> which occupies this ranking or <c>null</c> if
    /// this ranking is currently not defined.
    /// </summary>
    public int? TeamId { get; init; }
}
