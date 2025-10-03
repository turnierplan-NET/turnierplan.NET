using Turnierplan.Core.SeedWork;

namespace Turnierplan.Core.PlanningRealm;

public sealed class ApplicationChangeLog : Entity<long>
{
    public override long Id { get; protected set; }

    public Application Application { get; internal set; } = null!;

    public DateTime Timestamp { get; }
}
