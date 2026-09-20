using Turnierplan.Core.ResourcePlanner;

namespace Turnierplan.App.Models;

public sealed record ResourceAssignmentDto
{
    public required long ResourceId { get; init; }

    public required ResourceAssignmentState State { get; init; }
}