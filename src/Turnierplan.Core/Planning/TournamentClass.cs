using Turnierplan.Core.SeedWork;

namespace Turnierplan.Core.Planning;

public sealed class TournamentClass : Entity<int>
{
    internal TournamentClass(int id, string name, int? maxTeamCount)
    {
        Id = id;
        Name = name;
        MaxTeamCount = maxTeamCount;
    }

    public override int Id { get; protected set; }

    public string Name { get; set; }

    public int? MaxTeamCount { get; set; }
}
