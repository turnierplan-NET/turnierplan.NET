using Turnierplan.Server.Domain;

namespace Turnierplan.Server.Application.Organizations.GetOrganization;

internal sealed record GetOrganizationResponse
{
    public required PublicId Id { get; init; }

    public required string Scope { get; init; }

    public required string Name { get; init; }
}
