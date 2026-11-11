namespace Turnierplan.Core.TeamSelectors;

public sealed record NullTeamSelector : TeamSelectorBase
{
    public override Team? GetTargetTeam()
    {
        return null;
    }
}
