using Microsoft.AspNetCore.Mvc;

namespace Turnierplan.App.Pages.Shared.Components.Ranking;

public sealed class Ranking : ViewComponent
{
    public Task<IViewComponentResult> InvokeAsync(Turnierplan.Core.Tournament.Tournament tournament)
    {
        return Task.FromResult<IViewComponentResult>(View(new DataModel(tournament)));
    }

    public sealed record DataModel(Turnierplan.Core.Tournament.Tournament Tournament);
}
