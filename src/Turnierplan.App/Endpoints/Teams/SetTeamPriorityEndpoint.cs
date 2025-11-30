using Microsoft.AspNetCore.Mvc;
using Turnierplan.App.Security;
using Turnierplan.Core.PublicId;
using Turnierplan.Dal.Repositories;

namespace Turnierplan.App.Endpoints.Teams;

internal sealed class SetTeamPriorityEndpoint : EndpointBase
{
    protected override HttpMethod Method => HttpMethod.Patch;

    protected override string Route => "/api/tournaments/{tournamentId}/teams/{teamId:int}/priority";

    protected override Delegate Handler => Handle;

    private static async Task<IResult> Handle(
        [FromRoute] PublicId tournamentId,
        [FromRoute] int teamId,
        [FromBody] SetTeamPriorityEndpointRequest request,
        ITournamentRepository repository,
        IAccessValidator accessValidator,
        CancellationToken cancellationToken)
    {
        var tournament = await repository.GetByPublicIdAsync(tournamentId, ITournamentRepository.Includes.Teams | ITournamentRepository.Includes.Groups);

        if (tournament is null)
        {
            return Results.NotFound();
        }

        if (!accessValidator.IsActionAllowed(tournament, Actions.TournamentConduct))
        {
            return Results.Forbid();
        }

        var group = tournament.Groups.FirstOrDefault(x => x.Id == request.GroupId);
        var participant = group?.Participants.FirstOrDefault(x => x.Team.Id == teamId);

        if (participant is null)
        {
            return Results.NotFound();
        }

        participant.Priority = request.Priority;

        await repository.UnitOfWork.SaveChangesAsync(cancellationToken);

        return Results.NoContent();
    }

    public sealed record SetTeamPriorityEndpointRequest
    {
        public required int GroupId { get; set; }

        public required int Priority { get; init; }
    }
}
