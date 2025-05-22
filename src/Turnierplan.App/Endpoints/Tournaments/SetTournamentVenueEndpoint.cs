using Microsoft.AspNetCore.Mvc;
using Turnierplan.App.Security;
using Turnierplan.Core.PublicId;
using Turnierplan.Core.Tournament;
using Turnierplan.Core.Venue;

namespace Turnierplan.App.Endpoints.Tournaments;

internal sealed class SetTournamentVenueEndpoint : EndpointBase
{
    protected override HttpMethod Method => HttpMethod.Patch;

    protected override string Route => "/api/tournaments/{id}/venue";

    protected override Delegate Handler => Handle;

    private static async Task<IResult> Handle(
        [FromRoute] PublicId id,
        [FromBody] SetTournamentVenueEndpointRequest request,
        ITournamentRepository tournamentRepository,
        IVenueRepository venueRepository,
        IAccessValidator accessValidator,
        CancellationToken cancellationToken)
    {
        var tournament = await tournamentRepository.GetByPublicIdAsync(id).ConfigureAwait(false);

        if (tournament is null)
        {
            return Results.NotFound();
        }

        if (!accessValidator.CanSessionUserAccess(tournament.Organization))
        {
            return Results.Forbid();
        }

        if (request.VenueId == null)
        {
            tournament.SetVenue(null);
        }
        else
        {
            var venue = await venueRepository.GetByPublicIdAsync(request.VenueId.Value).ConfigureAwait(false);

            if (venue is null)
            {
                return Results.NotFound();
            }

            if (!accessValidator.CanSessionUserAccess(venue.Organization))
            {
                return Results.Forbid();
            }

            if (tournament.Organization != venue.Organization)
            {
                return Results.BadRequest("Venue must belong to the same organization as the tournament.");
            }

            tournament.SetVenue(venue);
        }

        await tournamentRepository.UnitOfWork.SaveChangesAsync(cancellationToken).ConfigureAwait(false);

        return Results.NoContent();
    }

    public sealed record SetTournamentVenueEndpointRequest
    {
        public PublicId? VenueId { get; init; }
    }
}
