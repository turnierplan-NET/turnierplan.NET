using Turnierplan.Core.SeedWork;

namespace Turnierplan.Core.PlanningRealm;

public sealed class TournamentClass : Entity<long>
{
    internal TournamentClass(long id, string name)
    {
        Id = id;
        Name = name;
    }

    internal TournamentClass(string name)
    {
        Id = 0;
        Name = name;
    }

    public override long Id { get; protected set; }

    public string Name { get; set; }

    // TODO: At some point, this entity will need to reference Tournaments
}
