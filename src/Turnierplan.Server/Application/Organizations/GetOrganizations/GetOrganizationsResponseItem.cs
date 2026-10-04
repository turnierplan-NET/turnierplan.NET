using Turnierplan.Server.Domain;

namespace Turnierplan.Server.Application.Organizations.GetOrganizations;

internal sealed record GetOrganizationsResponseItem
{
    public required PublicId Id { get; init; }

    public required string Scope { get; init; }

    public required string Name { get; init; }
}
