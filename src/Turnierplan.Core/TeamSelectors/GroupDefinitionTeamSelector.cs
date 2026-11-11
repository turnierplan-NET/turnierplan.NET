namespace Turnierplan.Core.TeamSelectors;

public sealed record GroupDefinitionTeamSelector(Group TargetGroup, int TargetTeamIndex) : TeamSelectorBase
{
    public override Team? GetTargetTeam()
    {
        if (TargetTeamIndex < 0 || TargetTeamIndex >= TargetGroup._participants.Count)
        {
            return null;
        }

        return TargetGroup._participants[TargetTeamIndex].Team;
    }
}
