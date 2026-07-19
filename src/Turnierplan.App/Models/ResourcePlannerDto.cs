using Turnierplan.Core.PublicId;

namespace Turnierplan.App.Models;

public sealed record ResourcePlannerDto
{
    public required PublicId Id { get; init; }

    public required PublicId OrganizationId { get; init; }

    public required string RbacScopeId { get; init; }

    public required string Name { get; init; }

    // groups
    // views
}

public sealed record ResourcePlannerHeaderDto
{
    public required PublicId Id { get; init; }

    public required PublicId OrganizationId { get; init; }

    public required string Name { get; init; }
}
