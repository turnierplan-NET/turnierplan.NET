using Turnierplan.Core.SeedWork;

namespace Turnierplan.Core.PlanningRealm;

public sealed class TournamentClass : Entity<long>
{
    internal TournamentClass(long id, string name, int? maxTeamCount)
    {
        Id = id;
        Name = name;
        MaxTeamCount = maxTeamCount;
    }

    internal TournamentClass(string name)
    {
        Id = 0;
        Name = name;
        MaxTeamCount = null;
    }

    public override long Id { get; protected set; }

    public string Name { get; set; }

    public int? MaxTeamCount { get; set; }

    // TODO: At some point, this entity will need to reference Tournaments
}
