namespace Turnierplan.Core.Tournament.TeamSelectors;

public sealed record GroupResultsSelector(int TargetGroupId, int TargetGroupPosition) : TeamSelectorBase
{
    public override Team? GetTargetTeam(Tournament tournament)
    {
        var group = tournament._groups.FirstOrDefault(x => x.Id == TargetGroupId);

        if (group is null)
        {
            return null;
        }

        if (tournament._matches.Where(x => x.Group?.Id == TargetGroupId).Any(x => !x.IsFinished))
        {
            return null;
        }

        var result = group._participants.SingleOrDefault(x => x.Statistics.Position == TargetGroupPosition);

        return result?.Team;
    }
}
