using Microsoft.AspNetCore.Mvc;
using Turnierplan.Server.Domain;
using Turnierplan.Server.Domain.Organizations;
using Turnierplan.Server.Infrastructure.Authorization;
using Turnierplan.Server.Infrastructure.Database;
using Turnierplan.Server.Infrastructure.Database.Extensions;
using Turnierplan.Server.Infrastructure.Endpoints;

namespace Turnierplan.Server.Application.Organizations.GetOrganization;

internal sealed class GetOrganizationEndpoint : EndpointBase<GetOrganizationResponse>
{
    public GetOrganizationEndpoint()
        : base(HttpMethod.Get, "/organizations/{id}", PolicyNames.Default)
    {
    }

    public static async Task<IResult> HandleAsync(
        [FromRoute, Permission(typeof(Organization), Permission.GenericRead)] PublicId id,
        TurnierplanContext context,
        ResponseRolesStore responseRolesStore,
        CancellationToken cancellationToken)
    {
        var organization = await context.Organizations.FindByPublicIdAsync(id, cancellationToken);

        if (organization is null)
        {
            return Results.NotFound();
        }

        await responseRolesStore.AddRolesToResponseHeaderAsync(organization, cancellationToken);

        return Results.Ok(new GetOrganizationResponse
        {
            Id = organization.PublicId,
            Scope = ScopeIdentifier.Create(organization),
            Name = organization.Name
        });
    }
}
