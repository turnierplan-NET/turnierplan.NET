using Microsoft.AspNetCore.Mvc;
using Turnierplan.App.Mapping;
using Turnierplan.App.Models;
using Turnierplan.App.Security;
using Turnierplan.Core.PublicId;
using Turnierplan.Dal.Repositories;

namespace Turnierplan.App.Endpoints.Venues;

internal sealed class GetVenuesEndpoint : EndpointBase<IEnumerable<VenueDto>>
{
    protected override HttpMethod Method => HttpMethod.Get;

    protected override string Route => "/api/venues";

    protected override Delegate Handler => Handle;

    private static async Task<IResult> Handle(
        [FromQuery] PublicId organizationId,
        IOrganizationRepository repository,
        IAccessValidator accessValidator,
        IMapper mapper)
    {
        var organization = await repository.GetByPublicIdAsync(organizationId, IOrganizationRepository.Includes.Venues);

        if (organization is null)
        {
            return Results.NotFound();
        }

        if (!accessValidator.IsActionAllowed(organization, Actions.GenericRead))
        {
            return Results.Forbid();
        }

        return Results.Ok(mapper.MapCollection<VenueDto>(organization.Venues));
    }
}
