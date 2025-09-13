using Microsoft.AspNetCore.Mvc;
using Turnierplan.App.Security;
using Turnierplan.Core.PublicId;
using Turnierplan.Core.Tournament;

namespace Turnierplan.App.Endpoints.Teams;

internal sealed class SetTeamOutOfCompetitionEndpoint : EndpointBase
{
    protected override HttpMethod Method => HttpMethod.Patch;

    protected override string Route => "/api/tournaments/{tournamentId}/teams/{teamId:int}/out-of-competition";

    protected override Delegate Handler => Handle;

    private static async Task<IResult> Handle(
        [FromRoute] PublicId tournamentId,
        [FromRoute] int teamId,
        [FromBody] SetTeamOutOfCompetitionEndpointRequest request,
        ITournamentRepository repository,
        IAccessValidator accessValidator,
        CancellationToken cancellationToken)
    {
        var tournament = await repository.GetByPublicIdAsync(tournamentId, ITournamentRepository.Include.Teams);

        if (tournament is null)
        {
            return Results.NotFound();
        }

        if (!accessValidator.IsActionAllowed(tournament, Actions.GenericWrite))
        {
            return Results.Forbid();
        }

        var team = tournament.Teams.FirstOrDefault(x => x.Id == teamId);

        if (team is null)
        {
            return Results.NotFound();
        }

        team.OutOfCompetition = request.OutOfCompetition;

        await repository.UnitOfWork.SaveChangesAsync(cancellationToken);

        return Results.NoContent();
    }

    public sealed record SetTeamOutOfCompetitionEndpointRequest
    {
        public required bool OutOfCompetition { get; init; }
    }
}
