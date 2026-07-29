using Turnierplan.Core.ResourcePlanner;

namespace Turnierplan.App.Models;

public sealed record ResourceGroupDto
{
    public required long Id { get; init; }

    public required string Name { get; init; }

    public required string? Notes { get; init; }

    public required DateTime? Start { get; init; }

    public required DateTime? End { get; init; }

    public required ResourceAssignmentDto[] Assignment { get; init; }
}
