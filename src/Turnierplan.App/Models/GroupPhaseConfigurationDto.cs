namespace Turnierplan.App.Models;

public sealed record GroupPhaseConfigurationDto
{
    public ScheduleConfigurationDto? Schedule { get; init; }

    public required short NumberOfCourts { get; init; }

    public required bool UseAlternatingOrder { get; init; }

    public required int NumberOfGroupRounds { get; init; }
}
