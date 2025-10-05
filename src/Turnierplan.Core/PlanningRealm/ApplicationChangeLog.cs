using Turnierplan.Core.SeedWork;

namespace Turnierplan.Core.PlanningRealm;

public sealed class ApplicationChangeLog : Entity<long>
{
    internal ApplicationChangeLog(long id, DateTime timestamp, ApplicationChangeLogType type, IReadOnlyDictionary<ApplicationChangeLogProperty, string> properties)
    {
        Id = id;
        Timestamp = timestamp;
        Type = type;
        Properties = properties;
    }

    internal ApplicationChangeLog(Application application, ApplicationChangeLogType type, Dictionary<ApplicationChangeLogProperty, string> properties)
    {
        Id = 0;
        Application = application;
        Timestamp = DateTime.UtcNow;
        Type = type;
        Properties = properties.ToDictionary();
    }

    public override long Id { get; protected set; }

    public Application Application { get; internal set; } = null!;

    public DateTime Timestamp { get; }

    public ApplicationChangeLogType Type { get; }

    public IReadOnlyDictionary<ApplicationChangeLogProperty, string> Properties { get; }
}
