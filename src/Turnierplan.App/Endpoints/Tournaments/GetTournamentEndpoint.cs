using Microsoft.AspNetCore.Mvc;
using Turnierplan.App.Mapping;
using Turnierplan.App.Models;
using Turnierplan.App.Security;
using Turnierplan.Core.PublicId;
using Turnierplan.Core.Tournament;

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
        var tournament = await repository.GetByPublicIdAsync(id, ITournamentRepository.Include.GameRelevant | ITournamentRepository.Include.Venue | ITournamentRepository.Include.Folder).ConfigureAwait(false);

        if (tournament is null)
        {
            return Results.NotFound();
        }

        if (!accessValidator.CanSessionUserAccess(tournament.Organization))
        {
            return Results.Forbid();
        }

        tournament.Compute();

        return Results.Ok(mapper.Map<TournamentDto>(tournament));
    }
}
