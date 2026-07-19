using Microsoft.AspNetCore.Mvc;
using Turnierplan.App.Mapping;
using Turnierplan.App.Models;
using Turnierplan.App.Security;
using Turnierplan.Core.PublicId;
using Turnierplan.Dal.Repositories;

namespace Turnierplan.App.Endpoints.ResourcePlanners;

internal sealed class GetResourcePlannerEndpoint : EndpointBase<ResourcePlannerDto>
{
    protected override HttpMethod Method => HttpMethod.Get;

    protected override string Route => "/api/resource-planners/{id}";

    protected override Delegate Handler => Handle;

    private static async Task<IResult> Handle(
        [FromRoute] PublicId id,
        IResourcePlannerRepository repository,
        IAccessValidator accessValidator,
        IMapper mapper)
    {
        var resourcePlanner = await repository.GetByPublicIdAsync(id, IResourcePlannerRepository.Includes.All);

        if (resourcePlanner is null)
        {
            return Results.NotFound();
        }

        if (!accessValidator.IsActionAllowed(resourcePlanner, Actions.GenericRead))
        {
            return Results.Forbid();
        }

        accessValidator.AddRolesToResponseHeader(resourcePlanner);

        return Results.Ok(mapper.Map<ResourcePlannerDto>(resourcePlanner));
    }
}
