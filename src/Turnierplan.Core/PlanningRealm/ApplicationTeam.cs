using Turnierplan.Core.SeedWork;

namespace Turnierplan.Core.PlanningRealm;

public sealed class ApplicationTeam : Entity<long>
{
    internal ApplicationTeam(long id, string name)
    {
        Id = id;
        Name = name;
    }

    internal ApplicationTeam(TournamentClass @class, string name)
    {
        Id = 0;
        Class = @class;
        Name = name;
    }

    public override long Id { get; protected set; }

    public string Name { get; set; }

    public TournamentClass Class { get; internal set; } = null!;
}
