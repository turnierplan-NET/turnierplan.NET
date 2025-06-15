using Microsoft.AspNetCore.Mvc;
using Turnierplan.App.Mapping;
using Turnierplan.App.Models;
using Turnierplan.App.Security;
using Turnierplan.Core.PublicId;
using Turnierplan.Core.Tournament;

namespace Turnierplan.App.Endpoints.Tournaments;

internal sealed class GetTournamentImagesEndpoint : EndpointBase<TournamentImagesDto>
{
    protected override HttpMethod Method => HttpMethod.Get;

    protected override string Route => "/api/tournaments/{id}/images";

    protected override Delegate Handler => Handle;

    private static async Task<IResult> Handle(
        [FromRoute] PublicId id,
        ITournamentRepository repository,
        IAccessValidator accessValidator,
        IMapper mapper)
    {
        var tournament = await repository.GetByPublicIdAsync(id, ITournamentRepository.Include.Images).ConfigureAwait(false);

        if (tournament is null)
        {
            return Results.NotFound();
        }

        if (!accessValidator.IsActionAllowed(tournament, Actions.GenericRead))
        {
            return Results.Forbid();
        }

        return Results.Ok(mapper.Map<TournamentImagesDto>(tournament));
    }
}
