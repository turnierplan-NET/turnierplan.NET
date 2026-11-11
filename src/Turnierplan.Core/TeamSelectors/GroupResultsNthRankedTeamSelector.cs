namespace Turnierplan.Core.TeamSelectors;

public sealed record GroupResultsNthRankedTeamSelector(IReadOnlyList<Group> TargetGroups, int OrdinalNumber, int PlacementRank) : TeamSelectorBase
{
    public override Team? GetTargetTeam()
    {
        if (OrdinalNumber < 0 || PlacementRank < 1 || TargetGroups.Count == 0)
        {
            return null;
        }

        var eligibleTeams = new List<GroupParticipant>();

        foreach (var group in TargetGroups)
        {
            if (group._tournament is null || group._tournament != TargetGroups[0]._tournament)
            {
                throw new TournamentException("Cannot access the tournament of one of the target group - or not all groups are from the same tournament.");
            }

            if (group._tournament._matches.Where(x => x.Group == group).Any(x => !x.IsFinished))
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

        // It is checked inside the loop that the _tournament is not null and identical for all groups.
        eligibleTeams.Sort(TargetGroups[0]._tournament!._teamComparer);

        return eligibleTeams[OrdinalNumber].Team;
    }

    public bool Equals(GroupResultsNthRankedTeamSelector? other)
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
            && !TargetGroups.Except(other.TargetGroups).Any();
    }

    public override int GetHashCode()
    {
        var groupIdsHashCode = TargetGroups
            .Select(group => group.GetHashCode())
            .Order()
            .Aggregate(TargetGroups.Count, (current, group) => unchecked(current * 31 + group));

        return HashCode.Combine(groupIdsHashCode, OrdinalNumber, PlacementRank);
    }
}
