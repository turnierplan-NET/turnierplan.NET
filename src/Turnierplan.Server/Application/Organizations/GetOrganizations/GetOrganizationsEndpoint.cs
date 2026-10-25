using Microsoft.EntityFrameworkCore;
using Turnierplan.Server.Domain.Organizations;
using Turnierplan.Server.Infrastructure.Authentication;
using Turnierplan.Server.Infrastructure.Authentication.Extensions;
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
        HttpContext httpContext,
        PrincipalProvider principalProvider,
        TurnierplanContext context,
        ResponseRolesStore responseRolesStore,
        CancellationToken cancellationToken)
    {
        Organization[] organizations;

        if (httpContext.User.HasIsAdministratorClaim())
        {
            organizations = await context.Organizations.ToArrayAsync(cancellationToken);
        }
        else
        {
            var principal = principalProvider.GetCurrentPrincipal();

            organizations = await context.OrganizationRoleAssignments
                .Where(ra => ra.PrincipalKind == principal.Kind && ra.PrincipalId == principal.Id)
                .Select(ra => ra.Scope)
                .Distinct()
                .ToArrayAsync(cancellationToken);
        }

        // TODO: Implement pagination (at some point later)
        // TODO: Check if we need the roles in the response because evaluation can be quite expensive

        foreach (var organization in organizations)
        {
            await responseRolesStore.AddRolesToResponseHeaderAsync(organization, cancellationToken);
        }

        return Results.Ok(new GetOrganizationsResponse
        {
            Organizations =
            [
                .. organizations.Select(organization => new GetOrganizationsResponseItem
                {
                    Id = organization.PublicId,
                    Scope = ScopeIdentifier.Create(organization),
                    Name = organization.Name
                })
            ]
        });
    }
}
