using Turnierplan.Server.Domain;

namespace Turnierplan.Server.Application.Organizations.GetOrganizations;

internal sealed record GetOrganizationsResponseEntry
{
    public required PublicId Id { get; init; }

    public required string ScopeId { get; init; }

    public required string Name { get; init; }
}