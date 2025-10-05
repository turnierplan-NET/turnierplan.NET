using Turnierplan.Core.SeedWork;

namespace Turnierplan.Core.PlanningRealm;

public sealed class ApplicationChangeLog : Entity<long>
{
    internal ApplicationChangeLog(long id, DateTime timestamp, ApplicationChangeLogType type)
    {
        Id = id;
        Timestamp = timestamp;
        Type = type;
    }

    internal ApplicationChangeLog(Application application, ApplicationChangeLogType type, IEnumerable<Property> properties)
    {
        Id = 0;
        Application = application;
        Timestamp = DateTime.UtcNow;
        Type = type;
        Properties = [..properties];
    }

    public override long Id { get; protected set; }

    public Application Application { get; internal set; } = null!;

    public DateTime Timestamp { get; }

    public ApplicationChangeLogType Type { get; }

    public IReadOnlyList<Property> Properties { get; internal set; }

    public sealed record Property(ApplicationChangeLogProperty Type, string Value);
}
