using Microsoft.AspNetCore.Mvc;
using Turnierplan.App.Security;
using Turnierplan.Core.PublicId;
using Turnierplan.Core.Venue;

namespace Turnierplan.App.Endpoints.Venues;

internal sealed class DeleteVenueEndpoint : EndpointBase
{
    protected override HttpMethod Method => HttpMethod.Delete;

    protected override string Route => "/api/venues/{id}";

    protected override Delegate Handler => Handle;

    private static async Task<IResult> Handle(
        [FromRoute] PublicId id,
        IVenueRepository repository,
        IAccessValidator accessValidator,
        CancellationToken cancellationToken)
    {
        var venue = await repository.GetByPublicIdAsync(id).ConfigureAwait(false);

        if (venue is null)
        {
            return Results.NotFound();
        }

        if (!accessValidator.IsActionAllowed(venue, Actions.GenericDelete))
        {
            return Results.Forbid();
        }

        repository.Remove(venue);

        await repository.UnitOfWork.SaveChangesAsync(cancellationToken).ConfigureAwait(false);

        return Results.NoContent();
    }
}
