using Turnierplan.Core.SeedWork;

namespace Turnierplan.Core.Planning;

public sealed class ApplicationTeam : Entity<long>
{
    internal ApplicationTeam(long id, string name)
    {
        Id = id;
        Name = name;
    }

    public override long Id { get; protected set; }

    public string Name { get; set; }

    public TournamentClass Class { get; internal set; } = null!;

    // TODO: At some point, this entity will need to reference a Team
}
