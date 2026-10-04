using Microsoft.AspNetCore.Mvc;
using Turnierplan.Server.Domain.Organizations;
using Turnierplan.Server.Domain.RoleAssignments;
using Turnierplan.Server.Infrastructure.Authentication;
using Turnierplan.Server.Infrastructure.Authorization;
using Turnierplan.Server.Infrastructure.Database;
using Turnierplan.Server.Infrastructure.Endpoints;

namespace Turnierplan.Server.Application.Organizations.CreateOrganization;

internal sealed class CreateOrganizationEndpoint : EndpointBase
{
    public CreateOrganizationEndpoint()
        : base(HttpMethod.Post, "/organizations", PolicyNames.CreateOrganization)
    {
    }

    public static async Task<IResult> HandleAsync(
        [FromBody] CreateOrganizationRequest request,
        PrincipalProvider principalProvider,
        TurnierplanContext context,
        ResponseRolesStore responseRolesStore,
        CancellationToken cancellationToken)
    {
        var organization = new Organization(request.Name);

        var principal = principalProvider.GetCurrentPrincipal();
        var roleAssignment = new RoleAssignment<Organization>(organization, Role.Owner, principal.Kind, principal.Id);

        await context.Organizations.AddAsync(organization, cancellationToken);
        await context.OrganizationRoleAssignments.AddAsync(roleAssignment, cancellationToken);

        await context.SaveChangesAsync(cancellationToken);

        await responseRolesStore.AddRolesToResponseHeaderAsync(organization, cancellationToken);

        return Results.Ok(new CreateOrganizationResponse
        {
            Id = organization.PublicId,
            ScopeId = ScopeIdentifier.Create(organization),
            Name = organization.Name
        });
    }
}
