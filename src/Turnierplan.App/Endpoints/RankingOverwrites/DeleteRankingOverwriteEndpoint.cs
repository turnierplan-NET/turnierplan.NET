using Microsoft.AspNetCore.Mvc;
using Turnierplan.App.Security;
using Turnierplan.Core.PublicId;
using Turnierplan.Dal.Repositories;

namespace Turnierplan.App.Endpoints.RankingOverwrites;

internal sealed class DeleteRankingOverwriteEndpoint : EndpointBase
{
    protected override HttpMethod Method => HttpMethod.Delete;

    protected override string Route => "/api/tournaments/{tournamentId}/ranking-overwrites/{rankingOverwriteId}";

    protected override Delegate Handler => Handle;

    private static async Task<IResult> Handle(
        [FromRoute] PublicId tournamentId,
        [FromRoute] int rankingOverwriteId,
        ITournamentRepository repository,
        IAccessValidator accessValidator,
        CancellationToken cancellationToken)
    {
        var tournament = await repository.GetByPublicIdAsync(tournamentId, ITournamentRepository.Includes.RankingOverwrites);

        if (tournament is null)
        {
            return Results.NotFound();
        }

        if (!accessValidator.IsActionAllowed(tournament, Actions.TournamentConduct))
        {
            return Results.Forbid();
        }

        var rankingOverwrite = tournament.RankingOverwrites.FirstOrDefault(x => x.Id == rankingOverwriteId);

        if (rankingOverwrite is null)
        {
            return Results.NotFound();
        }

        tournament.RemoveRankingOverwrite(rankingOverwrite);

        await repository.UnitOfWork.SaveChangesAsync(cancellationToken);

        return Results.NoContent();
    }
}
