using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Turnierplan.Server.Domain;
using Turnierplan.Server.Domain.Organizations;
using Turnierplan.Server.Infrastructure.Authorization;
using Turnierplan.Server.Infrastructure.Database;
using Turnierplan.Server.Infrastructure.Endpoints;

namespace Turnierplan.Server.Application.Projects.GetProjects;

internal sealed class GetProjectsEndpoint : EndpointBase<GetProjectsResponse>
{
    public GetProjectsEndpoint()
        : base(HttpMethod.Get, "/projects", PolicyNames.Default)
    {
    }

    public static async Task<IResult> HandleAsync(
        [FromQuery, Permission(typeof(Organization), Permission.GenericRead)] PublicId organizationId,
        TurnierplanContext context,
        ResponseRolesStore responseRolesStore,
        CancellationToken cancellationToken)
    {
        var projects = await context.Projects
            .Where(x => x.Organization.PublicId == organizationId)
            .ToArrayAsync(cancellationToken);

        // TODO: Implement pagination (at some point later)
        // TODO: Check if we need the roles in the response because evaluation can be quite expensive

        foreach (var project in projects)
        {
            await responseRolesStore.AddRolesToResponseHeaderAsync(project, cancellationToken);
        }

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
