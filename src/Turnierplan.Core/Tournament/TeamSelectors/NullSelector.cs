namespace Turnierplan.Core.Tournament.TeamSelectors;

public sealed record NullSelector : TeamSelectorBase
{
    public override Team? GetTargetTeam(Tournament tournament)
    {
        return default;
    }
}
