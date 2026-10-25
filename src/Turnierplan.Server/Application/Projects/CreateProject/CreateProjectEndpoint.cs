using Microsoft.AspNetCore.Mvc;
using Turnierplan.Server.Domain.Organizations;
using Turnierplan.Server.Infrastructure.Authorization;
using Turnierplan.Server.Infrastructure.Database;
using Turnierplan.Server.Infrastructure.Database.Extensions;
using Turnierplan.Server.Infrastructure.Endpoints;

namespace Turnierplan.Server.Application.Projects.CreateProject;

internal sealed class CreateProjectEndpoint : EndpointBase<CreateProjectResponse>
{
    public CreateProjectEndpoint()
        : base(HttpMethod.Post, "/projects", PolicyNames.CreateOrganization)
    {
    }

    public static async Task<IResult> HandleAsync(
        [FromBody, Permission(typeof(Organization), Permission.GenericWrite, nameof(CreateProjectRequest.OrganizationId))] CreateProjectRequest request,
        TurnierplanContext context,
        ResponseRolesStore responseRolesStore,
        CancellationToken cancellationToken)
    {
        var organization = await context.Organizations.FindByPublicIdAsync(request.OrganizationId, cancellationToken);

        if (organization is null)
        {
            return Results.NotFound();
        }

        var project = organization.AddProject(request.Name);

        await context.SaveChangesAsync(cancellationToken);

        await responseRolesStore.AddRolesToResponseHeaderAsync(project, cancellationToken);

        return Results.Ok(new CreateProjectResponse
        {
            Id = project.PublicId,
            ScopeId = ScopeIdentifier.Create(project),
            Name = project.Name
        });
    }
}
