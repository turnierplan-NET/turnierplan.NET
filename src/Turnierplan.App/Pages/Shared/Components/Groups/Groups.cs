using Microsoft.AspNetCore.Mvc;
using Turnierplan.Localization;

namespace Turnierplan.App.Pages.Shared.Components.Groups;

public sealed class Groups : ViewComponent
{
    public Task<IViewComponentResult> InvokeAsync(Turnierplan.Core.Tournament.Tournament tournament, ILocalization localization, bool inlineGroupNames)
    {
        return Task.FromResult<IViewComponentResult>(View(new DataModel(tournament, localization, inlineGroupNames)));
    }

    public sealed record DataModel(Turnierplan.Core.Tournament.Tournament Tournament, ILocalization Localization, bool InlineGroupNames);
}
