using Turnierplan.Server.Domain;

namespace Turnierplan.Server.Application.Organizations.CreateOrganization;

internal sealed record CreateOrganizationResponse
{
    public required PublicId Id { get; init; }

    public required string ScopeId { get; init; }

    public required string Name { get; init; }
}
