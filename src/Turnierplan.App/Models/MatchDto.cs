using Turnierplan.App.Models.Enums;
using Turnierplan.Core.Tournament;
using MatchType = Turnierplan.App.Models.Enums.MatchType;

namespace Turnierplan.App.Models;

public sealed record MatchDto
{
    public required int Id { get; init; }

    public required int Index { get; init; }

    public required short Court { get; init; }

    public required DateTime? Kickoff { get; init; }

    public required MatchType Type { get; init; }

    public required string FormattedType { get; init; }

    public required int? GroupId { get; init; }

    public required int? FinalsRound { get; init; }

    public required int? PlayoffPosition { get; init; }

    public required MatchTeamInfoDto TeamA { get; init; }

    public required MatchTeamInfoDto TeamB { get; init; }

    public required MatchState State { get; init; }

    public required MatchOutcomeType? OutcomeType { get; init; }
}
