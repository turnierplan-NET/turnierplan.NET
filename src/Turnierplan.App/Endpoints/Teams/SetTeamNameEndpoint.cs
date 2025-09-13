using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Turnierplan.App.Extensions;
using Turnierplan.App.Security;
using Turnierplan.Core.PublicId;
using Turnierplan.Core.Tournament;

namespace Turnierplan.App.Endpoints.Teams;

internal sealed class SetTeamNameEndpoint : EndpointBase
{
    protected override HttpMethod Method => HttpMethod.Patch;

    protected override string Route => "/api/tournaments/{tournamentId}/teams/{teamId:int}/name";

    protected override Delegate Handler => Handle;

    private static async Task<IResult> Handle(
        [FromRoute] PublicId tournamentId,
        [FromRoute] int teamId,
        [FromBody] SetTeamNameEndpointRequest request,
        ITournamentRepository repository,
        IAccessValidator accessValidator,
        CancellationToken cancellationToken)
    {
        if (!Validator.Instance.ValidateAndGetResult(request, out var result))
        {
            return result;
        }

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

        team.SetName(request.Name);

        await repository.UnitOfWork.SaveChangesAsync(cancellationToken);

        return Results.NoContent();
    }

    public sealed record SetTeamNameEndpointRequest
    {
        public required string Name { get; init; }
    }

    private sealed class Validator : AbstractValidator<SetTeamNameEndpointRequest>
    {
        public static readonly Validator Instance = new();

        private Validator()
        {
            RuleFor(x => x.Name)
                .NotEmpty();
        }
    }
}
