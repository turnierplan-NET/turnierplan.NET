using Microsoft.AspNetCore.Mvc;
using Turnierplan.App.Mapping;
using Turnierplan.App.Models;
using Turnierplan.App.Security;
using Turnierplan.Core.PublicId;
using Turnierplan.Dal.Repositories;

namespace Turnierplan.App.Endpoints.ResourcePlanners;

internal sealed class GetResourcePlannersEndpoint : EndpointBase<IEnumerable<ResourcePlannerHeaderDto>>
{
    protected override HttpMethod Method => HttpMethod.Get;

    protected override string Route => "/api/resource-planners";

    protected override Delegate Handler => Handle;

    private static async Task<IResult> Handle(
        [FromQuery] PublicId organizationId,
        IOrganizationRepository organizationRepository,
        IAccessValidator accessValidator,
        IMapper mapper)
    {
        var organization = await organizationRepository.GetByPublicIdAsync(organizationId.Value, IOrganizationRepository.Includes.ResourcePlanners);

        if (organization is null)
        {
            return Results.NotFound();
        }

        if (!accessValidator.IsActionAllowed(organization, Actions.GenericRead))
        {
            return Results.Forbid();
        }

        return Results.Ok(mapper.MapCollection<ResourcePlannerHeaderDto>(organization.ResourcePlanners));
    }
}
