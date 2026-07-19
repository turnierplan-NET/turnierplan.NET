using Turnierplan.Core.PublicId;

namespace Turnierplan.App.Models;

public sealed record ResourcePlannerDto
{
    public required PublicId Id { get; init; }

    public required PublicId OrganizationId { get; init; }

    public required string RbacScopeId { get; init; }

    public required string Name { get; init; }

    public required ResourceDto[] Resources { get; init; }

    public required ResourceGroupDto[] ResourceGroups { get; init; }

    public required ResourcePlannerViewDto[] Views { get; init; }
}
