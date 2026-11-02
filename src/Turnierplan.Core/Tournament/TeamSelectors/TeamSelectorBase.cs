namespace Turnierplan.Core.Tournament.TeamSelectors;

public abstract record TeamSelectorBase
{
    private protected TeamSelectorBase()
    {
    }

    public abstract Team? GetTargetTeam(Tournament tournament);
}
