namespace Turnierplan.Core.Tournament.TeamSelectors;

public sealed record GroupResultsNthRankedSelector(int[] TargetGroupIds, int OrdinalNumber, int PlacementRank) : TeamSelectorBase
{
    public override Team? GetTargetTeam(Tournament tournament)
    {
        if (OrdinalNumber < 0 || PlacementRank < 1)
        {
            return null;
        }

        var eligibleTeams = new List<GroupParticipant>();

        foreach (var groupId in TargetGroupIds)
        {
            var group = tournament._groups.FirstOrDefault(x => x.Id == groupId);

            if (group is null)
            {
                continue;
            }

            if (tournament._matches.Where(x => x.Group?.Id == groupId).Any(x => !x.IsFinished))
            {
                return null;
            }

            var results = group._participants.SingleOrDefault(x => x.Statistics.Position == PlacementRank);

            if (results == null)
            {
                continue;
            }

            eligibleTeams.Add(results);
        }

        if (OrdinalNumber >= eligibleTeams.Count)
        {
            return null;
        }

        eligibleTeams.Sort(tournament._teamComparer);

        return eligibleTeams[OrdinalNumber].Team;
    }

    public bool Equals(GroupResultsNthRankedSelector? other)
    {
        if (ReferenceEquals(null, other))
        {
            return false;
        }

        if (ReferenceEquals(this, other))
        {
            return true;
        }

        return OrdinalNumber == other.OrdinalNumber
            && PlacementRank == other.PlacementRank
            && !TargetGroupIds.Except(other.TargetGroupIds).Any();
    }

    public override int GetHashCode()
    {
        var groupIdsHashCode = TargetGroupIds.Order()
            .Aggregate(TargetGroupIds.Length, (current, groupId) => unchecked(current * 31 + groupId));

        return (groupIdsHashCode, OrdinalNumber, PlacementRank).GetHashCode();
    }
}
