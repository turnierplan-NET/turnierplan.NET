using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Serialization;
using Turnierplan.Adapter.Enums;
using MatchType = Turnierplan.Adapter.Enums.MatchType;

namespace Turnierplan.Adapter.Models;

/// <summary>
/// Represents a single match in the context of a <see cref="Tournament"/>. A match has two participants
/// which are computed by evaluating the team selectors. Once the match is finished, each participant is
/// assigned a score.
/// </summary>
public sealed record Match
{
    /// <summary>
    /// The numerical ID of this match which is unique in the scope of a single tournament.
    /// </summary>
    public required int Id { get; init; }

    /// <summary>
    /// Ths index of this match. Upon match plan generation, all matches are assigned sequential indices
    /// starting at <c>1</c>.
    /// </summary>
    public required int Index { get; init; }

    /// <summary>
    /// The index of the court where this match is played, starting at <c>0</c>.
    /// </summary>
    public required short Court { get; init; }

    /// <summary>
    /// The kickoff time of this match, or <c>null</c> if no kickoff time was defined.
    /// </summary>
    public DateTime? Kickoff { get; init; }

    /// <summary>
    /// The type of this match.
    /// </summary>
    public required MatchType Type { get; init; }

    /// <summary>
    /// The type of this match as a human-readable, localized value.
    /// </summary>
    /// <example><c>Group Match</c> or <c>Semi-Final</c></example>
    public required string FormattedType { get; init; }

    /// <summary>
    /// The <see cref="Group.Id"/> of the <see cref="Group"/> if this match's outcome counts towards
    /// the group statistics or <c>null</c> if this match is a deciding/ranking match.
    /// </summary>
    /// <seealso cref="IsGroupMatch"/>
    public int? GroupId { get; init; }

    /// <summary>
    /// <c>true</c> if the outcome of this match contributes to some group's statistics or <c>false</c>
    /// if this match is a deciding/ranking match.
    /// </summary>
    /// <seealso cref="GroupId"/>
    [JsonIgnore]
    [MemberNotNullWhen(true, nameof(GroupId))]
    public bool IsGroupMatch => GroupId is not null;

    /// <summary>
    /// The "home" team of this match.
    /// </summary>
    public required MatchTeamInfo TeamA { get; init; }

    /// <summary>
    /// The "aways" team of this match.
    /// </summary>
    public required MatchTeamInfo TeamB { get; init; }

    /// <summary>
    /// The state of this match, indicating whether the team's scores and <see cref="OutcomeType"/> are set.
    /// </summary>
    public required MatchState State { get; init; }

    /// <summary>
    /// The <see cref="MatchOutcomeType"/> of this match or <c>null</c> if this match is not finished yet.
    /// </summary>
    public MatchOutcomeType? OutcomeType { get; init; }
}
