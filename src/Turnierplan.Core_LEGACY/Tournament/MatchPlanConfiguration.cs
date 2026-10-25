using System.Text.Json.Serialization;

namespace Turnierplan.Core.Tournament;

public sealed record MatchPlanConfiguration
{
    [JsonPropertyName("gr")]
    public GroupRoundConfig? GroupRoundConfig { get; set; }

    [JsonPropertyName("fr")]
    public FinalsRoundConfig? FinalsRoundConfig { get; set; }

    [JsonPropertyName("sc")]
    public ScheduleConfig? ScheduleConfig { get; set; }
}
