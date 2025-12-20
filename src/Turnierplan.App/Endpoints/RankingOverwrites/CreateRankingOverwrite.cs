using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Turnierplan.App.Extensions;
using Turnierplan.App.Security;
using Turnierplan.Core.PublicId;
using Turnierplan.Core.Tournament;
using Turnierplan.Dal.Repositories;

namespace Turnierplan.App.Endpoints.RankingOverwrites;

internal sealed class CreateRankingOverwrite : EndpointBase
{
    protected override HttpMethod Method => HttpMethod.Post;

    protected override string Route => "/api/tournaments/{id}/ranking-overwrites";

    protected override Delegate Handler => Handle;

    private static async Task<IResult> Handle(
        [FromRoute] PublicId id,
        [FromBody] CreateRankingOverwriteRequest request,
        ITournamentRepository repository,
        IAccessValidator accessValidator,
        CancellationToken cancellationToken)
    {
        if (!Validator.Instance.ValidateAndGetResult(request, out var result))
        {
            return result;
        }

        var tournament = await repository.GetByPublicIdAsync(id, ITournamentRepository.Includes.GameRelevant);

        if (tournament is null)
        {
            return Results.NotFound();
        }

        if (!accessValidator.IsActionAllowed(tournament, Actions.TournamentConduct))
        {
            return Results.Forbid();
        }

        if (request.HideRanking)
        {
            tournament.AddRankingOverwrite(request.PlacementRank, hideRanking: true);
        }
        else
        {
            Team? assignTeam;

            if (request.AssignTeamId.HasValue)
            {
                assignTeam = tournament.Teams.FirstOrDefault(x => x.Id == request.AssignTeamId.Value);

                if (assignTeam is null)
                {
                    return Results.NotFound();
                }
            }
            else
            {
                assignTeam = null;
            }

            tournament.AddRankingOverwrite(request.PlacementRank, assignTeam);
        }

        if (!tournament.TryCompute(out var errorResult))
        {
            return errorResult;
        }

        await repository.UnitOfWork.SaveChangesAsync(cancellationToken);

        return Results.NoContent();
    }

    public sealed record CreateRankingOverwriteRequest
    {
        public required int PlacementRank { get; init; }

        public required bool HideRanking { get; init; }

        public int? AssignTeamId { get; init; }
    }

    private sealed class Validator : AbstractValidator<CreateRankingOverwriteRequest>
    {
        public static readonly Validator Instance = new();

        private Validator()
        {
            RuleFor(x => x.PlacementRank)
                .GreaterThanOrEqualTo(1);

            RuleFor(x => x.AssignTeamId)
                .Null()
                .When(x => x.HideRanking);

            RuleFor(x => x.AssignTeamId)
                .GreaterThanOrEqualTo(1)
                .When(x => x.AssignTeamId.HasValue);
        }
    }
}
