namespace Turnierplan.Core.TeamSelectors;

public sealed record GroupResultsSelector(Group TargetGroup, int TargetGroupPosition) : TeamSelectorBase
{
    public override Team? GetTargetTeam(Tournament tournament)
    {
        if (!tournament._groups.Contains(TargetGroup))
        {
            throw new TournamentException("The team selector refers a group that is not part of the tournament.");
        }

        if (tournament._matches.Where(x => x.Group == TargetGroup).Any(x => !x.IsFinished))
        {
            return null;
        }

        var result = TargetGroup._participants.SingleOrDefault(x => x.Statistics.Position == TargetGroupPosition);

        return result?.Team;
    }
}
