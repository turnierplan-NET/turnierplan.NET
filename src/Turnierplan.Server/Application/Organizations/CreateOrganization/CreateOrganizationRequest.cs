namespace Turnierplan.Server.Application.Organizations.CreateOrganization;

internal sealed record CreateOrganizationRequest
{
    public required string Name { get; init; }
}