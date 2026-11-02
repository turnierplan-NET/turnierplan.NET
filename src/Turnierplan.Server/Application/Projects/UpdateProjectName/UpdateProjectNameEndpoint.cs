using Microsoft.AspNetCore.Mvc;
using Turnierplan.Server.Domain;
using Turnierplan.Server.Domain.Projects;
using Turnierplan.Server.Infrastructure.Authorization;
using Turnierplan.Server.Infrastructure.Database;
using Turnierplan.Server.Infrastructure.Database.Extensions;
using Turnierplan.Server.Infrastructure.Endpoints;

namespace Turnierplan.Server.Application.Projects.UpdateProjectName;

internal sealed class UpdateProjectNameEndpoint : EndpointBase
{
    public UpdateProjectNameEndpoint()
        : base(HttpMethod.Patch, "/projects/{id}/name", PolicyNames.Default)
    {
    }

    public static async Task<IResult> HandleAsync(
        [FromRoute, Permission(typeof(Project), Permission.GenericWrite)] PublicId id,
        [FromBody] UpdateProjectNameRequest request,
        TurnierplanContext context,
        CancellationToken cancellationToken)
    {
        var project = await context.Projects.FindByPublicIdAsync(id, cancellationToken);

        if (project is null)
        {
            return Results.NotFound();
        }

        project.Name = request.Name;

        await context.SaveChangesAsync(cancellationToken);

        return Results.NoContent();
    }
}
