namespace Turnierplan.Core.Configuration;

public sealed record MatchPlanConfiguration
{
    public GroupRoundConfig? GroupRoundConfig { get; set; }

    public FinalsRoundConfig? FinalsRoundConfig { get; set; }

    public ScheduleConfig? ScheduleConfig { get; set; }
}
