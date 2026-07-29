using Microsoft.AspNetCore.Mvc;
using Turnierplan.App.Security;
using Turnierplan.Core.PublicId;
using Turnierplan.Dal.Repositories;

namespace Turnierplan.App.Endpoints.ResourcePlanners;

internal sealed class DeleteResourcePlannerEndpoint : EndpointBase
{
    protected override HttpMethod Method => HttpMethod.Delete;

    protected override string Route => "/api/resource-planners/{id}";

    protected override Delegate Handler => Handle;

    private static async Task<IResult> Handle(
        [FromRoute] PublicId id,
        IResourcePlannerRepository repository,
        IAccessValidator accessValidator,
        CancellationToken cancellationToken)
    {
        var resourcePlanner = await repository.GetByPublicIdAsync(id);

        if (resourcePlanner is null)
        {
            return Results.NotFound();
        }

        if (!accessValidator.IsActionAllowed(resourcePlanner, Actions.GenericWrite))
        {
            return Results.Forbid();
        }

        repository.Remove(resourcePlanner);

        await repository.UnitOfWork.SaveChangesAsync(cancellationToken);

        return Results.NoContent();
    }
}
