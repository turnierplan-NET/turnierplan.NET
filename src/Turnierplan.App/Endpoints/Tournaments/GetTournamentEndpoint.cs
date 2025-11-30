using Microsoft.AspNetCore.Mvc;
using Turnierplan.App.Extensions;
using Turnierplan.App.Mapping;
using Turnierplan.App.Models;
using Turnierplan.App.Security;
using Turnierplan.Core.PublicId;
using Turnierplan.Dal.Repositories;

namespace Turnierplan.App.Endpoints.Tournaments;

internal sealed class GetTournamentEndpoint : EndpointBase<TournamentDto>
{
    protected override HttpMethod Method => HttpMethod.Get;

    protected override string Route => "/api/tournaments/{id}";

    protected override Delegate Handler => Handle;

    protected override bool? AllowApiKeyAccess => true;

    private static async Task<IResult> Handle(
        [FromRoute] PublicId id,
        ITournamentRepository repository,
        IAccessValidator accessValidator,
        IMapper mapper)
    {
        var tournament = await repository.GetByPublicIdAsync(id, ITournamentRepository.Includes.GameRelevant | ITournamentRepository.Includes.Venue | ITournamentRepository.Includes.TeamsWithLinks);

        if (tournament is null)
        {
            return Results.NotFound();
        }

        if (!accessValidator.IsActionAllowed(tournament, Actions.GenericRead))
        {
            return Results.Forbid();
        }

        tournament.Compute();

        accessValidator.AddRolesToResponseHeader(tournament);

        var planningRealms = tournament.Teams.Select(x => x.TeamLink)
            .WhereNotNull()
            .Select(x => x.ApplicationTeam.Application.PlanningRealm)
            .Distinct();

        foreach (var planningRealm in planningRealms)
        {
            accessValidator.AddRolesToResponseHeader(planningRealm);
        }

        return Results.Ok(mapper.Map<TournamentDto>(tournament));
    }
}
