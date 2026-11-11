namespace Turnierplan.Core.TeamSelectors;

public sealed record StaticTeamSelector(Team TargetTeam) : TeamSelectorBase
{
    public override Team GetTargetTeam()
    {
        return TargetTeam;
    }
}
