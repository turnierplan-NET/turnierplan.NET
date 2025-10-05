using System.Text.Json.Serialization;
using Turnierplan.Core.SeedWork;

namespace Turnierplan.Core.PlanningRealm;

public sealed class ApplicationChangeLog : Entity<long>
{
    internal readonly List<Property> _properties;

    internal ApplicationChangeLog(long id, DateTime timestamp, ApplicationChangeLogType type)
    {
        Id = id;
        Timestamp = timestamp;
        Type = type;

        _properties = [];
    }

    internal ApplicationChangeLog(Application application, ApplicationChangeLogType type, IEnumerable<Property> properties)
    {
        Id = 0;
        Application = application;
        Timestamp = DateTime.UtcNow;
        Type = type;

        _properties = [..properties];
    }

    public override long Id { get; protected set; }

    public Application Application { get; internal set; } = null!;

    public DateTime Timestamp { get; }

    public ApplicationChangeLogType Type { get; }

    public IReadOnlyList<Property> Properties => _properties.AsReadOnly();

    public sealed record Property([property:JsonPropertyName("t")] ApplicationChangeLogProperty Type, [property:JsonPropertyName("v")] string Value);
}
