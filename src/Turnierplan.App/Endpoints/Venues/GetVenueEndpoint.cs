using Microsoft.AspNetCore.Mvc;
using Turnierplan.App.Mapping;
using Turnierplan.App.Models;
using Turnierplan.App.Security;
using Turnierplan.Core.PublicId;
using Turnierplan.Dal.Repositories;

namespace Turnierplan.App.Endpoints.Venues;

internal sealed class GetVenueEndpoint : EndpointBase<VenueDto>
{
    protected override HttpMethod Method => HttpMethod.Get;

    protected override string Route => "/api/venues/{id}";

    protected override Delegate Handler => Handle;

    private static async Task<IResult> Handle(
        [FromRoute] PublicId id,
        IVenueRepository repository,
        IAccessValidator accessValidator,
        IMapper mapper)
    {
        var venue = await repository.GetByPublicIdAsync(id);

        if (venue is null)
        {
            return Results.NotFound();
        }

        if (!accessValidator.IsActionAllowed(venue, Actions.GenericRead))
        {
            return Results.Forbid();
        }

        accessValidator.AddRolesToResponseHeader(venue);

        return Results.Ok(mapper.Map<VenueDto>(venue));
    }
}
