using Turnierplan.Core.SeedWork;

namespace Turnierplan.Core.PlanningRealm;

public sealed class ApplicationTeam : Entity<long>
{
    internal ApplicationTeam(long id)
    {
        Id = id;
    }

    public override long Id { get; protected set; }

    public TournamentClass Class { get; internal set; } = null!;

    // TODO: At some point, this entity will need to reference a Team
}
