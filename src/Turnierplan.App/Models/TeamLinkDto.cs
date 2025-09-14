using Turnierplan.Core.PublicId;

namespace Turnierplan.App.Models;

public sealed record TeamLinkDto
{
    public required PublicId PlanningRealmId { get; init; }

    public required string PlanningRealmName { get; init; }

    public required string TournamentClassName { get; init; }

    public required long ApplicationTeamId { get; init; }
}
