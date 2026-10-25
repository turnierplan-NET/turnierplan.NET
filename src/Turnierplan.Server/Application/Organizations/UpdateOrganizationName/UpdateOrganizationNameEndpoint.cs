using Microsoft.AspNetCore.Mvc;
using Turnierplan.Server.Domain;
using Turnierplan.Server.Domain.Organizations;
using Turnierplan.Server.Infrastructure.Authorization;
using Turnierplan.Server.Infrastructure.Database;
using Turnierplan.Server.Infrastructure.Database.Extensions;
using Turnierplan.Server.Infrastructure.Endpoints;

namespace Turnierplan.Server.Application.Organizations.UpdateOrganizationName;

internal sealed class UpdateOrganizationNameEndpoint : EndpointBase
{
    public UpdateOrganizationNameEndpoint()
        : base(HttpMethod.Patch, "/organizations/{id}/name", PolicyNames.Default)
    {
    }

    public static async Task<IResult> HandleAsync(
        [FromRoute, Permission(typeof(Organization), Permission.PrivilegedWrite)] PublicId id,
        [FromBody] UpdateOrganizationNameRequest request,
        TurnierplanContext context,
        CancellationToken cancellationToken)
    {
        var organization = await context.Organizations.FindByPublicIdAsync(id, cancellationToken);

        if (organization is null)
        {
            return Results.NotFound();
        }

        organization.Name = request.Name;

        await context.SaveChangesAsync(cancellationToken);

        return Results.NoContent();
    }
}
