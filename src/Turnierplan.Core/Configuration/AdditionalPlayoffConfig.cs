using System.Text.Json.Serialization;

namespace Turnierplan.Core.Configuration;

public sealed record AdditionalPlayoffConfig
{
    public AdditionalPlayoffConfig(int playoffPosition, string teamSelectorA, string teamSelectorB)
    {
        PlayoffPosition = playoffPosition;
        TeamSelectorA = teamSelectorA;
        TeamSelectorB = teamSelectorB;
    }

    [JsonPropertyName("p")]
    public int PlayoffPosition { get; set; }

    [JsonPropertyName("a")]
    public string TeamSelectorA { get; set; }

    [JsonPropertyName("b")]
    public string TeamSelectorB { get; set; }
}
