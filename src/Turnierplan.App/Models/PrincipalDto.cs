using Turnierplan.Core.RoleAssignment;

namespace Turnierplan.App.Models;

public sealed record PrincipalDto
{
    public required PrincipalKind Kind { get; init; }

    public required Guid PrincipalId { get; init; }
}
