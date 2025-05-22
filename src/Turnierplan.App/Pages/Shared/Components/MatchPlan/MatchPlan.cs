using Microsoft.AspNetCore.Mvc;
using Turnierplan.Localization;

namespace Turnierplan.App.Pages.Shared.Components.MatchPlan;

public sealed class MatchPlan : ViewComponent
{
    public Task<IViewComponentResult> InvokeAsync(Turnierplan.Core.Tournament.Tournament tournament, ILocalization localization)
    {
        return Task.FromResult<IViewComponentResult>(View(new DataModel(tournament, localization)));
    }

    public sealed record DataModel(Turnierplan.Core.Tournament.Tournament Tournament, ILocalization Localization);
}
