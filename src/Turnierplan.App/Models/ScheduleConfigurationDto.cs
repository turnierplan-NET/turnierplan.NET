namespace Turnierplan.App.Models;

public sealed record ScheduleConfigurationDto
{
    public required TimeSpan PlayTime { get; init; }

    public required TimeSpan PauseTime { get; init; }
}
