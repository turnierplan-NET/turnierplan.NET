using System.Text.Json.Serialization;

namespace Turnierplan.Core.Tournament;

public sealed record GroupRoundConfig
{
    [JsonPropertyName("o")]
    public required GroupMatchOrder GroupMatchOrder { get; set; }

    [JsonPropertyName("r")]
    public required int GroupPhaseRounds { get; set; }
}
