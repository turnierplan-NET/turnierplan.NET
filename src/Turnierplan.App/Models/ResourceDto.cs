using Turnierplan.Core.ResourcePlanner;

namespace Turnierplan.App.Models;

public sealed record ResourceDto
{
    public required long Id { get; init; }

    public required ResourceType Type { get; init; }

    public required string Name { get; init; }

    public required string? Notes { get; init; }
}