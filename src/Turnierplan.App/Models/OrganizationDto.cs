using Turnierplan.Core.PublicId;

namespace Turnierplan.App.Models;

public sealed record OrganizationDto
{
    public required PublicId Id { get; init; }

    public required string RbacScopeId { get; init; }

    public required string Name { get; init; }
}
