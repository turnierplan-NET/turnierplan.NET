namespace Turnierplan.Server.Application.Organizations.GetOrganizations;

internal sealed record GetOrganizationsResponse
{
    public required GetOrganizationsResponseEntry[] Entries { get; init; }
}