namespace Turnierplan.Core.TeamSelectors;

public sealed record NullSelector : TeamSelectorBase
{
    public override Team? GetTargetTeam(Tournament tournament)
    {
        return null;
    }
}
