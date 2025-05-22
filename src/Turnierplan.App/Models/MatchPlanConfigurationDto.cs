namespace Turnierplan.App.Models;

public sealed record MatchPlanConfigurationDto
{
    public DateTime? FirstMatchKickoff { get; init; }

    public GroupPhaseConfigurationDto? GroupPhaseConfig { get; init; }

    public TimeSpan? PauseBetweenGroupAndFinalsPhase { get; init; }

    public FinalsPhaseConfigurationDto? FinalsPhaseConfig { get; init; }
}
