namespace Turnierplan.Core;

public sealed class RankingOverwrite
{
    internal RankingOverwrite(int placementRank, bool hideRanking)
    {
        PlacementRank = placementRank;
        HideRanking = hideRanking;
    }

    public int PlacementRank { get; }

    public bool HideRanking { get; }

    public Team? AssignTeam { get; internal set; }
}
