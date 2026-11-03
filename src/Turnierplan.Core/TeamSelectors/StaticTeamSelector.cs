namespace Turnierplan.Core.TeamSelectors;

public sealed record StaticTeamSelector(Team TargetTeam) : TeamSelectorBase
{
    public override Team GetTargetTeam(Tournament tournament)
    {
        if (!tournament._teams.Contains(TargetTeam))
        {
            throw new TournamentException("The team selector refers a team that is not part of the tournament.");
        }

        return TargetTeam;
    }
}
