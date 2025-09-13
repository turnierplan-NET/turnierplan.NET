using Microsoft.AspNetCore.Mvc;
using Turnierplan.App.Security;
using Turnierplan.Core.PublicId;
using Turnierplan.Core.Tournament;

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

        team.EntryFeePaidAt = request.HasPaidEntryFee ? DateTime.UtcNow : null;

        await repository.UnitOfWork.SaveChangesAsync(cancellationToken);

        return Results.NoContent();
    }

    public sealed record SetTeamEntryFeePaidEndpointRequest
    {
        public required bool HasPaidEntryFee { get; init; }
    }
}
