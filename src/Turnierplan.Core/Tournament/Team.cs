using Turnierplan.Core.SeedWork;

namespace Turnierplan.Core.Tournament;

public sealed class Team : Entity<int>
{
    internal Team(int id, string name, bool outOfCompetition = false, DateTime? entryFeePaidAt = null)
    {
        Id = id;
        Name = name;
        OutOfCompetition = outOfCompetition;
        EntryFeePaidAt = entryFeePaidAt;
    }

    public override int Id { get; protected set; }

    public string Name { get; set; }

    public bool OutOfCompetition { get; set; }

    public DateTime? EntryFeePaidAt { get; set; }

    public int? Ranking { get; internal set; }
}
