namespace Turnierplan.Core.TeamSelectors;

public sealed record GroupResultsNthRankedSelector(IReadOnlyCollection<Group> TargetGroups, int OrdinalNumber, int PlacementRank) : TeamSelectorBase
{
    public override Team? GetTargetTeam(Tournament tournament)
    {
        if (OrdinalNumber < 0 || PlacementRank < 1)
        {
            return null;
        }

        var eligibleTeams = new List<GroupParticipant>();

        foreach (var group in TargetGroups)
        {
            if (!tournament._groups.Contains(group))
            {
                throw new TournamentException("The team selector refers a group that is not part of the tournament.");
            }

            if (tournament._matches.Where(x => x.Group == group).Any(x => !x.IsFinished))
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
            && !TargetGroups.Except(other.TargetGroups).Any();
    }

    public override int GetHashCode()
    {
        var groupIdsHashCode = TargetGroups.Order()
            .Aggregate(TargetGroups.Count, (current, group) => unchecked(current * 31 + group.GetHashCode()));

        return HashCode.Combine(groupIdsHashCode, OrdinalNumber, PlacementRank);
    }
}
