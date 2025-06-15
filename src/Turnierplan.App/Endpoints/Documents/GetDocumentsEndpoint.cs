using Microsoft.AspNetCore.Mvc;
using Turnierplan.App.Mapping;
using Turnierplan.App.Models;
using Turnierplan.App.Security;
using Turnierplan.Core.PublicId;
using Turnierplan.Core.Tournament;

namespace Turnierplan.App.Endpoints.Documents;

internal sealed class GetDocumentsEndpoint : EndpointBase<IEnumerable<DocumentDto>>
{
    protected override HttpMethod Method => HttpMethod.Get;

    protected override string Route => "/api/documents";

    protected override Delegate Handler => Handle;

    private static async Task<IResult> Handle(
        [FromQuery] PublicId tournamentId,
        ITournamentRepository repository,
        IAccessValidator accessValidator,
        IMapper mapper)
    {
        var tournament = await repository.GetByPublicIdAsync(tournamentId, ITournamentRepository.Include.Documents).ConfigureAwait(false);

        if (tournament is null)
        {
            return Results.NotFound();
        }

        if (!accessValidator.IsActionAllowed(tournament, Actions.GenericRead))
        {
            return Results.Forbid();
        }

        return Results.Ok(mapper.MapCollection<DocumentDto>(tournament.Documents));
    }
}
