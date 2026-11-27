namespace Turnierplan.Core.Tournament;

internal sealed class TeamSaves
{
    
    public void LinkWithApplicationTeam(ApplicationTeam applicationTeam)
    {
        if (applicationTeam.TeamLink is not null)
        {
            if (applicationTeam.TeamLink.Team == this)
            {
                return;
            }

            throw new TurnierplanException("Cannot link with application team because it already has a link to a different team.");
        }

        TeamLink = new TeamLink(applicationTeam, this);
        Name = applicationTeam.Name;

        applicationTeam.TeamLink = TeamLink;

        applicationTeam.Application.AddChangeLog(ApplicationChangeLogType.TeamLinkCreated, [
            new ApplicationChangeLog.Property(ApplicationChangeLogProperty.TeamName, applicationTeam.Name),
            new ApplicationChangeLog.Property(ApplicationChangeLogProperty.TournamentName, Tournament.Name)
        ]);
    }

    public void UnlinkApplicationTeam()
    {
        if (TeamLink is not null)
        {
            TeamLink.ApplicationTeam.TeamLink = null;

            TeamLink.ApplicationTeam.Application.AddChangeLog(ApplicationChangeLogType.TeamLinkDestroyed, [
                new ApplicationChangeLog.Property(ApplicationChangeLogProperty.TeamName, Name),
                new ApplicationChangeLog.Property(ApplicationChangeLogProperty.TournamentName, Tournament.Name)
            ]);
        }

        TeamLink = null;
    }
}
