using Turnierplan.Core.SeedWork;
using Turnierplan.Core.Tournament;

namespace Turnierplan.Core.PlanningRealm;

public sealed class TeamLink : Entity<long>
{
    public TeamLink(PlanningRealm planningRealm, ApplicationTeam applicationTeam, Tournament.Tournament tournament, Team team)
    {
        if (planningRealm.Organization is null || planningRealm.Organization != tournament.Organization)
        {
            throw new ArgumentException($"Both {planningRealm} and {tournament} must belong to the same {nameof(Organization.Organization)} which may not be null.");
        }

        if (!planningRealm._applications.Any(application => application._teams.Contains(applicationTeam)))
        {
            throw new ArgumentException($"The {applicationTeam} must belong to the {planningRealm}");
        }

        if (!tournament._teams.Contains(team))
        {
            throw new ArgumentException($"The {team} must belong to the {tournament}.");
        }

        Id = 0;
        CreatedAt = DateTime.UtcNow;
        PlanningRealm = planningRealm;
        ApplicationTeam = applicationTeam;
        Tournament = tournament;
        Team = team;
    }

    internal TeamLink(long id, DateTime createdAt)
    {
        Id = id;
        CreatedAt = createdAt;
    }

    public override long Id { get; protected set; }

    public DateTime CreatedAt { get; }

    public PlanningRealm PlanningRealm { get; internal set; } = null!;

    public ApplicationTeam ApplicationTeam { get; internal set; } = null!;

    public Tournament.Tournament Tournament { get; internal set; } = null!;

    public Team Team { get; internal set; } = null!;
}
