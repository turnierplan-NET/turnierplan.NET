using System.Diagnostics.CodeAnalysis;
using Turnierplan.Adapter.Enums;

namespace Turnierplan.Adapter.Models;

/// <summary>
/// A ranking in the context of a <see cref="Tournament"/>. The number of rankings can differ from the number of teams.
/// Also, the sequence of <see cref="PlacementRank"/> does not necessarily have to start at 1, and it may also contain gaps.
/// </summary>
public sealed record Ranking
{
    /// <summary>
    /// The placement rank, starting at <c>1</c> for the first place and counting up.
    /// </summary>
    public required int PlacementRank { get; init; }

    /// <summary>
    /// The reason is a short explanation of why the specific team is selected / will be selected for this ranking position.
    /// </summary>
    public required RankingReason Reason { get; init; }

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
