using Turnierplan.Core.RoleAssignment;

namespace Turnierplan.App.Models;

public sealed record RoleAssignmentDto
{
    public required Guid Id { get; init; }

    public required string ScopeId { get; init; }

    public required string ScopeName { get; init; }

    public required DateTime CreatedAt { get; init; }

    public required Role Role { get; init; }

    public required PrincipalDto Principal { get; init; }

    public required bool IsInherited { get; init; }
}
