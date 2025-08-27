using Turnierplan.Core.PlanningRealm;
using Turnierplan.Core.SeedWork;

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

    public TeamLink? TeamLink { get; set; }

    public string Name { get; set; }

    public bool OutOfCompetition { get; set; }

    public DateTime? EntryFeePaidAt { get; set; }

    public int? Ranking { get; internal set; }
}
