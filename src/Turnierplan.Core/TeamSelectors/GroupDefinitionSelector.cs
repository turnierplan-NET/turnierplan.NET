namespace Turnierplan.Core.TeamSelectors;

public sealed record GroupDefinitionSelector(Group TargetGroup, int TargetTeamIndex) : TeamSelectorBase
{
    public override Team? GetTargetTeam(Tournament tournament)
    {
        if (!tournament._groups.Contains(TargetGroup))
        {
            throw new TournamentException("The team selector refers a group that is not part of the tournament.");
        }

        if (TargetTeamIndex < 0 || TargetTeamIndex >= TargetGroup._participants.Count)
        {
            return null;
        }

        return TargetGroup._participants[TargetTeamIndex].Team;
    }
}
