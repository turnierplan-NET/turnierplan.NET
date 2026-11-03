namespace Turnierplan.Core.Configuration;

public sealed record ComputationConfiguration
{
    private static readonly IReadOnlyList<TeamComparisonMode> __defaultTeamComparisonModes =
    [
        TeamComparisonMode.ByPoints,
        TeamComparisonMode.ByScoreDifference,
        TeamComparisonMode.ByScore,
        TeamComparisonMode.ByDirectComparison
    ];

    public int MatchWonPoints { get; set; } = 3;

    public int MatchDrawnPoints { get; set; } = 1;

    public int MatchLostPoints { get; set; }

    public bool HigherScoreLoses { get; set; }

    public List<TeamComparisonMode> ComparisonModes { get; set; } = [..__defaultTeamComparisonModes];
}
