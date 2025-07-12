using Microsoft.AspNetCore.Mvc;
using Turnierplan.App.Mapping;
using Turnierplan.App.Models;
using Turnierplan.App.Security;
using Turnierplan.Core.Organization;
using Turnierplan.Core.PublicId;

namespace Turnierplan.App.Endpoints.PlanningRealms;

internal sealed class GetPlanningRealmsEndpoint : EndpointBase<IEnumerable<PlanningRealmHeaderDto>>
{
    protected override HttpMethod Method => HttpMethod.Get;

    protected override string Route => "/api/planning-realms";

    protected override Delegate Handler => Handle;

    private static async Task<IResult> Handle(
        [FromQuery] PublicId organizationId,
        IOrganizationRepository organizationRepository,
        IAccessValidator accessValidator,
        IMapper mapper)
    {
        var organization = await organizationRepository.GetByPublicIdAsync(organizationId.Value, IOrganizationRepository.Include.PlanningRealms).ConfigureAwait(false);

        if (organization is null)
        {
            return Results.NotFound();
        }

        if (!accessValidator.IsActionAllowed(organization, Actions.GenericRead))
        {
            return Results.Forbid();
        }

        return Results.Ok(mapper.MapCollection<PlanningRealmHeaderDto>(organization.PlanningRealms));
    }
}
