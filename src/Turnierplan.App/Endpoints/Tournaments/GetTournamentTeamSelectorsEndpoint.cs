using Microsoft.AspNetCore.Mvc;
using Turnierplan.App.Mapping;
using Turnierplan.App.Models;
using Turnierplan.App.Security;
using Turnierplan.Core.PublicId;
using Turnierplan.Core.Tournament;
using Turnierplan.Dal.Converters;
using Turnierplan.Localization;

namespace Turnierplan.App.Endpoints.Tournaments;

internal sealed class GetTournamentTeamSelectorsEndpoint : EndpointBase<IEnumerable<TeamSelectorDto>>
{
    protected override HttpMethod Method => HttpMethod.Get;

    protected override string Route => "/api/tournaments/{id}/team-selectors";

    protected override Delegate Handler => Handle;

    private static async Task<IResult> Handle(
        [FromRoute] PublicId id,
        [FromQuery] string languageCode,
        ITournamentRepository repository,
        IAccessValidator accessValidator,
        ILocalizationProvider localizationProvider,
        IMapper mapper)
    {
        var tournament = await repository.GetByPublicIdAsync(id, ITournamentRepository.Include.GameRelevant).ConfigureAwait(false);

        if (tournament is null)
        {
            return Results.NotFound();
        }

        if (!accessValidator.CanSessionUserAccess(tournament.Organization))
        {
            return Results.Forbid();
        }

        if (!localizationProvider.TryGetLocalization(languageCode, out var localization))
        {
            return Results.BadRequest("Invalid language code specified.");
        }

        var result = tournament.GenerateAllTeamSelectors()
            .Select(teamSelector => new TeamSelectorDto
            {
                Key = TeamSelectorConverter.ConvertTeamSelectorToString(teamSelector),
                Localized = localization.LocalizeTeamSelector(teamSelector, tournament)
            })
            .OrderBy(x => x.Localized)
            .ToList();

        return Results.Ok(result);
    }
}
