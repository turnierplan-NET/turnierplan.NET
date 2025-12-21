using Turnierplan.Core.Entity;

namespace Turnierplan.Core.Tournament;

public sealed class RankingOverwrite : Entity<int>
{
    internal RankingOverwrite(int id, int placementRank, bool hideRanking)
    {
        Id = id;
        PlacementRank = placementRank;
        HideRanking = hideRanking;
    }

    public override int Id { get; protected set; }

    public int PlacementRank { get; }

    public bool HideRanking { get; }

    public Team? AssignTeam { get; internal set; }
}
