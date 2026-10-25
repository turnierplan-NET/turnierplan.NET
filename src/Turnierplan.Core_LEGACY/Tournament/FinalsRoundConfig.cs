using System.Text.Json.Serialization;

namespace Turnierplan.Core.Tournament;

public sealed record FinalsRoundConfig
{
    [JsonPropertyName("fo")]
    public required FinalsRoundOrder FirstFinalsRoundOrder { get; set; }

    [JsonPropertyName("3rd")]
    public required bool EnableThirdPlacePlayoff { get; set; }

    [JsonPropertyName("ts")]
    public List<string>? TeamSelectors { get; set; }

    [JsonPropertyName("ap")]
    public List<AdditionalPlayoffConfig>? AdditionalPlayoffs { get; set; }
}

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
