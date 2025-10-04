using Turnierplan.Core.SeedWork;

namespace Turnierplan.Core.PlanningRealm;

public sealed class ApplicationChangeLog : Entity<long>
{
    internal ApplicationChangeLog(long id, DateTime timestamp, ApplicationChangeLogType type, string? oldValue, string? newValue)
    {
        Id = id;
        Timestamp = timestamp;
        Type = type;
        OldValue = oldValue;
        NewValue = newValue;
    }

    internal ApplicationChangeLog(Application application, ApplicationChangeLogType type, string? oldValue, string? newValue)
    {
        Id = 0;
        Application = application;
        Timestamp = DateTime.UtcNow;
        Type = type;
        OldValue = oldValue;
        NewValue = newValue;
    }

    public override long Id { get; protected set; }

    public Application Application { get; internal set; } = null!;

    public DateTime Timestamp { get; }

    public ApplicationChangeLogType Type { get; }

    public string? OldValue { get; }

    public string? NewValue { get; }
}
