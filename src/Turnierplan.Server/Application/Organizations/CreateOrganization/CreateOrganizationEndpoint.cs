using Microsoft.AspNetCore.Mvc;
using Turnierplan.Server.Domain.Organizations;
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
        TurnierplanContext context,
        CancellationToken cancellationToken)
    {
        var organization = new Organization(request.Name);

        // TODO: organization.AddRoleAssignment(Role.Owner, user.AsPrincipal());

        await context.Organizations.AddAsync(organization, cancellationToken);
        await context.SaveChangesAsync(cancellationToken);

        // TODO: accessValidator.AddRolesToResponseHeader(organization);

        return Results.Ok(new CreateOrganizationResponse
        {
            Id = organization.PublicId,
            ScopeId = ScopeIdentifier.Create(organization),
            Name = organization.Name
        });
    }
}
