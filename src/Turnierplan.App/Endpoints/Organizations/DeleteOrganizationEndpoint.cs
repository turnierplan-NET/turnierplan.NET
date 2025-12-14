using Microsoft.AspNetCore.Mvc;
using Turnierplan.App.Helpers;
using Turnierplan.App.Security;
using Turnierplan.Core.PublicId;
using Turnierplan.Dal.Repositories;

namespace Turnierplan.App.Endpoints.Organizations;

internal sealed class DeleteOrganizationEndpoint : EndpointBase
{
    protected override HttpMethod Method => HttpMethod.Delete;

    protected override string Route => "/api/organizations/{id}";

    protected override Delegate Handler => Handle;

    private static async Task<IResult> Handle(
        [FromRoute] PublicId id,
        IOrganizationRepository repository,
        IAccessValidator accessValidator,
        IDeletionHelper deletionHelper,
        CancellationToken cancellationToken)
    {
        var organization = await repository.GetByPublicIdAsync(id, IOrganizationRepository.Includes.Tournaments | IOrganizationRepository.Includes.Venues | IOrganizationRepository.Includes.Images | IOrganizationRepository.Includes.PlanningRealms);

        if (organization is null)
        {
            return Results.NotFound();
        }

        if (!accessValidator.IsActionAllowed(organization, Actions.PrivilegedDelete))
        {
            return Results.Forbid();
        }

        var result = await deletionHelper.DeleteOrganizationAsync(organization, cancellationToken);

        if (!result)
        {
            return Results.InternalServerError("The deletion of the organization failed. Please contact your administrator.");
        }

        return Results.NoContent();
    }
}
