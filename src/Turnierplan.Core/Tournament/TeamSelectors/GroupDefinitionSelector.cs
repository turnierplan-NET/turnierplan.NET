namespace Turnierplan.Core.Tournament.TeamSelectors;

public sealed record GroupDefinitionSelector(int TargetGroupId, int TargetTeamIndex) : TeamSelectorBase
{
    public override Team? GetTargetTeam(Tournament tournament)
    {
        var group = tournament._groups.FirstOrDefault(x => x.Id == TargetGroupId);

        if (group is null)
        {
            return default;
        }

        if (TargetTeamIndex < 0 || TargetTeamIndex >= group._participants.Count)
        {
            return default;
        }

        return group._participants[TargetTeamIndex].Team;
    }
}
