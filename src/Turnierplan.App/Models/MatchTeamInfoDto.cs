namespace Turnierplan.App.Models;

public sealed record MatchTeamInfoDto
{
    public required TeamSelectorDto TeamSelector { get; init; }

    public required int? TeamId { get; init; }

    public required int? Score { get; init; }
}
