namespace Turnierplan.Core.Tournament.TeamSelectors;

public sealed record StaticTeamSelector(int TargetTeamId) : TeamSelectorBase
{
    public override Team? GetTargetTeam(Tournament tournament)
    {
        return TargetTeamId > 0 ? tournament._teams.FirstOrDefault(x => x.Id == TargetTeamId) : null;
    }
}
