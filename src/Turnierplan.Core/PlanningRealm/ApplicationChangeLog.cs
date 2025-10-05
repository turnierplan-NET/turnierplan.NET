using Turnierplan.Core.SeedWork;

namespace Turnierplan.Core.PlanningRealm;

public sealed class ApplicationChangeLog : Entity<long>
{
    internal ApplicationChangeLog(long id, DateTime timestamp, ApplicationChangeLogType type, string? oldValue, string? newValue, IReadOnlyDictionary<string, string>? properties)
    {
        Id = id;
        Timestamp = timestamp;
        Type = type;
        OldValue = oldValue;
        NewValue = newValue;
        Properties = properties;
    }

    internal ApplicationChangeLog(Application application, ApplicationChangeLogType type, string? oldValue, string? newValue, Dictionary<string, string>? properties)
    {
        Id = 0;
        Application = application;
        Timestamp = DateTime.UtcNow;
        Type = type;
        OldValue = oldValue;
        NewValue = newValue;
        Properties = properties?.ToDictionary();
    }

    public override long Id { get; protected set; }

    public Application Application { get; internal set; } = null!;

    public DateTime Timestamp { get; }

    public ApplicationChangeLogType Type { get; }

    public string? OldValue { get; }

    public string? NewValue { get; }

    public IReadOnlyDictionary<string, string>? Properties { get; }
}
