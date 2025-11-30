using Turnierplan.Core.Entity;
using Turnierplan.Core.Exceptions;
using Turnierplan.Core.PlanningRealm;

namespace Turnierplan.Core.Tournament;

public sealed class Team : Entity<int>
{
    internal Team(int id, Tournament tournament, string name, bool outOfCompetition = false, DateTime? entryFeePaidAt = null)
    {
        Id = id;
        Tournament = tournament;
        Name = name;
        OutOfCompetition = outOfCompetition;
        EntryFeePaidAt = entryFeePaidAt;
    }

    internal Team(int id, string name, bool outOfCompetition, DateTime? entryFeePaidAt)
    {
        Id = id;
        Name = name;
        OutOfCompetition = outOfCompetition;
        EntryFeePaidAt = entryFeePaidAt;
    }

    public override int Id { get; protected set; }

    public Tournament Tournament { get; internal set; } = null!;

    public TeamLink? TeamLink { get; internal set; }

    public string Name { get; internal set; }

    public bool OutOfCompetition { get; set; }

    public DateTime? EntryFeePaidAt { get; set; }

    public void SetName(string name)
    {
        if (TeamLink is not null)
        {
            throw new TurnierplanException("Cannot update team name if there is a link to an application team.");
        }

        Name = name.Trim();
    }

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
