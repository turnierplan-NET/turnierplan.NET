using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Turnierplan.Server.Domain;
using Turnierplan.Server.Infrastructure.Authorization;
using Turnierplan.Server.Infrastructure.Database;
using Turnierplan.Server.Infrastructure.Endpoints;

namespace Turnierplan.Server.Application.Projects.GetProjects;

internal sealed class GetProjectsEndpoint : EndpointBase
{
    public GetProjectsEndpoint()
        : base(HttpMethod.Get, "/projects", PolicyNames.Default)
    {
    }

    public static async Task<IResult> HandleAsync(
        [FromQuery] PublicId organizationId,
        TurnierplanContext context,
        ResponseRolesStore responseRolesStore,
        CancellationToken cancellationToken)
    {
        // TODO: Implement permission check

        var projects = await context.Projects
            .Where(x => x.Organization.PublicId == organizationId)
            .ToArrayAsync(cancellationToken);

        await responseRolesStore.AddRolesToResponseHeaderAsync(projects, cancellationToken);

        return Results.Ok(new GetProjectsResponse
        {
            Projects =
            [
                .. projects.Select(project => new GetProjectsResponseItem
                {
                    Id = project.PublicId,
                    Scope = ScopeIdentifier.Create(project),
                    Name = project.Name
                })
            ]
        });
    }
}
