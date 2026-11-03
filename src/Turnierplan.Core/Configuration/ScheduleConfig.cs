namespace Turnierplan.Core.Configuration;

public sealed record ScheduleConfig
{
    public required DateTime FirstMatchKickoff { get; set; }

    public required short GroupPhaseNumberOfCourts { get; set; }

    public required TimeSpan GroupPhasePlayTime { get; set; }

    public required TimeSpan GroupPhasePauseTime { get; set; }

    public required TimeSpan PauseBetweenGroupAndFinalsPhase { get; set; }

    public required short FinalsPhaseNumberOfCourts { get; set; }

    public required TimeSpan FinalsPhasePlayTime { get; set; }

    public required TimeSpan FinalsPhasePauseTime { get; set; }
}
