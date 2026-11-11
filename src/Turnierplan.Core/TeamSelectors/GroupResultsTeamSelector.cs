namespace Turnierplan.Core.TeamSelectors;

public sealed record GroupResultsTeamSelector(Group TargetGroup, int TargetGroupPosition) : TeamSelectorBase
{
    public override Team? GetTargetTeam()
    {
        if (TargetGroup._tournament is null)
        {
            throw new TournamentException("Cannot access the tournament of the target group.");
        }

        if (TargetGroup._tournament._matches.Where(x => x.Group == TargetGroup).Any(x => !x.IsFinished))
        {
            return null;
        }

        var result = TargetGroup._participants.SingleOrDefault(x => x.Statistics.Position == TargetGroupPosition);

        return result?.Team;
    }
}
