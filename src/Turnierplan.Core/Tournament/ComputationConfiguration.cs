using System.Text.Json.Serialization;

namespace Turnierplan.Core.Tournament;

public sealed record ComputationConfiguration
{
    private static readonly IReadOnlyList<TeamComparisonMode> __defaultTeamComparisonModes =
    [
        TeamComparisonMode.ByPoints,
        TeamComparisonMode.ByScoreDifference,
        TeamComparisonMode.ByScore,
        TeamComparisonMode.ByDirectComparison
    ];

    [JsonPropertyName("w")]
    public int MatchWonPoints { get; set; } = 3;

    [JsonPropertyName("d")]
    public int MatchDrawnPoints { get; set; } = 1;

    [JsonPropertyName("l")]
    public int MatchLostPoints { get; set; } = 0;

    [JsonPropertyName("r")]
    public bool HigherScoreLoses { get; set; } = false;

    [JsonPropertyName("cmp")]
    public List<TeamComparisonMode> ComparisonModes { get; set; } = [..__defaultTeamComparisonModes];
}
