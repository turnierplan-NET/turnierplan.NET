using Microsoft.AspNetCore.Mvc;
using Turnierplan.App.Security;
using Turnierplan.Core.PublicId;
using Turnierplan.Dal.Repositories;

namespace Turnierplan.App.Endpoints.TournamentPlanners;

internal sealed class DeleteTournamentPlannerEndpoint : EndpointBase
{
    protected override HttpMethod Method => HttpMethod.Delete;

    protected override string Route => "/api/tournament-planners/{id}";

    protected override Delegate Handler => Handle;

    private static async Task<IResult> Handle(
        [FromRoute] PublicId id,
        ITournamentPlannerRepository repository,
        IAccessValidator accessValidator,
        CancellationToken cancellationToken)
    {
        var tournamentPlanner = await repository.GetByPublicIdAsync(id);

        if (tournamentPlanner is null)
        {
            return Results.NotFound();
        }

        if (!accessValidator.IsActionAllowed(tournamentPlanner, Actions.GenericWrite))
        {
            return Results.Forbid();
        }

        repository.Remove(tournamentPlanner);

        await repository.UnitOfWork.SaveChangesAsync(cancellationToken);

        return Results.NoContent();
    }
}
