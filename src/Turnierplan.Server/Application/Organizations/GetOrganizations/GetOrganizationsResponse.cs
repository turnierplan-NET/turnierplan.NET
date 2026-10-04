namespace Turnierplan.Server.Application.Organizations.GetOrganizations;

internal sealed record GetOrganizationsResponse
{
    public required GetOrganizationsResponseItem[] Organizations { get; init; }
}
