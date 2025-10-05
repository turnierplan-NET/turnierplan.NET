using Turnierplan.Core.PlanningRealm;

namespace Turnierplan.App.Models;

public sealed record ApplicationChangeLogDto
{
    public required long Id { get; init; }

    public required DateTime Timestamp { get; init; }

    public required ApplicationChangeLogType Type { get; init; }

    public required Dictionary<ApplicationChangeLogProperty, string> AdditionalProperties { get; init; }
}
