using System.Text.Json.Serialization;

namespace Turnierplan.Core.Tournament;

public sealed record ScheduleConfig
{
    [JsonPropertyName("f")]
    public required DateTime FirstMatchKickoff { get; set; }

    [JsonPropertyName("gc")]
    public required short GroupPhaseNumberOfCourts { get; set; }

    [JsonPropertyName("gd")]
    public required TimeSpan GroupPhasePlayTime { get; set; }

    [JsonPropertyName("gp")]
    public required TimeSpan GroupPhasePauseTime { get; set; }

    [JsonPropertyName("p")]
    public required TimeSpan PauseBetweenGroupAndFinalsPhase { get; set; }

    [JsonPropertyName("fc")]
    public required short FinalsPhaseNumberOfCourts { get; set; }

    [JsonPropertyName("fd")]
    public required TimeSpan FinalsPhasePlayTime { get; set; }

    [JsonPropertyName("fp")]
    public required TimeSpan FinalsPhasePauseTime { get; set; }
}
