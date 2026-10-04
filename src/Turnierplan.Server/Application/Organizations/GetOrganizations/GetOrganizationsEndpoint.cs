using Microsoft.EntityFrameworkCore;
using Turnierplan.Server.Infrastructure.Authentication;
using Turnierplan.Server.Infrastructure.Authorization;
using Turnierplan.Server.Infrastructure.Database;
using Turnierplan.Server.Infrastructure.Endpoints;

namespace Turnierplan.Server.Application.Organizations.GetOrganizations;

internal sealed class GetOrganizationsEndpoint : EndpointBase
{
    public GetOrganizationsEndpoint()
        : base(HttpMethod.Get, "/organizations", PolicyNames.Default)
    {
    }

    public static async Task<IResult> HandleAsync(
        PrincipalProvider principalProvider,
        TurnierplanContext context,
        ResponseRolesStore responseRolesStore,
        CancellationToken cancellationToken)
    {
        // TODO: Admins should always see all organizations

        var principal = principalProvider.GetCurrentPrincipal();

        var organizations = await context.OrganizationRoleAssignments
            .Where(ra => ra.PrincipalKind == principal.Kind && ra.PrincipalId == principal.Id)
            .Select(ra => ra.Scope)
            .Distinct()
            .ToArrayAsync(cancellationToken);

        // TODO: Implement pagination (at some point later)

        await responseRolesStore.AddRolesToResponseHeaderAsync(organizations, cancellationToken);

        return Results.Ok(new GetOrganizationsResponse
        {
            Entries =
            [
                .. organizations.Select(organization => new GetOrganizationsResponseEntry
                {
                    Id = organization.PublicId,
                    ScopeId = ScopeIdentifier.Create(organization),
                    Name = organization.Name
                })
            ]
        });
    }
}
