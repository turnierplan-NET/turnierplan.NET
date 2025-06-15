using Turnierplan.Core.RoleAssignment;

namespace Turnierplan.App.Models;

public sealed record RoleAssignmentDto
{
    public required long Id { get; init; }

    public required string Scope { get; init; }

    public required DateTime CreatedAt { get; init; }

    public required Role Role { get; init; }

    public required PrincipalDto Principal { get; init; }

    public required string Description { get; init; }

    public required bool IsInherited { get; init; }

    public required string? InheritedFrom { get; init; }
}
