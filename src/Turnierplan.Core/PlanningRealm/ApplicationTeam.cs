using Turnierplan.Core.SeedWork;

namespace Turnierplan.Core.PlanningRealm;

public sealed class ApplicationTeam : Entity<long>
{
    internal ApplicationTeam(long id, string name)
    {
        Id = id;
        Name = name;
    }

    internal ApplicationTeam(Application application, TournamentClass @class, string name)
    {
        Id = 0;
        Application = application;
        Class = @class;
        Name = name;
    }

    public override long Id { get; protected set; }

    public string Name { get; set; }

    public Application Application { get; internal set; } = null!;

    public TournamentClass Class { get; internal set; } = null!;

    public TeamLink? TeamLink { get; set; }
}
