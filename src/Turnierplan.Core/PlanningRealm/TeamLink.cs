using Turnierplan.Core.SeedWork;
using Turnierplan.Core.Tournament;

namespace Turnierplan.Core.PlanningRealm;

public sealed class TeamLink : Entity<long>
{
    public TeamLink(ApplicationTeam applicationTeam, Team team)
    {
        var planningRealm = applicationTeam.Application.PlanningRealm;
        var tournament = team.Tournament;

        if (planningRealm.Organization is null || planningRealm.Organization != tournament.Organization)
        {
            throw new ArgumentException("Both the planning realm and the tournament must belong to the same organization which may not be null.");
        }

        Id = 0;
        CreatedAt = DateTime.UtcNow;
        ApplicationTeam = applicationTeam;
        Team = team;
    }

    internal TeamLink(long id, DateTime createdAt)
    {
        Id = id;
        CreatedAt = createdAt;
    }

    public override long Id { get; protected set; }

    public DateTime CreatedAt { get; }

    public ApplicationTeam ApplicationTeam { get; internal set; } = null!;

    public Team Team { get; internal set; } = null!;
}
