using Turnierplan.Core.PlanningRealm;

namespace Turnierplan.App.Models;

public sealed record ApplicationChangeLogPropertyDto
{
    public required ApplicationChangeLogProperty Type { get; init; }

    public required string Value { get; init; }
}
