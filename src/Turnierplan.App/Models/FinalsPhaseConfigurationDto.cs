namespace Turnierplan.App.Models;

public sealed record FinalsPhaseConfigurationDto
{
    public ScheduleConfigurationDto? Schedule { get; init; }

    public required short NumberOfCourts { get; init; }

    public required int FirstFinalsRound { get; init; }

    public required bool ThirdPlacePlayoff { get; init; }

    public required AdditionalPlayoffDto[] AdditionalPlayoffs { get; init; }
}
