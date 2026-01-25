using Microsoft.AspNetCore.Mvc;
using Turnierplan.App.Security;
using Turnierplan.Core.PublicId;
using Turnierplan.Dal.Repositories;

namespace Turnierplan.App.Endpoints.Teams;

internal sealed class SetTeamEntryFeePaidEndpoint : EndpointBase
{
    protected override HttpMethod Method => HttpMethod.Patch;

    protected override string Route => "/api/tournaments/{tournamentId}/teams/{teamId:int}/entry-fee";

    protected override Delegate Handler => Handle;

    private static async Task<IResult> Handle(
        [FromRoute] PublicId tournamentId,
        [FromRoute] int teamId,
        [FromBody] SetTeamEntryFeePaidEndpointRequest request,
        ITournamentRepository repository,
        IAccessValidator accessValidator,
        CancellationToken cancellationToken)
    {
        var tournament = await repository.GetByPublicIdAsync(tournamentId, ITournamentRepository.Includes.Teams);

        if (tournament is null)
        {
            return Results.NotFound();
        }

        if (!accessValidator.IsActionAllowed(tournament, Actions.TournamentConduct))
        {
            return Results.Forbid();
        }

        var team = tournament.Teams.FirstOrDefault(x => x.Id == teamId);

        if (team is null)
        {
            return Results.NotFound();
        }

        if (request.HasPaidEntryFee)
        {
            if (team.EntryFeePaidAt.HasValue)
            {
                // If the team already has an "entry fee paid at" timestamp, we don't overwrite
                // the current value of the timestamp and instead, we exit early.

                return Results.NoContent();
            }

            team.EntryFeePaidAt = DateTime.UtcNow;
        }
        else
        {
            team.EntryFeePaidAt = null;
        }

        await repository.UnitOfWork.SaveChangesAsync(cancellationToken);

        return Results.NoContent();
    }

    public sealed record SetTeamEntryFeePaidEndpointRequest
    {
        public required bool HasPaidEntryFee { get; init; }
    }
}
