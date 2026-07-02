using Microsoft.AspNetCore.Mvc;
using Turnierplan.App.Mapping;
using Turnierplan.App.Models;
using Turnierplan.App.Security;
using Turnierplan.Core.PublicId;
using Turnierplan.Dal.Repositories;

namespace Turnierplan.App.Endpoints.TournamentPlanners;

internal sealed class GetTournamentPlannerEndpoint : EndpointBase<TournamentPlannerDto>
{
    protected override HttpMethod Method => HttpMethod.Get;

    protected override string Route => "/api/tournament-planners/{id}";

    protected override Delegate Handler => Handle;

    private static async Task<IResult> Handle(
        [FromRoute] PublicId id,
        ITournamentPlannerRepository repository,
        IAccessValidator accessValidator,
        IMapper mapper)
    {
        // Note: We must use 'ApplicationsWithTeams' in order to have access to the number of applications/teams per invitation link/tournament class
        var tournamentPlanner = await repository.GetByPublicIdAsync(id, ITournamentPlannerRepository.Includes.All | ITournamentPlannerRepository.Includes.ApplicationsWithTeams);

        if (tournamentPlanner is null)
        {
            return Results.NotFound();
        }

        if (!accessValidator.IsActionAllowed(tournamentPlanner, Actions.GenericRead))
        {
            return Results.Forbid();
        }

        accessValidator.AddRolesToResponseHeader(tournamentPlanner);

        return Results.Ok(mapper.Map<TournamentPlannerDto>(tournamentPlanner));
    }
}
